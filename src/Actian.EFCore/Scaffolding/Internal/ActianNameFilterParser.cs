// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using Actian.EFCore.Internal;
using Actian.EFCore.Parsing.Internal;
using JetBrains.Annotations;
using Sprache;
using static Actian.EFCore.Parsing.Internal.ActianSqlGrammar;
using static Sprache.Parse;

namespace Actian.EFCore.Scaffolding.Internal
{
    public static class ActianNameFilterParser
    {
        public static string ParseSchemaName(
            [NotNull] string schema,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var result = Identifier.End().TryParse(schema);
            if (!result.WasSuccessful)
            {
                throw new InvalidOperationException(ActianStrings.InvalidTableToIncludeInScaffolding(schema)); // TODO: Better message
            }
            return NormalizeCase(result.Value, dbNameCase, dbDelimitedCase);
        }

        public static (string table, string schema, string name) ParseTableName(
            [NotNull] string table,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var result = TableName.End().TryParse(table);
            if (!result.WasSuccessful)
            {
                throw new InvalidOperationException(ActianStrings.InvalidTableToIncludeInScaffolding(table));
            }
            var (schema, name) = result.Value;
            return (table, NormalizeCase(schema, dbNameCase, dbDelimitedCase), NormalizeCase(name, dbNameCase, dbDelimitedCase));
        }

        private static string NormalizeCase((string name, bool delimited) value, ActianCasing dbNameCase, ActianCasing dbDelimitedCase) => value switch
        {
            (_, true) => dbDelimitedCase.Normalize(value.name),
            (_, false) => dbNameCase.Normalize(value.name)
        };

        public static readonly Parser<string> UndelimitedIdentifier =
            from first in CharExcept("\".")
            from rest in CharExcept('.').Many().Text()
            select first + rest;

        public static readonly Parser<(string name, bool delimited)> Identifier = OneOf(
            DelimitedIdentifier.Select(name => (name, true)),
            UndelimitedIdentifier.Select(name => (name, false))
        );

        public static readonly Parser<((string name, bool delimited) schema, (string name, bool delimited) name)> TableNameWithSchema =
            from schema in Identifier.Before(Period)
            from name in Identifier
            select (schema, name);

        public static readonly (string name, bool delimited) NoName = ((string)null, false);

        public static readonly Parser<((string name, bool delimited) schema, (string name, bool delimited) name)> TableNameWithoutSchema =
            Identifier.Select(name => (NoName, name));

        public static readonly Parser<((string name, bool delimited) schema, (string name, bool delimited) name)> TableName =
            TableNameWithSchema.Or(TableNameWithoutSchema);
    }
}
