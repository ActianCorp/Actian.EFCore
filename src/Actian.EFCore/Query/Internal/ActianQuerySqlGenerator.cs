// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq.Expressions;
using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Query.Internal
{
    public class ActianQuerySqlGenerator : QuerySqlGenerator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly int _actianCompatibilityLevel;

        public ActianQuerySqlGenerator(
            QuerySqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource,
            IActianSingletonOptions actianSingletonOptions)
            : base(dependencies)
        {
            _typeMappingSource = typeMappingSource;
            _sqlGenerationHelper = dependencies.SqlGenerationHelper;
            _actianCompatibilityLevel = actianSingletonOptions.CompatibilityLevel;
        }

        // Transform OUTER APPLY to LEFT OUTER JOIN for Ingres compatibility
        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            Sql.Append("LEFT OUTER JOIN ");
            
            // Visit the table part - this handles the subquery
            Visit(outerApplyExpression.Table);
            
            // Add alias if present
            if (!string.IsNullOrEmpty(outerApplyExpression.Alias))
            {
                Sql.Append(" AS ");
                Sql.Append(_sqlGenerationHelper.DelimitIdentifier(outerApplyExpression.Alias));
            }
            
            // For OUTER APPLY, the correlation is implicit in the subquery
            // We need to add an ON clause, but since the correlation is in the WHERE clause
            // of the subquery, we use ON 1=1 (or ON CAST(1 AS boolean) = CAST(1 AS boolean) for Actian)
            Sql.Append(" ON CAST(1 AS boolean) = CAST(1 AS boolean)");
            
            return outerApplyExpression;
        }

        // Transform CROSS APPLY to INNER JOIN for Ingres compatibility
        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            Sql.Append("INNER JOIN ");
            
            // Visit the table part - this handles the subquery
            Visit(crossApplyExpression.Table);
            
            // Add alias if present
            if (!string.IsNullOrEmpty(crossApplyExpression.Alias))
            {
                Sql.Append(" AS ");
                Sql.Append(_sqlGenerationHelper.DelimitIdentifier(crossApplyExpression.Alias));
            }
            
            // For CROSS APPLY, use ON 1=1
            Sql.Append(" ON CAST(1 AS boolean) = CAST(1 AS boolean)");
            
            return crossApplyExpression;
        }

        // Add this method inside the ActianQuerySqlGenerator class
        protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression)
        {
            // EFCore 10: TypeMapping can be null, which causes NullReferenceException in base implementation
            if (sqlConstantExpression.TypeMapping == null)
            {
                // Generate SQL directly for constants without type mappings
                if (sqlConstantExpression.Value == null)
                {
                    Sql.Append("NULL");
                    return sqlConstantExpression;
                }

                if (sqlConstantExpression.Value is bool boolValue)
                {
                    Sql.Append(boolValue ? "CAST(1 AS boolean)" : "CAST(0 AS boolean)");
                    return sqlConstantExpression;
                }

                // For other types, try to infer type mapping
                var typeMapping = _typeMappingSource.FindMapping(sqlConstantExpression.Type);
                if (typeMapping != null)
                {
                    Sql.Append(typeMapping.GenerateSqlLiteral(sqlConstantExpression.Value));
                }
                else
                {
                    // Ultimate fallback: use ToString
                    Sql.Append(sqlConstantExpression.Value.ToString());
                }
                return sqlConstantExpression;
            }

            // TypeMapping exists, use base implementation
            return base.VisitSqlConstant(sqlConstantExpression);
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
        {
            // Check if this is a Convert operation to a string type
            if (sqlUnaryExpression.OperatorType == ExpressionType.Convert
                && sqlUnaryExpression.Type == typeof(string))
            {
                // Check if we're converting from a numeric type or if the type mapping suggests string
                var operandType = sqlUnaryExpression.Operand.Type;
                var isNumericToString = operandType.IsNumeric();

                // Also check if the store type is a character type that might need trimming
                var storeType = sqlUnaryExpression.TypeMapping?.StoreType?.ToLowerInvariant();
                var needsTrim = isNumericToString 
                    || (storeType != null && (storeType.Contains("char") || storeType.Contains("varchar")));

                if (needsTrim)
                {
                    // Wrap the CAST with TRIM to remove trailing spaces
                    Sql.Append("TRIM(");
                    base.VisitSqlUnary(sqlUnaryExpression);
                    Sql.Append(")");
                    return sqlUnaryExpression;
                }
            }

            return base.VisitSqlUnary(sqlUnaryExpression);
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
            if (selectExpression.Limit != null && selectExpression.Offset == null)
            {
                Sql.Append("FIRST ");
                Visit(selectExpression.Limit);
                Sql.Append(" ");
            }
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            // Note: For Limit without Offset, Actian generates SELECT FIRST n ...
            if (selectExpression.Offset != null)
            {
                Sql.AppendLine();
                Sql.Append("OFFSET ");
                Visit(selectExpression.Offset);

                if (selectExpression.Limit != null)
                {
                    Sql.Append(" FETCH NEXT ");
                    Visit(selectExpression.Limit);
                    Sql.Append(" ROWS ONLY");
                }
            }
        }

        protected virtual void GenerateModulo(SqlBinaryExpression sqlBinaryExpression)
        {
            Sql.Append("mod(");
            Visit(sqlBinaryExpression.Left);
            Sql.Append(", ");
            Visit(sqlBinaryExpression.Right);
            Sql.Append(")");
        }

        protected virtual void GenerateBitwiseAnd(SqlExpression left, SqlExpression right, Type returnType)
            => GenerateBitwiseOperatorExpression(left, right, returnType, "&");

        protected virtual void GenerateBitwiseOr(SqlExpression left, SqlExpression right, Type returnType)
            => GenerateBitwiseOperatorExpression(left, right, returnType, "|");

        protected virtual void GenerateBitwiseExclusiveOr(SqlExpression left, SqlExpression right, Type returnType)
            => GenerateBitwiseOperatorExpression(left, right, returnType, "^");

        protected virtual void GenerateBitwiseOperatorExpression(SqlExpression left, SqlExpression right, Type returnType, string op)
        {
            static string getOperandConverter(Type returnType)
            {
                if (returnType.IsEnum)
                    returnType = returnType.GetEnumUnderlyingType();

                if (returnType == typeof(bool) || returnType == typeof(byte) || returnType == typeof(sbyte))
                    return "INT1";

                if (returnType == typeof(short))
                    return "INT2";

                if (returnType == typeof(int))
                    return "INT4";

                if (returnType == typeof(long))
                    return "INT8";

                return "";
            }

            returnType = returnType.UnwrapNullableType();

            var converter = getOperandConverter(returnType);

            if (returnType == typeof(bool))
                Sql.Append("BOOLEAN(");

            Sql.Append(converter);
            Sql.Append("(");
            Visit(left);
            Sql.Append(")");

            Sql.Append($" {op} ");


            Sql.Append(converter);
            Sql.Append("(");
            Visit(right);
            Sql.Append(")");

            if (returnType == typeof(bool))
                Sql.Append(")");
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            switch (sqlBinaryExpression.OperatorType)
            {
                case ExpressionType.Modulo:
                    GenerateModulo(sqlBinaryExpression);
                    return sqlBinaryExpression;
                case ExpressionType.And:
                    GenerateBitwiseAnd(sqlBinaryExpression.Left, sqlBinaryExpression.Right, sqlBinaryExpression.Type);
                    return sqlBinaryExpression;
                case ExpressionType.Or:
                    GenerateBitwiseOr(sqlBinaryExpression.Left, sqlBinaryExpression.Right, sqlBinaryExpression.Type);
                    return sqlBinaryExpression;
                case ExpressionType.ExclusiveOr:
                    GenerateBitwiseExclusiveOr(sqlBinaryExpression.Left, sqlBinaryExpression.Right, sqlBinaryExpression.Type);
                    return sqlBinaryExpression;
                default:
                    return base.VisitSqlBinary(sqlBinaryExpression);
            }
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.Name == ActianFakeFunctions.ExclusiveOr && sqlFunctionExpression.Arguments.Count == 2)
            {
                GenerateBitwiseExclusiveOr(sqlFunctionExpression.Arguments[0], sqlFunctionExpression.Arguments[1], sqlFunctionExpression.Type);
                return sqlFunctionExpression;
            }

            if (!sqlFunctionExpression.IsBuiltIn
                && string.IsNullOrEmpty(sqlFunctionExpression.Schema))
            {
                sqlFunctionExpression = new SqlFunctionExpression(
                    sqlFunctionExpression.Name,
                    sqlFunctionExpression.Arguments,
                    true,
                    new[] {true, true },
                    sqlFunctionExpression.Type,
                    sqlFunctionExpression.TypeMapping);
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }

        private bool RequiresBrackets(SqlExpression expression)
        {
            return expression is SqlBinaryExpression sqlBinary
                && sqlBinary.OperatorType != ExpressionType.Coalesce
                || expression is LikeExpression;
        }
    }
}
