﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using Actian.EFCore.Utilities;
using Microsoft.EntityFrameworkCore.Query;

namespace Actian.EFCore.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class ActianParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public ActianParameterBasedSqlProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression Optimize(
        Expression queryExpression,
        IReadOnlyDictionary<string, object?> parametersValues,
        out bool canCache)
    {
        var optimizedQueryExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
            .Process(queryExpression, parametersValues, out var canCache2);

        optimizedQueryExpression = base.Optimize(optimizedQueryExpression, parametersValues, out canCache);

        canCache &= canCache2;

        return new SearchConditionConvertingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(optimizedQueryExpression);
    }

    /// <inheritdoc />
    protected override Expression ProcessSqlNullability(
        Expression selectExpression,
        IReadOnlyDictionary<string, object?> parametersValues,
        out bool canCache)
    {
        Check.NotNull(selectExpression, nameof(selectExpression));
        Check.NotNull(parametersValues, nameof(parametersValues));

        return new ActianSqlNullabilityProcessor(Dependencies, Parameters).Process(
            selectExpression, parametersValues, out canCache);
    }
}
