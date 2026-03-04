// Copyright (c) 2026 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

#nullable enable

namespace Actian.EFCore.Query.Internal
{
    /// <summary>
    ///     Converts boolean expressions between search-condition (WHERE-style) and scalar (SELECT-style) contexts
    ///     for the Ingres database, which supports TRUE/FALSE literals but not bitwise boolean operations.
    /// </summary>
    public class SearchConditionConverter(
        ISqlExpressionFactory sqlExpressionFactory,
        IRelationalTypeMappingSource typeMappingSource) : ExpressionVisitor
    {
        [return: NotNullIfNotNull(nameof(expression))]
        public override Expression? Visit(Expression? expression)
            => Visit(expression, inSearchConditionContext: false, allowNullFalseEquivalence: false);

        [return: NotNullIfNotNull(nameof(expression))]
        protected virtual Expression? Visit(Expression? expression, bool inSearchConditionContext, bool allowNullFalseEquivalence)
            => expression switch
            {
                CaseExpression e => VisitCase(e, inSearchConditionContext, allowNullFalseEquivalence),
                SelectExpression e => VisitSelect(e),
                SqlBinaryExpression e => VisitSqlBinary(e, inSearchConditionContext, allowNullFalseEquivalence),
                SqlUnaryExpression e => VisitSqlUnary(e, inSearchConditionContext),
                PredicateJoinExpressionBase e => VisitPredicateJoin(e),

                // Expressions that are inherently search conditions
                SqlExpression e and (ExistsExpression or InExpression or LikeExpression) =>
                    ApplyConversion((SqlExpression)base.VisitExtension(e), inSearchConditionContext, isExpressionSearchCondition: true),

                SqlExpression e =>
                    ApplyConversion((SqlExpression)base.VisitExtension(e), inSearchConditionContext, isExpressionSearchCondition: false),

                _ => base.Visit(expression)
            };

        private SqlExpression ApplyConversion(SqlExpression sqlExpression, bool inSearchConditionContext, bool isExpressionSearchCondition)
            => (inSearchCondition: inSearchConditionContext, isExpressionSearchCondition) switch
            {
                // Convert scalar boolean into search condition (WHERE context)
                (true, false) => sqlExpression is SqlConstantExpression { Value: bool boolValue }
                    ? sqlExpressionFactory.Equal(
                        sqlExpressionFactory.Constant(boolValue),
                        sqlExpressionFactory.Constant(true))
                    : sqlExpressionFactory.Equal(sqlExpression, sqlExpressionFactory.Constant(true)),

                // Convert search condition (e.g. a LIKE) into scalar boolean (SELECT context)
                (false, true) => CreateNonNullableBooleanCase(SimplifyNegatedBinary(sqlExpression)),

                // All other cases — no conversion required
                _ => sqlExpression
            };

        private SqlExpression CreateNonNullableBooleanCase(SqlExpression condition)
        {
            // Get non-nullable bool type mapping explicitly
            var nonNullableBoolTypeMapping = typeMappingSource.FindMapping(typeof(bool));
            
            // Create constants with non-nullable type mapping
            var trueConstant = sqlExpressionFactory.Constant(true, nonNullableBoolTypeMapping);
            var falseConstant = sqlExpressionFactory.Constant(false, nonNullableBoolTypeMapping);
            
            // Create CASE expression - it should inherit the non-nullable type mapping from the constants
            var caseExpression = sqlExpressionFactory.Case(
                new[]
                {
                    new CaseWhenClause(condition, trueConstant)
                },
                falseConstant);

            // Ensure the CASE expression has non-nullable type mapping
            // If it somehow got nullable type mapping, explicitly apply non-nullable one
            if (caseExpression.TypeMapping is RelationalTypeMapping typeMapping 
                && typeMapping.ClrType == typeof(bool?))
            {
                return sqlExpressionFactory.ApplyTypeMapping(caseExpression, nonNullableBoolTypeMapping);
            }

            return caseExpression;
        }

        private SqlExpression SimplifyNegatedBinary(SqlExpression sqlExpression)
        {
            if (sqlExpression is SqlUnaryExpression { OperatorType: ExpressionType.Not } notExpr
                && notExpr.Operand is SqlBinaryExpression
                {
                    OperatorType: ExpressionType.Equal
                } eqExpr)
            {
                // NOT (a = b) → a <> b
                return sqlExpressionFactory.MakeBinary(
                    ExpressionType.NotEqual,
                    eqExpr.Left,
                    eqExpr.Right,
                    eqExpr.TypeMapping)!;
            }

            return sqlExpression;
        }

        protected virtual Expression VisitCase(CaseExpression caseExpression, bool inSearchConditionContext, bool allowNullFalseEquivalence)
        {
            var testIsCondition = caseExpression.Operand is null;
            var operand = (SqlExpression?)Visit(caseExpression.Operand);
            var whenClauses = new List<CaseWhenClause>();

            foreach (var whenClause in caseExpression.WhenClauses)
            {
                var test = (SqlExpression)Visit(whenClause.Test, testIsCondition, testIsCondition);
                var result = (SqlExpression)Visit(whenClause.Result, inSearchConditionContext: false, allowNullFalseEquivalence);
                whenClauses.Add(new CaseWhenClause(test, result));
            }

            var elseResult = (SqlExpression?)Visit(caseExpression.ElseResult, inSearchConditionContext: false, allowNullFalseEquivalence);

            return ApplyConversion(
                sqlExpressionFactory.Case(operand, whenClauses, elseResult, caseExpression),
                inSearchConditionContext,
                isExpressionSearchCondition: false);
        }

        protected virtual Expression VisitPredicateJoin(PredicateJoinExpressionBase join)
            => join.Update(
                (TableExpressionBase)Visit(join.Table),
                (SqlExpression)Visit(join.JoinPredicate, inSearchConditionContext: true, allowNullFalseEquivalence: true));

        protected virtual Expression VisitSelect(SelectExpression select)
        {
            var tables = this.VisitAndConvert(select.Tables);
            var predicate = (SqlExpression?)Visit(select.Predicate, inSearchConditionContext: true, allowNullFalseEquivalence: true);
            var groupBy = this.VisitAndConvert(select.GroupBy);
            var havingExpression = (SqlExpression?)Visit(select.Having, inSearchConditionContext: true, allowNullFalseEquivalence: true);
            var projections = this.VisitAndConvert(select.Projection);
            var orderings = this.VisitAndConvert(select.Orderings);
            var offset = (SqlExpression?)Visit(select.Offset);
            var limit = (SqlExpression?)Visit(select.Limit);

            return select.Update(tables, predicate, groupBy, havingExpression, projections, orderings, offset, limit);
        }
        protected virtual Expression VisitSqlBinary(SqlBinaryExpression binary, bool inSearchConditionContext, bool allowNullFalseEquivalence)
        {
            // Only logical operations need conditions on both sides
            var areOperandsInSearchConditionContext = binary.OperatorType is ExpressionType.AndAlso or ExpressionType.OrElse;

            var newLeft = (SqlExpression)Visit(binary.Left, areOperandsInSearchConditionContext, allowNullFalseEquivalence: false);
            var newRight = (SqlExpression)Visit(binary.Right, areOperandsInSearchConditionContext, allowNullFalseEquivalence: false);

            if (binary.OperatorType is ExpressionType.NotEqual or ExpressionType.Equal)
            {
                var leftType = newLeft.TypeMapping?.Converter?.ProviderClrType ?? newLeft.Type;
                var rightType = newRight.TypeMapping?.Converter?.ProviderClrType ?? newRight.Type;
                if (!inSearchConditionContext && !allowNullFalseEquivalence
                    && (leftType == typeof(bool) || leftType.IsInteger())
                    && (rightType == typeof(bool) || rightType.IsInteger()))
                {
                    // "lhs != rhs" is the same as "CAST(lhs ^ rhs AS BIT)", except that
                    // the first is a boolean, the second is a BIT
                    var result = sqlExpressionFactory.MakeBinary(ExpressionType.ExclusiveOr, newLeft, newRight, null)!;

                    if (result.Type != typeof(bool))
                    {
                        result = sqlExpressionFactory.Convert(result, typeof(bool), binary.TypeMapping);
                    }

                    // "lhs == rhs" is the same as "NOT(lhs != rhs)" aka "~(lhs ^ rhs)"
                    if (binary.OperatorType is ExpressionType.Equal)
                    {
                        result = sqlExpressionFactory.MakeUnary(ExpressionType.OnesComplement, result, result.Type, result.TypeMapping)!;
                    }

                    return result;
                }

                if (newLeft is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } negatedLeft
                    && newRight is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } negatedRight)
                {
                    newLeft = negatedLeft.Operand;
                    newRight = negatedRight.Operand;
                }
            }

            binary = binary.Update(newLeft, newRight);

            var isExpressionSearchCondition = binary.OperatorType is ExpressionType.AndAlso
                or ExpressionType.OrElse
                or ExpressionType.Equal
                or ExpressionType.NotEqual
                or ExpressionType.GreaterThan
                or ExpressionType.GreaterThanOrEqual
                or ExpressionType.LessThan
                or ExpressionType.LessThanOrEqual;

            return ApplyConversion(binary, inSearchConditionContext, isExpressionSearchCondition);
        }

        protected virtual Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression, bool inSearchConditionContext)
        {
            bool isOperandInSearchConditionContext, isSearchConditionExpression;

            switch (sqlUnaryExpression.OperatorType)
            {
                case ExpressionType.Not
                    when (sqlUnaryExpression.TypeMapping?.Converter?.ProviderClrType ?? sqlUnaryExpression.Type) == typeof(bool):
                {
                    // when possible, avoid converting to/from predicate form
                    if (!inSearchConditionContext && sqlUnaryExpression.Operand is not (ExistsExpression or InExpression or LikeExpression))
                    {
                        var negatedOperand = (SqlExpression)Visit(sqlUnaryExpression.Operand);

                        if (negatedOperand is SqlUnaryExpression { OperatorType: ExpressionType.OnesComplement } unary)
                        {
                            return unary.Operand;
                        }

                        return sqlExpressionFactory.MakeUnary(
                            ExpressionType.OnesComplement, negatedOperand, negatedOperand.Type, negatedOperand.TypeMapping)!;
                    }

                    isOperandInSearchConditionContext = true;
                    isSearchConditionExpression = true;
                    break;
                }

                case ExpressionType.Not:
                    isOperandInSearchConditionContext = false;
                    isSearchConditionExpression = false;
                    break;

                case ExpressionType.Convert:
                case ExpressionType.Negate:
                    isOperandInSearchConditionContext = false;
                    isSearchConditionExpression = false;
                    break;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    isOperandInSearchConditionContext = false;
                    isSearchConditionExpression = true;
                    break;

                default:
                    throw new InvalidOperationException(
                        RelationalStrings.UnsupportedOperatorForSqlExpression(
                            sqlUnaryExpression.OperatorType, typeof(SqlUnaryExpression)));
            }

            var operand = (SqlExpression)Visit(sqlUnaryExpression.Operand, isOperandInSearchConditionContext, allowNullFalseEquivalence: false);

            return SimplifyNegatedBinary(
                ApplyConversion(
                    sqlUnaryExpression.Update(operand),
                    inSearchConditionContext,
                    isSearchConditionExpression));
        }
    }
}



