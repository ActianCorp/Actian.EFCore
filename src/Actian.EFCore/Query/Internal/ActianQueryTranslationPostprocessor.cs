// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Diagnostics.CodeAnalysis;
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

using Actian.EFCore.Internal;

#nullable enable

namespace Actian.EFCore.Query.Internal
{
    public class ActianQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly SkipWithoutOrderByInSplitQueryVerifier _skipWithoutOrderByInSplitQueryVerifier = new();

        public ActianQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            ActianQueryCompilationContext queryCompilationContext)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {

        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);

            _skipWithoutOrderByInSplitQueryVerifier.Visit(query);

            return query;
        }

        private sealed class SkipWithoutOrderByInSplitQueryVerifier : ExpressionVisitor
        {
            [return: NotNullIfNotNull("expression")]
            public override Expression? Visit(Expression? expression)
            {
                switch (expression)
                {
                    case ShapedQueryExpression shapedQueryExpression:
                        Visit(shapedQueryExpression.ShaperExpression);
                        return shapedQueryExpression;

                    case RelationalSplitCollectionShaperExpression relationalSplitCollectionShaperExpression:
                        foreach (var table in relationalSplitCollectionShaperExpression.SelectExpression.Tables)
                        {
                            Visit(table);
                        }

                        Visit(relationalSplitCollectionShaperExpression.InnerShaper);

                        return relationalSplitCollectionShaperExpression;

                    case SelectExpression { Offset: not null, Orderings.Count: 0 }:
                        throw new InvalidOperationException(ActianStrings.SplitQueryOffsetWithoutOrderBy);

                    default:
                        return base.Visit(expression);
                }
            }
        }
    }
}
