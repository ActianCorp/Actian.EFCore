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
    public class ActianIngresDateTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActianIngresDateTypeMapping" /> class.
        /// </summary>
        /// <param name="clrType">The CLR type</param>
        public ActianIngresDateTypeMapping(
            [NotNull] Type clrType
            )
            : base("ingresdate", clrType ?? typeof(DateTime), System.Data.DbType.DateTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActianIngresDateTypeMapping" /> class.
        /// </summary>
        /// <param name="parameters"> Parameter object for <see cref="RelationalTypeMapping" />. </param>
        protected ActianIngresDateTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianIngresDateTypeMapping(parameters);

        /// <inheritdoc />
        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is IngresParameter ingresParameter)
                ingresParameter.IngresType = IngresType.IngresDate;
            else
                throw new InvalidOperationException($"Actian-specific type mapping {GetType().Name} being used with non-Actian parameter type {parameter.GetType().Name}");
        }

        /// <inheritdoc />
        protected override string GenerateNonNullSqlLiteral(object value) => value switch
        {
            DateTime date when date == date.Date => $"date('{date:yyyy_MM_dd}')",
            _ => $"date('{value:yyyy_MM_dd HH:mm:ss}')",
        };
    }
}
