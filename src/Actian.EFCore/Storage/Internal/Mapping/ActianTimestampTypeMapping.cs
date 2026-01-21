// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data;
using System.Data.Common;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianTimestampTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActianTimestampTypeMapping" /> class.
        /// </summary>
        /// <param name="storeType"> The name of the database type. </param>
        /// <param name="clrType">The CLR type</param>
        /// <param name="withTimeZone"></param>
        public ActianTimestampTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            bool withTimeZone = false
            )
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(clrType),
                    storeType,
                    storeTypePostfix: StoreTypePostfix.Precision,
                    dbType: System.Data.DbType.DateTime
                ),
                withTimeZone
            )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActianTimestampTypeMapping" /> class.
        /// </summary>
        /// <param name="parameters"> Parameter object for <see cref="RelationalTypeMapping" />. </param>
        /// <param name="withTimeZone"></param>
        protected ActianTimestampTypeMapping(RelationalTypeMappingParameters parameters, bool withTimeZone)
            : base(parameters)
        {
            WithTimeZone = withTimeZone;
        }

        public bool WithTimeZone { get; }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianTimestampTypeMapping(parameters, WithTimeZone);

        public override DbParameter CreateParameter(DbCommand command, string name, object value, bool? nullable = null, ParameterDirection direction = ParameterDirection.Input)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                value = ActianDateTimeOffsetConverter.ConvertFromDateTimeOffset(dateTimeOffset);
            }
            var param = base.CreateParameter(command, name, value, nullable);
            return param;
        }

        /// <inheritdoc />
        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is IngresParameter ingresParameter)
            {
                ingresParameter.IngresType = IngresType.DateTime;
                if (ingresParameter.DbType == System.Data.DbType.DateTimeOffset)
                {
                    ingresParameter.DbType = System.Data.DbType.DateTime;
                }
            }
            else
                throw new InvalidOperationException($"Actian-specific type mapping {GetType().Name} being used with non-Actian parameter type {parameter.GetType().Name}");
        }

        /// <inheritdoc />
        protected override string GenerateNonNullSqlLiteral(object value)
            => FormattableString.Invariant(GenerateNonNullSqlLiteralAsFormattableString(value));

        private FormattableString GenerateNonNullSqlLiteralAsFormattableString(object value) => value switch
        {
            DateTime dateTime when WithTimeZone => $"TIMESTAMP '{dateTime:yyyy-MM-dd HH:mm:ss.FFFFFFF}{GetTimeZone(dateTime)}'",
            DateTime dateTime => $"TIMESTAMP '{dateTime:yyyy-MM-dd HH:mm:ss.FFFFFFF}'",
            DateTimeOffset dateTimeOffset => $"TIMESTAMP '{dateTimeOffset:yyyy-MM-dd HH:mm:ss.FFFFFFFzzz}'",
            _ => throw new InvalidCastException($"Attempted to generate timestamp literal for type {value.GetType()}, only DateTime and DateTimeOffset are supported")
        };

        private string GetTimeZone(DateTime value)
            => value.Kind == DateTimeKind.Local ? FormattableString.Invariant($"{value:zzz}") : "+00:00";
    }
}
