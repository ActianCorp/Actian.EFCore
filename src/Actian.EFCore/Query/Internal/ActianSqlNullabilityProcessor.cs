// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Query.Internal;

#nullable enable

/// <summary>
///     Ingres-specific nullability processor. 
///     Removes SQL Server–specific JSON and parameter logic while retaining EF Core nullability inference.
/// </summary>
public class ActianSqlNullabilityProcessor : SqlNullabilityProcessor
{
    private readonly IActianSingletonOptions _actianServerSingletonOptions;

    public ActianSqlNullabilityProcessor(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        RelationalParameterBasedSqlProcessorParameters parameters,
        IActianSingletonOptions actianSingletonOptions)
        : base(dependencies, parameters)
        => _actianServerSingletonOptions = actianSingletonOptions;

    /// <summary>
    ///     Override to force MultipleParameters mode since Actian/Ingres does not support
    ///     JSON-based collection parameters (OPENJSON, etc.).
    /// </summary>
    public override ParameterTranslationMode CollectionParameterTranslationMode
        => ParameterTranslationMode.MultipleParameters;

    /// <summary>
    ///     Entry point for processing query expressions to infer nullability.
    /// </summary>
    public override Expression Process(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
    {
        // Pre-count parameters so that bucketization padding is calculated correctly.
        // The count itself is not used, but visiting triggers side-effects in parametersDecorator.
        var parametersCounter = new ParametersCounter(
            parametersDecorator,
            CollectionParameterTranslationMode,
#pragma warning disable EF1001
            (count, elementTypeMapping) => CalculatePadding(count, CalculateParameterBucketSize(count, elementTypeMapping)));
#pragma warning restore EF1001
        parametersCounter.Visit(queryExpression);

        return base.Process(queryExpression, parametersDecorator);
    }

    /// <summary>
    ///     Override VisitCase to ensure CASE expressions that wrap boolean search conditions
    ///     (EXISTS, IN, LIKE) with TRUE/FALSE constants are treated as non-nullable.
    /// </summary>
    protected override SqlExpression VisitCase(CaseExpression caseExpression, bool allowOptimizedExpansion, out bool nullable)
    {
        var visited = base.VisitCase(caseExpression, allowOptimizedExpansion, out nullable);

        // Check if this is a CASE expression generated for search condition conversion
        // Pattern: CASE WHEN <search_condition> THEN true ELSE false END
        if (visited is CaseExpression visitedCase && IsBooleanSearchConditionCase(visitedCase))
        {
            // This type of CASE expression always returns non-nullable boolean
            nullable = false;
            
            // Ensure the type mapping is for non-nullable bool
            if (visitedCase.TypeMapping is RelationalTypeMapping typeMapping 
                && typeMapping.ClrType == typeof(bool?))
            {
                // Get non-nullable bool type mapping
                var nonNullableTypeMapping = Dependencies.TypeMappingSource.FindMapping(typeof(bool));
                if (nonNullableTypeMapping != null)
                {
                    // Apply the non-nullable type mapping to the CASE expression
                    return Dependencies.SqlExpressionFactory.ApplyTypeMapping(visitedCase, nonNullableTypeMapping);
                }
            }
        }

        return visited;
    }

    /// <summary>
    ///     Determines if a CASE expression is wrapping a search condition (EXISTS, IN, LIKE)
    ///     with TRUE/FALSE constants, which should always be non-nullable.
    /// </summary>
    private static bool IsBooleanSearchConditionCase(CaseExpression caseExpression)
    {
        // Pattern: CASE WHEN <condition> THEN true ELSE false END
        if (caseExpression.Operand == null  // Simple CASE (not CASE x WHEN...)
            && caseExpression.WhenClauses.Count == 1
            && caseExpression.ElseResult is SqlConstantExpression { Value: bool elseValue }
            && elseValue == false)
        {
            var whenClause = caseExpression.WhenClauses[0];
            
            // Check if the result is TRUE constant
            if (whenClause.Result is SqlConstantExpression { Value: bool resultValue } && resultValue == true)
            {
                // Check if the test is a search condition (EXISTS, IN, LIKE, or comparison)
                return IsSearchCondition(whenClause.Test);
            }
        }

        return false;
    }

    /// <summary>
    ///     Determines if an expression is a search condition that inherently returns non-nullable boolean.
    /// </summary>
    private static bool IsSearchCondition(SqlExpression expression)
    {
        return expression is ExistsExpression
            or InExpression
            or LikeExpression
            or SqlBinaryExpression
            {
                OperatorType: ExpressionType.Equal
                or ExpressionType.NotEqual
                or ExpressionType.GreaterThan
                or ExpressionType.GreaterThanOrEqual
                or ExpressionType.LessThan
                or ExpressionType.LessThanOrEqual
                or ExpressionType.AndAlso
                or ExpressionType.OrElse
            };
    }

    /// <summary>
    ///     Visits Actian-specific expressions or falls back to the base implementation.
    /// </summary>
    protected override SqlExpression VisitCustomSqlExpression(
        SqlExpression sqlExpression,
        bool allowOptimizedExpansion,
        out bool nullable)
        => sqlExpression switch
        {
            ActianAggregateFunctionExpression aggregateFunctionExpression
                => VisitActianAggregateFunction(aggregateFunctionExpression, allowOptimizedExpansion, out nullable),

            _ => base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable)
        };

    /// <summary>
    ///     Handles aggregate function expressions, preserving nullability.
    /// </summary>
    protected virtual SqlExpression VisitActianAggregateFunction(
        ActianAggregateFunctionExpression aggregateFunctionExpression,
        bool allowOptimizedExpansion,
        out bool nullable)
    {
        nullable = aggregateFunctionExpression.IsNullable;

        SqlExpression[]? arguments = null;
        for (var i = 0; i < aggregateFunctionExpression.Arguments.Count; i++)
        {
            var visitedArg = Visit(aggregateFunctionExpression.Arguments[i], out _);
            if (visitedArg != aggregateFunctionExpression.Arguments[i] && arguments is null)
            {
                arguments = new SqlExpression[aggregateFunctionExpression.Arguments.Count];
                for (var j = 0; j < i; j++)
                {
                    arguments[j] = aggregateFunctionExpression.Arguments[j];
                }
            }

            if (arguments is not null)
            {
                arguments[i] = visitedArg;
            }
        }

        return arguments is not null
            ? aggregateFunctionExpression.Update(
                arguments ?? aggregateFunctionExpression.Arguments,
                aggregateFunctionExpression.Orderings)
            : aggregateFunctionExpression;
    }

    /// <summary>
    ///     Ingres prefers EXISTS for null-safe IN evaluation.
    /// </summary>
    protected override bool PreferExistsToInWithCoalesce
        => true;

    /// <summary>
    ///     Ingres does not require collection-table handling (no OPENJSON or VALUES() hack).
    /// </summary>
    protected override bool IsCollectionTable(TableExpressionBase table, [NotNullWhen(true)] out Expression? collection)
    {
        collection = null;
        return false;
    }

    public class ParametersCounter(
    ParametersCacheDecorator parametersDecorator,
    ParameterTranslationMode collectionParameterTranslationMode,
    Func<int, RelationalTypeMapping, int> bucketizationPadding) : ExpressionVisitor
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int Count { get; private set; }

        private readonly HashSet<SqlParameterExpression> _visitedSqlParameters =
            new(EqualityComparer<SqlParameterExpression>.Create(
                (lhs, rhs) =>
                    ReferenceEquals(lhs, rhs)
                    || (lhs is not null && rhs is not null
                        && lhs.InvariantName == rhs.InvariantName
                        && lhs.Type == rhs.Type
                        && lhs.TypeMapping == rhs.TypeMapping
                        && lhs.TranslationMode == rhs.TranslationMode),
                x => HashCode.Combine(x.InvariantName, x.Type, x.TypeMapping, x.TranslationMode)));

        private readonly HashSet<QueryParameterExpression> _visitedQueryParameters =
            new(EqualityComparer<QueryParameterExpression>.Create(
                (lhs, rhs) =>
                    ReferenceEquals(lhs, rhs)
                    || (lhs is not null && rhs is not null
                        && lhs.Name == rhs.Name
                        && lhs.TranslationMode == rhs.TranslationMode),
                x => HashCode.Combine(x.Name, x.TranslationMode)));

        /// <inheritdoc/>
        protected override Expression VisitExtension(Expression node)
        {
            switch (node)
            {
                case ValuesExpression { ValuesParameter: { } valuesParameter }:
                    ProcessCollectionParameter(valuesParameter, bucketization: false);
                    break;

                case InExpression { ValuesParameter: { } valuesParameter }:
                    ProcessCollectionParameter(valuesParameter, bucketization: true);
                    break;

                case FromSqlExpression { Arguments: QueryParameterExpression queryParameter }:
                    if (_visitedQueryParameters.Add(queryParameter))
                    {
                        var parameters = parametersDecorator.GetAndDisableCaching();
                        Count += ((object?[])parameters[queryParameter.Name]!).Length;
                    }
                    break;

                case SqlParameterExpression sqlParameterExpression:
                    if (_visitedSqlParameters.Add(sqlParameterExpression))
                    {
                        Count++;
                    }
                    break;
            }

            return base.VisitExtension(node);
        }

        private void ProcessCollectionParameter(SqlParameterExpression sqlParameterExpression, bool bucketization)
        {
            if (!_visitedSqlParameters.Add(sqlParameterExpression))
            {
                return;
            }

            switch (sqlParameterExpression.TranslationMode ?? collectionParameterTranslationMode)
            {
                case ParameterTranslationMode.MultipleParameters:
                    var parameters = parametersDecorator.GetAndDisableCaching();
                    var parameterValue = parameters[sqlParameterExpression.Name];
                    var count = (parameterValue as IEnumerable)?.Cast<object?>().Count() ?? 0;
                    Count += count;

                    if (bucketization)
                    {
                        var elementTypeMapping = (RelationalTypeMapping)sqlParameterExpression.TypeMapping!.ElementTypeMapping!;
                        Count += bucketizationPadding(count, elementTypeMapping);
                    }

                    break;

                case ParameterTranslationMode.Parameter:
                    Count++;
                    break;

                case ParameterTranslationMode.Constant:
                    break;

                default:
                    throw new UnreachableException();
            }
        }
    }
}
