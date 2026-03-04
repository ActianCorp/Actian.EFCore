// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

#nullable enable
namespace Actian.EFCore.Query.Internal
{
    public class SearchConditionConvertingExpressionVisitor : SqlExpressionVisitor
    {
        private bool _isSearchCondition;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public SearchConditionConvertingExpressionVisitor(ISqlExpressionFactory sqlExpressionFactory)
            => _sqlExpressionFactory = sqlExpressionFactory;

        private SqlExpression ApplyConversion(SqlExpression sqlExpression, bool condition)
            => _isSearchCondition
                ? ConvertToSearchCondition(sqlExpression, condition)
                : ConvertToValue(sqlExpression, condition);

        private SqlExpression ConvertToSearchCondition(SqlExpression sqlExpression, bool condition)
        {
            if (!condition)
                return BuildCompareToExpression(sqlExpression);

            var result = _sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression);
            return result ?? sqlExpression; // EFCore 10: handle null result
        }

        private SqlExpression ConvertToValue(SqlExpression sqlExpression, bool condition)
        {
            if (!condition)
            {
                var result = _sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression);
                return result ?? sqlExpression; // EFCore 10: handle null result
            }

            // For CASE expressions, ensure boolean constants have type mappings
            var trueConstant = _sqlExpressionFactory.Constant(true);
            var mappedTrue = _sqlExpressionFactory.ApplyDefaultTypeMapping(trueConstant);
            if (mappedTrue == null)
                mappedTrue = trueConstant; // Fallback to original

            var falseConstant = _sqlExpressionFactory.Constant(false);
            var mappedFalse = _sqlExpressionFactory.ApplyDefaultTypeMapping(falseConstant);
            if (mappedFalse == null)
                mappedFalse = falseConstant; // Fallback to original

            return _sqlExpressionFactory.Case(
                new[]
                {
            new CaseWhenClause(SimplifyNegatedBinary(sqlExpression), mappedTrue)
                },
                mappedFalse);
        }

        private SqlExpression BuildCompareToExpression(SqlExpression sqlExpression)
        {
            if (sqlExpression is SqlConstantExpression { Value: bool boolValue })
            {
                var leftConst = _sqlExpressionFactory.Constant(boolValue ? 1 : 0);
                var leftMapped = _sqlExpressionFactory.ApplyDefaultTypeMapping(leftConst) ?? leftConst;

                var rightConst = _sqlExpressionFactory.Constant(1);
                var rightMapped = _sqlExpressionFactory.ApplyDefaultTypeMapping(rightConst) ?? rightConst;

                return _sqlExpressionFactory.Equal(leftMapped, rightMapped);
            }

            var sqlMapped = _sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression) ?? sqlExpression;
            var trueConst = _sqlExpressionFactory.Constant(true);
            var trueMapped = _sqlExpressionFactory.ApplyDefaultTypeMapping(trueConst) ?? trueConst;

            return _sqlExpressionFactory.Equal(sqlMapped, trueMapped);
        }

        private SqlExpression SimplifyNegatedBinary(SqlExpression sqlExpression)
        {
            if (sqlExpression is SqlUnaryExpression { OperatorType: ExpressionType.Not } sqlUnaryExpression
                && sqlUnaryExpression.Type == typeof(bool)
                && sqlUnaryExpression.Operand is SqlBinaryExpression { OperatorType: ExpressionType.Equal } sqlBinaryOperand)
            {
                if (sqlBinaryOperand.Left.Type == typeof(bool)
                    && sqlBinaryOperand.Right.Type == typeof(bool)
                    && (sqlBinaryOperand.Left is SqlConstantExpression || sqlBinaryOperand.Right is SqlConstantExpression))
                {
                    var constant = sqlBinaryOperand.Left as SqlConstantExpression ?? (SqlConstantExpression)sqlBinaryOperand.Right;

                    if (sqlBinaryOperand.Left is SqlConstantExpression)
                    {
                        return _sqlExpressionFactory.MakeBinary(
                            ExpressionType.Equal,
                            _sqlExpressionFactory.ApplyDefaultTypeMapping(_sqlExpressionFactory.Constant(!(bool)constant.Value!)),
                            sqlBinaryOperand.Right,
                            sqlBinaryOperand.TypeMapping)!;
                    }

                    return _sqlExpressionFactory.MakeBinary(
                        ExpressionType.Equal,
                        sqlBinaryOperand.Left,
                        _sqlExpressionFactory.ApplyDefaultTypeMapping(_sqlExpressionFactory.Constant(!(bool)constant.Value!)),
                        sqlBinaryOperand.TypeMapping)!;
                }

                return _sqlExpressionFactory.MakeBinary(
                    sqlBinaryOperand.OperatorType == ExpressionType.Equal
                        ? ExpressionType.NotEqual
                        : ExpressionType.Equal,
                    sqlBinaryOperand.Left,
                    sqlBinaryOperand.Right,
                    sqlBinaryOperand.TypeMapping)!;
            }

            return sqlExpression;
        }

        protected override Expression VisitCase(CaseExpression caseExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;

            // Operand
            var operand = caseExpression.Operand is not null
                ? Visit(caseExpression.Operand) as SqlExpression ?? caseExpression.Operand
                : null;

            if (operand is not null)
                operand = _sqlExpressionFactory.ApplyDefaultTypeMapping(operand);

            // WHEN clauses
            var whenClauses = new List<CaseWhenClause>();
            foreach (var whenClause in caseExpression.WhenClauses)
            {
                _isSearchCondition = true;
                var test = Visit(whenClause.Test) as SqlExpression ?? whenClause.Test;

                _isSearchCondition = false;
                var result = Visit(whenClause.Result) as SqlExpression ?? whenClause.Result;

                if (test is not null && result is not null)
                {
                    whenClauses.Add(new CaseWhenClause(test, result));
                }
            }

            // ELSE clause
            var elseResult = Visit(caseExpression.ElseResult) as SqlExpression ?? caseExpression.ElseResult;

            _isSearchCondition = parentSearchCondition;

            // Return a CaseExpression where QuerySqlGenerator always sees SqlExpressions
            return _sqlExpressionFactory.Case(operand, whenClauses, elseResult, caseExpression);
        }

        protected override Expression VisitCollate(CollateExpression collateExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var operand = (SqlExpression)Visit(collateExpression.Operand);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(collateExpression.Update(operand), false);
        }

        protected override Expression VisitColumn(ColumnExpression columnExpression)
            => ApplyConversion(columnExpression, false);

        protected override Expression VisitDelete(DeleteExpression deleteExpression)
            => deleteExpression.Update(deleteExpression.Table, (SelectExpression)Visit(deleteExpression.SelectExpression));

        protected override Expression VisitDistinct(DistinctExpression distinctExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var operand = (SqlExpression)Visit(distinctExpression.Operand);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(distinctExpression.Update(operand), false);
        }

        protected override Expression VisitExists(ExistsExpression existsExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var subquery = (SelectExpression)Visit(existsExpression.Subquery);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(existsExpression.Update(subquery), true);
        }

        protected override Expression VisitFromSql(FromSqlExpression fromSqlExpression)
            => fromSqlExpression;

        protected override Expression VisitIn(InExpression inExpression)
        {
            var parentSearchCondition = _isSearchCondition;

            _isSearchCondition = false;
            var item = (SqlExpression)Visit(inExpression.Item);
            var subquery = (SelectExpression?)Visit(inExpression.Subquery);

            var values = inExpression.Values;
            SqlExpression[]? newValues = null;
            if (values is not null)
            {
                for (var i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var newValue = (SqlExpression)Visit(value);

                    if (newValue != value && newValues is null)
                    {
                        newValues = new SqlExpression[values.Count];
                        for (var j = 0; j < i; j++)
                        {
                            newValues[j] = values[j];
                        }
                    }

                    if (newValues is not null)
                    {
                        newValues[i] = newValue;
                    }
                }
            }

            var valuesParameter = (SqlParameterExpression?)Visit(inExpression.ValuesParameter);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(inExpression.Update(item, subquery, newValues ?? values, valuesParameter), true);
        }

        protected override Expression VisitLike(LikeExpression likeExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var match = (SqlExpression)Visit(likeExpression.Match);
            var pattern = (SqlExpression)Visit(likeExpression.Pattern);
            var escapeChar = (SqlExpression?)Visit(likeExpression.EscapeChar);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(likeExpression.Update(match, pattern, escapeChar), true);
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            var parentSearchCondition = _isSearchCondition;

            _isSearchCondition = false;
            var projections = this.VisitAndConvert(selectExpression.Projection);
            var tables = this.VisitAndConvert(selectExpression.Tables);
            var groupBy = this.VisitAndConvert(selectExpression.GroupBy);
            var orderings = this.VisitAndConvert(selectExpression.Orderings);
            var offset = (SqlExpression?)Visit(selectExpression.Offset);
            var limit = (SqlExpression?)Visit(selectExpression.Limit);

            _isSearchCondition = true;
            var predicate = (SqlExpression?)Visit(selectExpression.Predicate);
            var havingExpression = (SqlExpression?)Visit(selectExpression.Having);

            _isSearchCondition = parentSearchCondition;

            return selectExpression.Update(tables, predicate, groupBy, havingExpression, projections, orderings, offset, limit);
        }

        protected override Expression VisitAtTimeZone(AtTimeZoneExpression atTimeZoneExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var operand = (SqlExpression)Visit(atTimeZoneExpression.Operand);
            var timeZone = (SqlExpression)Visit(atTimeZoneExpression.TimeZone);
            _isSearchCondition = parentSearchCondition;

            return atTimeZoneExpression.Update(operand, timeZone);
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            var parentIsSearchCondition = _isSearchCondition;

            switch (sqlBinaryExpression.OperatorType)
            {
                // Only logical operations need conditions on both sides
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    _isSearchCondition = true;
                    break;
                default:
                    _isSearchCondition = false;
                    break;
            }

            var newLeft = (SqlExpression)Visit(sqlBinaryExpression.Left);
            var newRight = (SqlExpression)Visit(sqlBinaryExpression.Right);

            _isSearchCondition = parentIsSearchCondition;

            if (!parentIsSearchCondition
                && (newLeft.Type == typeof(bool) || newLeft.Type.IsEnum || newLeft.Type.IsInteger())
                && (newRight.Type == typeof(bool) || newRight.Type.IsEnum || newRight.Type.IsInteger())
                && sqlBinaryExpression.OperatorType is ExpressionType.NotEqual or ExpressionType.Equal)
            {
                // "lhs != rhs" is the same as "CAST(lhs ^ rhs AS BIT)", except that
                // the first is a boolean, the second is a BIT
                var result = _sqlExpressionFactory.MakeBinary(
                    ExpressionType.ExclusiveOr,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(newLeft),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(newRight),
                    null)!;

                // "lhs == rhs" is the same as "NOT(lhs != rhs)" aka "~(lhs ^ rhs)"
                if (sqlBinaryExpression.OperatorType is ExpressionType.Equal)
                {
                    result = _sqlExpressionFactory.MakeUnary(
                        ExpressionType.OnesComplement,
                        result,
                        result.Type,
                        result.TypeMapping
                    )!;
                }

                return result;
            }

            if (sqlBinaryExpression.OperatorType is ExpressionType.NotEqual or ExpressionType.Equal
                && newLeft is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } negatedLeft
                && newRight is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } negatedRight)
            {
                newLeft = negatedLeft.Operand;
                newRight = negatedRight.Operand;
            }

            sqlBinaryExpression = sqlBinaryExpression.Update(newLeft, newRight);
            var condition = sqlBinaryExpression.OperatorType is ExpressionType.AndAlso
                or ExpressionType.OrElse
                or ExpressionType.Equal
                or ExpressionType.NotEqual
                or ExpressionType.GreaterThan
                or ExpressionType.GreaterThanOrEqual
                or ExpressionType.LessThan
                or ExpressionType.LessThanOrEqual;

            return ApplyConversion(sqlBinaryExpression, condition);
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            bool resultCondition;

            switch (sqlUnaryExpression.OperatorType)
            {
                case ExpressionType.Not when sqlUnaryExpression.Type == typeof(bool):
                    if (!_isSearchCondition && sqlUnaryExpression.Operand is not (ExistsExpression or InExpression or LikeExpression))
                    {
                        var negatedOperand = (SqlExpression)Visit(sqlUnaryExpression.Operand);
                        negatedOperand = _sqlExpressionFactory.ApplyDefaultTypeMapping(negatedOperand);

                        if (negatedOperand is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } unary)
                        {
                            return unary.Operand;
                        }

                        return _sqlExpressionFactory.MakeUnary(
                            ExpressionType.OnesComplement,
                            negatedOperand,
                            negatedOperand.Type,
                            negatedOperand.TypeMapping
                        )!;
                    }

                    _isSearchCondition = true;
                    resultCondition = true;
                    break;

                case ExpressionType.Not:
                    _isSearchCondition = false;
                    resultCondition = false;
                    break;

                case ExpressionType.Convert:
                case ExpressionType.Negate:
                    _isSearchCondition = false;
                    resultCondition = false;
                    break;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    _isSearchCondition = false;
                    resultCondition = true;
                    break;

                default:
                    throw new InvalidOperationException(
                        RelationalStrings.UnsupportedOperatorForSqlExpression(
                            sqlUnaryExpression.OperatorType, typeof(SqlUnaryExpression)));
            }

            var operand = (SqlExpression)Visit(sqlUnaryExpression.Operand);
            _isSearchCondition = parentSearchCondition;

            return SimplifyNegatedBinary(ApplyConversion(sqlUnaryExpression.Update(operand), resultCondition));
        }

        protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression)
        {
            if (sqlConstantExpression is null)
                return null!;

            var withTypeMapping = _sqlExpressionFactory.ApplyDefaultTypeMapping(sqlConstantExpression);

            // EFCore 10 can return null from ApplyDefaultTypeMapping
            // If null, keep the original expression instead of losing type mapping
            if (withTypeMapping is null)
            {
                // Don't call ApplyConversion with null - just return original
                return sqlConstantExpression;
            }

            return ApplyConversion((SqlConstantExpression)withTypeMapping, false);
        }


        protected override Expression VisitSqlFragment(SqlFragmentExpression sqlFragmentExpression)
            => sqlFragmentExpression;

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var instance = (SqlExpression?)Visit(sqlFunctionExpression.Instance);
            SqlExpression[]? arguments = default;
            if (!sqlFunctionExpression.IsNiladic)
            {
                arguments = new SqlExpression[sqlFunctionExpression.Arguments.Count];
                for (var i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = (SqlExpression)Visit(sqlFunctionExpression.Arguments[i]);
                }
            }

            _isSearchCondition = parentSearchCondition;
            var newFunction = sqlFunctionExpression.Update(instance, arguments);
            var condition = sqlFunctionExpression.Name is "FREETEXT" or "CONTAINS";
            return ApplyConversion(newFunction, condition);
        }

        protected override Expression VisitTableValuedFunction(TableValuedFunctionExpression tableValuedFunctionExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;

            var arguments = new SqlExpression[tableValuedFunctionExpression.Arguments.Count];
            for (var i = 0; i < arguments.Length; i++)
            {
                arguments[i] = (SqlExpression)Visit(tableValuedFunctionExpression.Arguments[i]);
            }

            _isSearchCondition = parentSearchCondition;
            return tableValuedFunctionExpression.Update(arguments);
        }

        protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
            => ApplyConversion(sqlParameterExpression, false);

        protected override Expression VisitTable(TableExpression tableExpression)
            => tableExpression;
        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            if (projectionExpression.Expression is null)
            {
                return projectionExpression;
            }

            var visited = Visit(projectionExpression.Expression);

            // Ensure we always return a SqlExpression
            var sqlExpression = visited as SqlExpression ?? projectionExpression.Expression;

            // EFCore 10: ApplyDefaultTypeMapping can return null
            var withTypeMapping = _sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression);

            // If null, keep the original expression with its existing type mapping
            if (withTypeMapping is not null)
            {
                sqlExpression = withTypeMapping;
            }

            return projectionExpression.Update(sqlExpression);
        }

        protected override Expression VisitOrdering(OrderingExpression orderingExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var expression = (SqlExpression)Visit(orderingExpression.Expression);
            _isSearchCondition = parentSearchCondition;
            return orderingExpression.Update(expression);
        }

        protected override Expression VisitCrossJoin(CrossJoinExpression crossJoinExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(crossJoinExpression.Table);
            _isSearchCondition = parentSearchCondition;

            return crossJoinExpression.Update(table);
        }

        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(crossApplyExpression.Table);
            _isSearchCondition = parentSearchCondition;

            return crossApplyExpression.Update(table);
        }

        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(outerApplyExpression.Table);
            _isSearchCondition = parentSearchCondition;

            return outerApplyExpression.Update(table);
        }

        protected override Expression VisitInnerJoin(InnerJoinExpression innerJoinExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(innerJoinExpression.Table);
            _isSearchCondition = true;
            var joinPredicate = (SqlExpression)Visit(innerJoinExpression.JoinPredicate);
            _isSearchCondition = parentSearchCondition;

            return innerJoinExpression.Update(table, joinPredicate);
        }

        protected override Expression VisitLeftJoin(LeftJoinExpression leftJoinExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(leftJoinExpression.Table);
            _isSearchCondition = true;
            var joinPredicate = (SqlExpression)Visit(leftJoinExpression.JoinPredicate);
            _isSearchCondition = parentSearchCondition;

            return leftJoinExpression.Update(table, joinPredicate);
        }

        protected override Expression VisitRightJoin(RightJoinExpression rightJoinExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var table = (TableExpressionBase)Visit(rightJoinExpression.Table);
            _isSearchCondition = true;
            var joinPredicate = (SqlExpression)Visit(rightJoinExpression.JoinPredicate);
            _isSearchCondition = parentSearchCondition;

            return rightJoinExpression.Update(table, joinPredicate);
        }

        protected override Expression VisitScalarSubquery(ScalarSubqueryExpression scalarSubqueryExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            var subquery = (SelectExpression)Visit(scalarSubqueryExpression.Subquery);
            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(scalarSubqueryExpression.Update(subquery), condition: false);
        }

        protected override Expression VisitRowNumber(RowNumberExpression rowNumberExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var partitions = new List<SqlExpression>();
            foreach (var partition in rowNumberExpression.Partitions)
            {
                var newPartition = (SqlExpression)Visit(partition);
                partitions.Add(newPartition);
            }

            var orderings = new List<OrderingExpression>();
            foreach (var ordering in rowNumberExpression.Orderings)
            {
                var newOrdering = (OrderingExpression)Visit(ordering);
                orderings.Add(newOrdering);
            }

            _isSearchCondition = parentSearchCondition;

            return ApplyConversion(rowNumberExpression.Update(partitions, orderings), condition: false);
        }

        protected override Expression VisitRowValue(RowValueExpression rowValueExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;

            var values = new SqlExpression[rowValueExpression.Values.Count];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = (SqlExpression)Visit(rowValueExpression.Values[i]);
            }

            _isSearchCondition = parentSearchCondition;
            return rowValueExpression.Update(values);
        }

        protected override Expression VisitExcept(ExceptExpression exceptExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var source1 = (SelectExpression)Visit(exceptExpression.Source1);
            var source2 = (SelectExpression)Visit(exceptExpression.Source2);
            _isSearchCondition = parentSearchCondition;

            return exceptExpression.Update(source1, source2);
        }

        protected override Expression VisitIntersect(IntersectExpression intersectExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var source1 = (SelectExpression)Visit(intersectExpression.Source1);
            var source2 = (SelectExpression)Visit(intersectExpression.Source2);
            _isSearchCondition = parentSearchCondition;

            return intersectExpression.Update(source1, source2);
        }

        protected override Expression VisitUnion(UnionExpression unionExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            var source1 = (SelectExpression)Visit(unionExpression.Source1);
            var source2 = (SelectExpression)Visit(unionExpression.Source2);
            _isSearchCondition = parentSearchCondition;

            return unionExpression.Update(source1, source2);
        }

        protected override Expression VisitUpdate(UpdateExpression updateExpression)
        {
            var selectExpression = (SelectExpression)Visit(updateExpression.SelectExpression);
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            List<ColumnValueSetter>? columnValueSetters = null;
            for (var (i, n) = (0, updateExpression.ColumnValueSetters.Count); i < n; i++)
            {
                var columnValueSetter = updateExpression.ColumnValueSetters[i];
                var newValue = (SqlExpression)Visit(columnValueSetter.Value);
                if (columnValueSetters != null)
                {
                    columnValueSetters.Add(new ColumnValueSetter(columnValueSetter.Column, newValue));
                }
                else if (!ReferenceEquals(newValue, columnValueSetter.Value))
                {
                    columnValueSetters = [];
                    for (var j = 0; j < i; j++)
                    {
                        columnValueSetters.Add(updateExpression.ColumnValueSetters[j]);
                    }

                    columnValueSetters.Add(new ColumnValueSetter(columnValueSetter.Column, newValue));
                }
            }

            _isSearchCondition = parentSearchCondition;
            return updateExpression.Update(selectExpression, columnValueSetters ?? updateExpression.ColumnValueSetters);
        }

        protected override Expression VisitJsonScalar(JsonScalarExpression jsonScalarExpression)
            => ApplyConversion(jsonScalarExpression, condition: false);

        protected override Expression VisitValues(ValuesExpression valuesExpression)
        {
            var parentSearchCondition = _isSearchCondition;
            _isSearchCondition = false;
            switch (valuesExpression)
            {
                case { RowValues: not null }:
                    var rowValues = new RowValueExpression[valuesExpression.RowValues!.Count];
                    for (var i = 0; i < rowValues.Length; i++)
                    {
                        rowValues[i] = (RowValueExpression)Visit(valuesExpression.RowValues[i]);
                    }

                    _isSearchCondition = parentSearchCondition;
                    return valuesExpression.Update(rowValues);

                case { ValuesParameter: not null }:
                    var valuesParameter = (SqlParameterExpression)Visit(valuesExpression.ValuesParameter);
                    _isSearchCondition = parentSearchCondition;
                    return valuesExpression.Update(valuesParameter);

                default:
                    throw new UnreachableException();
            }
        }
    }
}
