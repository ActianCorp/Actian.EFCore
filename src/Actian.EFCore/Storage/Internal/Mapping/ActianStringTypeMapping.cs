// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data.Common;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianStringTypeMapping : StringTypeMapping
    {
        private readonly IngresType _ingresType;

        public static ActianStringTypeMapping UnicodeDefault { get; } = new(
            "nvarchar(max)", unicode: true, storeTypePostfix: StoreTypePostfix.None);

        public ActianStringTypeMapping(
            [CanBeNull] string storeType = null,
            bool unicode = false,
            int? size = null,
            bool fixedLength = false,
            bool unbounded = false,
            StoreTypePostfix? storeTypePostfix = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(string)),
                    storeType ?? (unicode, fixedLength, unbounded) switch
                    {
                        (true, _, true) => "long nvarchar",
                        (true, true, _) => "nchar",
                        (true, _, _) => "nvarchar",
                        (false, _, true) => "long varchar",
                        (false, true, _) => "char",
                        (false, _, _) => "varchar"
                    },
                    storeTypePostfix ?? unbounded switch
                    {
                        true => StoreTypePostfix.None,
                        false => StoreTypePostfix.Size
                    },
                    unicode switch
                    {
                        true => System.Data.DbType.String,
                        false => System.Data.DbType.AnsiString
                    },
                    unicode,
                    size,
                    fixedLength
                ),
                (unicode, fixedLength, unbounded) switch
                {
                    (true, _, true) => IngresType.LongNVarChar,
                    (true, true, _) => IngresType.NChar,
                    (true, _, _) => IngresType.NVarChar,
                    (false, _, true) => IngresType.LongVarChar,
                    (false, true, _) => IngresType.Char,
                    (false, _, _) => IngresType.VarChar,
                }
            )
        {
        }

        protected ActianStringTypeMapping(RelationalTypeMappingParameters parameters, IngresType ingresType)
            : base(parameters)
        {
            _ingresType = ingresType;
        }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianStringTypeMapping(parameters, _ingresType);

        /// <inheritdoc />
        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is IngresParameter ingresParameter)
                ingresParameter.IngresType = _ingresType;
            else
                throw new InvalidOperationException($"Actian-specific type mapping {GetType().Name} being used with non-Actian parameter type {parameter.GetType().Name}");
        }

        /// <inheritdoc />
        protected override string GenerateNonNullSqlLiteral(object value) => IsUnicode
            ? $"N'{EscapeSqlLiteral((string)value)}'" // Interpolation okay; strings
            : $"'{EscapeSqlLiteral((string)value)}'";
    }
}
