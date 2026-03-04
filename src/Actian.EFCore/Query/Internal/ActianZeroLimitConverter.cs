// Copyright (c) 2026 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Actian.EFCore.Query.Internal
{
    public class ActianZeroLimitConverter : ExpressionVisitor
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private ParametersCacheDecorator _parametersDecorator;

        public ActianZeroLimitConverter(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _parametersDecorator = null!;
        }

        public virtual Expression Process(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
        {
            _parametersDecorator = parametersDecorator;

            return Visit(queryExpression);
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            // SQL Server doesn't support 0 in the FETCH NEXT x ROWS ONLY clause. We use this clause when translating LINQ Take(), but
            // only if there's also a Skip(), otherwise we translate to SQL TOP(x), which does allow 0.
            // Check for this case, and replace with a false predicate (since no rows should be returned).
            if (extensionExpression is SelectExpression { Offset: not null, Limit: not null } selectExpression)
            {
                if (IsZero(selectExpression.Limit))
                {
                    return selectExpression.Update(
                        selectExpression.Tables,
                        selectExpression.GroupBy.Count > 0 ? selectExpression.Predicate : _sqlExpressionFactory.Constant(false),
                        selectExpression.GroupBy,
                        selectExpression.GroupBy.Count > 0 ? _sqlExpressionFactory.Constant(false) : null,
                        selectExpression.Projection,
                        orderings: [],
                        offset: null,
                        limit: null);
                }

                bool IsZero(SqlExpression sqlExpression)
                    => sqlExpression switch
                    {
                        SqlConstantExpression { Value: int i } => i == 0,
                        SqlParameterExpression p => _parametersDecorator.GetAndDisableCaching()[p.Name] is 0,
                        _ => false
                    };
            }

            return base.VisitExtension(extensionExpression);
        }
    }
}
