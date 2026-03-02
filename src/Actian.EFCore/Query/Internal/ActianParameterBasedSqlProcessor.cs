// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Linq.Expressions;
using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Actian.EFCore.Query.Internal;

public class ActianParameterBasedSqlProcessor(
    RelationalParameterBasedSqlProcessorDependencies dependencies,
    RelationalParameterBasedSqlProcessorParameters parameters,
    IActianSingletonOptions actianSingletonOptions)
    : RelationalParameterBasedSqlProcessor(dependencies, parameters)
{
    private readonly IActianSingletonOptions _actianSingletonOptions = actianSingletonOptions;

    public override Expression Process(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
    {
        var afterZeroLimitConversion = new ActianZeroLimitConverter(Dependencies.SqlExpressionFactory)
            .Process(queryExpression, parametersDecorator);

        var afterBaseProcessing = base.Process(afterZeroLimitConversion, parametersDecorator);

        var afterSearchConditionConversion = new SearchConditionConverter(
            Dependencies.SqlExpressionFactory,
            Dependencies.TypeMappingSource)
            .Visit(afterBaseProcessing);

        return afterSearchConditionConversion;
    }

    /// <inheritdoc />
    protected override Expression ProcessSqlNullability(Expression selectExpression, ParametersCacheDecorator Decorator)
        => new ActianSqlNullabilityProcessor(Dependencies, Parameters, _actianSingletonOptions)
            .Process(selectExpression, Decorator);
}
