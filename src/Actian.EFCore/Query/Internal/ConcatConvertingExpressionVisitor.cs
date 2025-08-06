// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Actian.EFCore.Query.Internal
{
    public class ConcatConvertingExpressionVisitor : SqlExpressionVisitor
    {
        private readonly ISqlExpressionFactory Factory;

        public ConcatConvertingExpressionVisitor(ISqlExpressionFactory factory)
        {
            Factory = factory;
        }

        protected override Expression VisitAtTimeZone(AtTimeZoneExpression atTimeZoneExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitCase(CaseExpression caseExpression) => caseExpression;

        protected override Expression VisitCollate([NotNull] CollateExpression collateExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitColumn(ColumnExpression columnExpression) => columnExpression;
        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression) => crossApplyExpression;
        protected override Expression VisitCrossJoin(CrossJoinExpression crossJoinExpression) => crossJoinExpression;
        protected override Expression VisitDelete(DeleteExpression deleteExpression) => deleteExpression;
        protected override Expression VisitDistinct([NotNull] DistinctExpression distinctExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitExcept(ExceptExpression exceptExpression) => exceptExpression;
        protected override Expression VisitExists(ExistsExpression existsExpression) => existsExpression;
        protected override Expression VisitFromSql(FromSqlExpression fromSqlExpression) => fromSqlExpression;
        protected override Expression VisitIn(InExpression inExpression) => inExpression;
        protected override Expression VisitInnerJoin(InnerJoinExpression innerJoinExpression) => innerJoinExpression;
        protected override Expression VisitIntersect(IntersectExpression intersectExpression) => intersectExpression;
        protected override Expression VisitJsonScalar(JsonScalarExpression jsonScalarExpression) => jsonScalarExpression;
        protected override Expression VisitLeftJoin(LeftJoinExpression leftJoinExpression) => leftJoinExpression;
        protected override Expression VisitLike(LikeExpression likeExpression) => likeExpression;
        protected override Expression VisitOrdering(OrderingExpression orderingExpression) => orderingExpression;
        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression) => outerApplyExpression;
        protected override Expression VisitProjection(ProjectionExpression projectionExpression) => projectionExpression;
        protected override Expression VisitRowNumber(RowNumberExpression rowNumberExpression) => rowNumberExpression;
        protected override Expression VisitRowValue(RowValueExpression rowValueExpression) => rowValueExpression;

        protected override Expression VisitScalarSubquery([NotNull] ScalarSubqueryExpression scalarSubqueryExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitSelect(SelectExpression selectExpression) => selectExpression;
        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression) => sqlBinaryExpression;
        protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression) => sqlConstantExpression;
        protected override Expression VisitSqlFragment(SqlFragmentExpression sqlFragmentExpression) => sqlFragmentExpression;
        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression) => sqlFunctionExpression;
        protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression) => sqlParameterExpression;

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlCastExpression) => sqlCastExpression.Update(
            Visit(sqlCastExpression.Operand)
        );

        protected Expression VisitSubSelect(ScalarSubqueryExpression scalarSubqueryExpression) => scalarSubqueryExpression.Update(
            Visit(scalarSubqueryExpression.Subquery)
        );

        protected override Expression VisitTable(TableExpression tableExpression)
            => tableExpression;

        protected override Expression VisitTableValuedFunction([NotNull] TableValuedFunctionExpression tableValuedFunctionExpression)
        {
            throw new System.NotImplementedException();
        }

        protected override Expression VisitUpdate(UpdateExpression updateExpression) => updateExpression;

        protected override Expression VisitUnion(UnionExpression unionExpression) => unionExpression.Update(
            Visit(unionExpression.Source1),
            Visit(unionExpression.Source2)
        );

        protected override Expression VisitValues(ValuesExpression valuesExpression) => valuesExpression;

        private TExpression Visit<TExpression>(TExpression expression)
            where TExpression : Expression
        {
            return (TExpression)base.Visit(expression);
        }
    }
}
