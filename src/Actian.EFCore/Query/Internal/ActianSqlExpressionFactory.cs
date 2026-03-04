// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

#nullable enable

namespace Actian.EFCore.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class ActianSqlExpressionFactory : SqlExpressionFactory
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private RelationalTypeMapping? _nonNullableBoolTypeMapping;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public ActianSqlExpressionFactory(
        SqlExpressionFactoryDependencies dependencies)
        : base(dependencies)
        => _typeMappingSource = dependencies.TypeMappingSource;

    /// <summary>
    ///     Override Constant to ensure boolean constants use non-nullable type mapping.
    ///     This is critical for EXISTS, IN, LIKE expressions wrapped in CASE statements.
    /// </summary>
    public override SqlConstantExpression Constant(object? value, RelationalTypeMapping? typeMapping = null)
    {
        // For boolean constants, ensure we use non-nullable bool type mapping
        if (value is bool && typeMapping == null)
        {
            _nonNullableBoolTypeMapping ??= _typeMappingSource.FindMapping(typeof(bool));
            typeMapping = _nonNullableBoolTypeMapping;
        }

        return (SqlConstantExpression)base.Constant(value!, typeMapping);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    [return: NotNullIfNotNull(nameof(sqlExpression))]
    public override SqlExpression? ApplyTypeMapping(SqlExpression? sqlExpression, RelationalTypeMapping? typeMapping)
    {
        if (sqlExpression == null)
        {
            return null;
        }

        // Handle search condition expressions that should return non-nullable bool
        // Only handle ExistsExpression and LikeExpression which are simple
        // InExpression is more complex with child expressions, so let base class handle it entirely
        if (sqlExpression is ExistsExpression or LikeExpression)
        {
            _nonNullableBoolTypeMapping ??= _typeMappingSource.FindMapping(typeof(bool));
            
            // If the expression already has a nullable bool mapping, or if no mapping was provided,
            // use non-nullable bool mapping instead
            if (sqlExpression.TypeMapping?.ClrType == typeof(bool?) || typeMapping?.ClrType == typeof(bool?))
            {
                typeMapping = _nonNullableBoolTypeMapping;
            }
            else if (typeMapping == null && sqlExpression.TypeMapping == null)
            {
                typeMapping = _nonNullableBoolTypeMapping;
            }
            
            // Allow early return if already properly mapped
            if (sqlExpression.TypeMapping != null && typeMapping?.ClrType == sqlExpression.TypeMapping.ClrType)
            {
                return sqlExpression;
            }
        }
        else if (sqlExpression.TypeMapping != null)
        {
            // Early return for other expressions that already have type mappings
            return sqlExpression;
        }

        // Let base class handle the actual type mapping application
        return sqlExpression switch
        {
            AtTimeZoneExpression e => ApplyTypeMappingOnAtTimeZone(e, typeMapping),
            ActianAggregateFunctionExpression e => e.ApplyTypeMapping(typeMapping),

            _ => base.ApplyTypeMapping(sqlExpression, typeMapping)
        };
    }

    private SqlExpression ApplyTypeMappingOnAtTimeZone(AtTimeZoneExpression atTimeZoneExpression, RelationalTypeMapping? typeMapping)
    {
        var operandTypeMapping = typeMapping is null
            ? null
            : atTimeZoneExpression.Operand.Type == typeof(DateTimeOffset)
                ? typeMapping
                : atTimeZoneExpression.Operand.Type == typeof(DateTime)
                    ? _typeMappingSource.FindMapping(typeof(DateTime), "datetime2", precision: typeMapping.Precision)
                    : null;

        return new AtTimeZoneExpression(
            operandTypeMapping is null ? atTimeZoneExpression.Operand : ApplyTypeMapping(atTimeZoneExpression.Operand, operandTypeMapping),
            atTimeZoneExpression.TimeZone,
            atTimeZoneExpression.Type,
            typeMapping);
    }
}
