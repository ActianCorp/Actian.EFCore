// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Actian.EFCore.Parsing.Internal
{
    public class ActianSqlTokenizer : IEnumerator<ActianSqlToken>
    {
        public static ActianSqlTokenizer Tokenize(string text)
        {
            return new ActianSqlTokenizer(new StringReader(text), true);
        }

        public static ActianSqlTokenizer Tokenize(TextReader reader, bool disposeReader = false)
        {
            return new ActianSqlTokenizer(reader, disposeReader);
        }

        private const int BufferLength = 10;
        private readonly TextReader _reader;
        private readonly bool _disposeReader;
        private readonly char?[] _buffer = new char?[BufferLength];
        private readonly StringBuilder _value = new StringBuilder();
        private char? _currentChar => _buffer[0];
        private bool _endOfText => _currentChar is null;
        private int _pos = 0;
        private int _line = 1;
        private int _column = 1;
        private int _tokenPos = 0;
        private int _tokenLine = 1;
        private int _tokenColumn = 1;

        private ActianSqlTokenizer(TextReader reader, bool disposeReader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _disposeReader = disposeReader;
            Init();
        }

        public ActianSqlToken Current { get; private set; }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_endOfText)
            {
                Current = null;
                return false;
            }

            Current = ReadNextToken();
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            if (_disposeReader)
            {
                _reader.Dispose();
            }
        }

        private ActianSqlToken ReadNextToken()
        {
            _value.Clear();
            _tokenPos = _pos;
            _tokenLine = _line;
            _tokenColumn = _column;

            return ReadNewLine()
                ?? ReadToken(ActianSqlTokenType.WhiteSpace, StartsWithWhiteSpace, rest: () => StartsWithWhiteSpace)
                ?? ReadToken(ActianSqlTokenType.LineComment, StartsWith("--"), rest: () => HasChar && !StartsWithNewLineChar)
                ?? ReadBlockComment()
                ?? ReadCommandOrSymbol()
                ?? ReadString()
                ?? ReadToken(ActianSqlTokenType.Word, StartsWithLetter || StartsWith('_'), rest: () => StartsWithLetterOrDigit || StartsWithAnyOf("_."))
                ?? ReadToken(ActianSqlTokenType.Number, StartsWithDigit, rest: () => StartsWithDigit || StartsWith('.'))
                ?? ReadCharToken(ActianSqlTokenType.Semicolon, StartsWith(';'))
                ?? ReadCharToken(ActianSqlTokenType.Symbol);
        }

        private ActianSqlToken ReadNewLine()
        {
            if (!StartsWithNewLineChar)
                return null;

            if (StartsWith('\r'))
                Advance();

            if (StartsWith('\n'))
                Advance();

            _line += 1;
            _column = 1;

            return CreateToken(ActianSqlTokenType.NewLine);
        }

        private ActianSqlToken ReadToken(ActianSqlTokenType type, bool first, Func<bool> rest)
        {
            if (!first)
                return null;

            while (rest())
                Advance();

            return CreateToken(type);
        }

        private ActianSqlToken ReadToken(ActianSqlTokenType type, Func<bool> rest)
        {
            if (!rest())
                return null;

            while (rest())
                Advance();

            return CreateToken(type);
        }

        private ActianSqlToken ReadCharToken(ActianSqlTokenType type, bool first = true)
        {
            if (!first)
                return null;

            Advance();
            return CreateToken(type);
        }

        private ActianSqlToken ReadBlockComment()
        {
            if (!StartsWith("/*"))
                return null;

            Advance();
            Advance();

            do
            {
                while (HasChar && !StartsWith('*'))
                    Advance();

                if (StartsWith('*'))
                    Advance();
            }
            while (HasChar && !StartsWith('/'));

            if (StartsWith('/'))
                Advance();

            return CreateToken(ActianSqlTokenType.BlockComment);
        }

        private ActianSqlToken ReadString()
        {
            if (!StartsWith('\''))
                return null;

            while (StartsWith('\''))
            {
                Advance();

                while (HasChar && !StartsWith('\''))
                    Advance();

                if (StartsWith('\''))
                    Advance();
            }

            return CreateToken(ActianSqlTokenType.String);
        }

        private ActianSqlToken ReadCommandOrSymbol()
        {
            if (!StartsWith('\\'))
                return null;

            Advance();

            if (!StartsWithLetter)
                return CreateToken(ActianSqlTokenType.Symbol);

            return ReadToken(ActianSqlTokenType.Command, () => StartsWithLetter);
        }

        private ActianSqlToken CreateToken(ActianSqlTokenType type)
        {
            return new ActianSqlToken(type, _value.ToString(), _tokenPos, _tokenLine, _tokenColumn);
        }

        private bool HasChar => _currentChar.HasValue;
        private bool StartsWith(char c) => _currentChar == c;
        private bool StartsWith(string str)
        {
            if (str.Length > _buffer.Length)
                throw new Exception("String is longer than buffer");
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != _buffer[i])
                    return false;
            }
            return true;
        }
        private bool StartsWithAnyOf(IEnumerable<char> chars) => HasChar && chars.Contains(_currentChar.Value);
        private bool StartsWithNewLineChar => StartsWithAnyOf("\r\n");
        private bool StartsWithWhiteSpace => HasChar && char.IsWhiteSpace(_currentChar.Value) && !StartsWithNewLineChar;
        private bool StartsWithLetter => HasChar && char.IsLetter(_currentChar.Value);
        private bool StartsWithLetterOrDigit => HasChar && char.IsLetterOrDigit(_currentChar.Value);
        private bool StartsWithDigit => HasChar && char.IsDigit(_currentChar.Value);

        private void Init()
        {
            for (var i = 0; i < BufferLength; i++)
            {
                _buffer[i] = CodeToChar(_reader.Read());
            }
        }

        private void Advance(uint count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                if (_endOfText)
                    return;

                if (_currentChar.HasValue)
                {
                    _pos += 1;
                    _column += 1;
                    _value.Append(_currentChar.Value);
                }

                Array.Copy(_buffer, 1, _buffer, 0, _buffer.Length - 1);
                _buffer[_buffer.Length - 1] = CodeToChar(_reader.Read());
            }
        }

        private static char? CodeToChar(int code)
        {
            if (code < 0)
                return null;
            return (char)code;
        }
    }
}
