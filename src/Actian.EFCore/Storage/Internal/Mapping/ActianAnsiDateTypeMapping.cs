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
    public class ActianAnsiDateTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActianAnsiDateTypeMapping" /> class.
        /// </summary>
        /// <param name="clrType">The CLR type</param>
        public ActianAnsiDateTypeMapping(
            [NotNull] Type clrType
            )
            : base("ansidate", clrType ?? typeof(DateTime), System.Data.DbType.Date)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActianAnsiDateTypeMapping" /> class.
        /// </summary>
        /// <param name="parameters"> Parameter object for <see cref="RelationalTypeMapping" />. </param>
        protected ActianAnsiDateTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianAnsiDateTypeMapping(parameters);

        /// <inheritdoc />
        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (parameter is IngresParameter ingresParameter)
                ingresParameter.IngresType = IngresType.Date;
            else
                throw new InvalidOperationException($"Actian-specific type mapping {GetType().Name} being used with non-Actian parameter type {parameter.GetType().Name}");
        }

        /// <inheritdoc />
        protected override string SqlLiteralFormatString { get; } = "DATE '{0:yyyy-MM-dd}'";
    }
}
