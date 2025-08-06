// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

#nullable enable
namespace Actian.EFCore.Query.Internal
{
    public class ActianConvertTranslator : IMethodCallTranslator
    {
        private static readonly Dictionary<string, string> TypeMapping = new()
        {
            [nameof(Convert.ToBoolean)] = "bit",
            [nameof(Convert.ToByte)] = "tinyint",
            [nameof(Convert.ToDecimal)] = "decimal(18, 2)",
            [nameof(Convert.ToDouble)] = "float",
            [nameof(Convert.ToInt16)] = "smallint",
            [nameof(Convert.ToInt32)] = "int",
            [nameof(Convert.ToInt64)] = "bigint",
            [nameof(Convert.ToString)] = "nvarchar(max)"
        };

        private static readonly List<Type> SupportedTypes = new()
    {
        typeof(bool),
        typeof(byte),
        typeof(DateTime),
        typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(int),
        typeof(long),
        typeof(short),
        typeof(string)
    };

        private static readonly MethodInfo[] SupportedMethods
            = TypeMapping.Keys
                .SelectMany(
                    t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                        .Where(
                            m => m.GetParameters().Length == 1
                                && SupportedTypes.Contains(m.GetParameters().First().ParameterType)))
                .ToArray();

        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public ActianConvertTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression? Translate(
            SqlExpression? instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
            => SupportedMethods.Contains(method)
                ? _sqlExpressionFactory.Function(
                    "CONVERT",
                    new[] { _sqlExpressionFactory.Fragment(TypeMapping[method.Name]), arguments[0] },
                    nullable: true,
                    argumentsPropagateNullability: new[] { false, true },
                    method.ReturnType)
                : null;
    }
}
