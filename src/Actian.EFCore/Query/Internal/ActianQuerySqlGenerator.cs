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
