// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Sprache;
using static Sprache.Parse;

namespace Actian.EFCore.Parsing.Internal
{
    internal static class ActianSqlGrammar
    {
        public static Parser<T> OneOf<T>(Parser<T> parser, params Parser<T>[] otherParsers)
        {
            foreach (var otherParser in otherParsers)
            {
                parser = parser.Or(otherParser);
            }
            return parser;
        }

        public static Parser<T> XOneOf<T>(Parser<T> parser, params Parser<T>[] otherParsers)
        {
            foreach (var otherParser in otherParsers)
            {
                parser = parser.XOr(otherParser);
            }
            return parser;
        }

        public static Parser<string> Strings(params string[] strings)
        {
            return Strings(strings.AsEnumerable());
        }

        public static Parser<string> Strings(IEnumerable<string> strings)
        {
            var parser = String(strings.First()).Text();
            foreach (var str in strings.Skip(1))
            {

                parser = parser.Or(String(str).Text());
            }
            return parser;
        }

        public static Parser<T> If<T>(bool choice, Parser<T> parser1, Parser<T> parser2)
        {
            return choice ? parser1 : parser2;
        }

        public static Parser<T> Optional<T>(this Parser<T> parser, T defaultValue)
        {
            return parser.Optional().Select(result => result.IsDefined ? result.Get() : defaultValue);
        }

        public static Parser<T> Between<T, U>(this Parser<T> parser, Parser<U> surroundingParser)
        {
            return parser.Between(surroundingParser, surroundingParser);
        }

        public static Parser<T> Between<T, U, V>(this Parser<T> parser, Parser<U> leadingParser, Parser<V> trailingParser)
        {
            return from leading in leadingParser
                   from result in parser
                   from trailing in trailingParser
                   select result;
        }

        public static Parser<T> Before<T, U>(this Parser<T> parser, Parser<U> trailingParser)
        {
            return from result in parser
                   from trailing in trailingParser
                   select result;
        }

        public static Parser<U> Then<T, U>(this Parser<T> firstParser, Parser<U> secondParser)
        {
            return from first in firstParser
                   from second in secondParser
                   select second;
        }

        public static readonly Parser<string> Space =
            WhiteSpace.AtLeastOnce().Text();

        public static readonly Parser<string> WSpace =
            WhiteSpace.Many().Text();

        public static readonly Parser<char> Comma =
            Char(',').Between(WSpace);

        public static readonly Parser<char> Period =
            Char('.').Between(WSpace);

        public static readonly Parser<char> SemiColon =
            Char('.').Between(WSpace);

        public static readonly Parser<int> UnsignedInteger =
            Digit.AtLeastOnce().Text().Select(int.Parse);

        public static readonly Parser<int> Integer =
            from sign in Chars("+-").Optional()
            from number in UnsignedInteger
            select sign.GetOrElse('+') == '-' ? number * -1 : number;

        //public static readonly Parser<char> Letter =
        //    Char(c => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z', "Letter");

        //public static readonly Parser<char> Digit =
        //    Char(c => c >= '0' && c <= '9', "Digit");

        public static readonly Parser<string> UndelimitedIdentifier =
            from first in Char('_').Or(Letter)
            from rest in Char('_').Or(Letter).Or(Digit).Or(Chars("#@$")).Many().Text()
            select first + rest;

        public static readonly Parser<char> DelimitedIdentifierChar =
            String("\"\"").Return('"').Or(CharExcept('"'));

        public static readonly Parser<string> DelimitedIdentifier =
            DelimitedIdentifierChar.AtLeastOnce().Text().Between(Char('"'));

        public static readonly Parser<string> Identifier =
            DelimitedIdentifier.Or(UndelimitedIdentifier);

        public static readonly Parser<IEnumerable<string>> Keys =
            Identifier.DelimitedBy(Comma, minimumCount: 1, maximumCount: null).Between(WSpace).Between(Char('('), Char(')'));

        public static readonly Parser<(string schema, string name)> TableNameWithSchema =
            from schema in Identifier.Before(Period)
            from name in Identifier
            select (schema, name);

        public static readonly Parser<(string schema, string name)> TableNameWithoutSchema =
            Identifier.Select(name => ((string)null, name));

        public static readonly Parser<(string schema, string name)> TableName =
            TableNameWithSchema.Or(TableNameWithoutSchema);

        public static Parser<string> KeyWord(string keyWord)
        {
            var words = keyWord.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            var parser = IgnoreCase(words.First());

            foreach (var word in words.Skip(1))
            {
                parser = parser.Then(Space).Then(IgnoreCase(word));
            }

            return parser.Return(string.Join(" ", words).ToLowerInvariant());
        }
    }
}
