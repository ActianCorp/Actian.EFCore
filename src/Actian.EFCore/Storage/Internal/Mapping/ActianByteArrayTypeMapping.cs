// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianByteArrayTypeMapping : ByteArrayTypeMapping
    {
        private readonly IngresType _ingresType;

        public ActianByteArrayTypeMapping(
            [CanBeNull] string storeType = null,
            int? size = null,
            bool fixedLength = false,
            bool unbounded = false,
            ValueComparer comparer = null,
            StoreTypePostfix? storeTypePostfix = null)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(byte[]), null, comparer),
                    storeType ?? (fixedLength, unbounded) switch
                    {
                        (_, true) => "long byte",
                        (true, _) => "byte",
                        (_, _) => "varbyte"
                    },
                    storeTypePostfix ?? unbounded switch
                    {
                        true => StoreTypePostfix.None,
                        false => StoreTypePostfix.Size
                    },
                    System.Data.DbType.Binary,
                    size: size,
                    fixedLength: fixedLength
                ),
                (fixedLength, unbounded) switch
                {
                    (_, true) => IngresType.LongVarBinary,
                    (true, _) => IngresType.Binary,
                    (_, _) => IngresType.VarBinary,
                }
            )
        {
        }

        protected ActianByteArrayTypeMapping(RelationalTypeMappingParameters parameters, IngresType ingresType)
            : base(parameters)
        {
            _ingresType = ingresType;
        }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianByteArrayTypeMapping(parameters, _ingresType);

        /// <inheritdoc />
        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is IngresParameter ingresParameter)
                ingresParameter.IngresType = _ingresType;
            else
                throw new InvalidOperationException($"Actian-specific type mapping {GetType().Name} being used with non-Actian parameter type {parameter.GetType().Name}");
        }

        /// <inheritdoc />
        protected override string GenerateNonNullSqlLiteral(object value)
        {
            var builder = new StringBuilder();
            builder.Append("0x");

            foreach (var @byte in (byte[])value)
            {
                builder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}
