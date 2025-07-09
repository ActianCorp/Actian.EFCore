using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SqlExpression = Microsoft.EntityFrameworkCore.Query.SqlExpressions.SqlExpression;

#nullable enable

namespace Actian.EFCore.Query.Internal
{
    public class ActianStringMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo IndexOfMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), [typeof(string)])!;

        private static readonly MethodInfo IndexOfMethodInfoWithStartingPosition
            = typeof(string).GetRuntimeMethod(nameof(string.IndexOf), [typeof(string), typeof(int)])!;

        private static readonly MethodInfo ReplaceMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.Replace), [typeof(string), typeof(string)])!;

        private static readonly MethodInfo ToLowerMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        private static readonly MethodInfo ToUpperMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.ToUpper), Type.EmptyTypes)!;

        private static readonly MethodInfo SubstringMethodInfoWithOneArg
            = typeof(string).GetRuntimeMethod(nameof(string.Substring), [typeof(int)])!;

        private static readonly MethodInfo SubstringMethodInfoWithTwoArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Substring), [typeof(int), typeof(int)])!;

        private static readonly MethodInfo IsNullOrEmptyMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrEmpty), [typeof(string)])!;

        private static readonly MethodInfo IsNullOrWhiteSpaceMethodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrWhiteSpace), [typeof(string)])!;

        // Method defined in netcoreapp2.0 only
        private static readonly MethodInfo TrimStartMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), Type.EmptyTypes)!;

        private static readonly MethodInfo TrimEndMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), Type.EmptyTypes)!;

        private static readonly MethodInfo TrimMethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), Type.EmptyTypes)!;

        // Method defined in netstandard2.0
        private static readonly MethodInfo TrimStartMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), [typeof(char[])])!;

        private static readonly MethodInfo TrimEndMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), [typeof(char[])])!;

        private static readonly MethodInfo TrimMethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), [typeof(char[])])!;

        private static readonly MethodInfo TrimStartMethodInfoWithCharArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), [typeof(char)])!;

        private static readonly MethodInfo TrimEndMethodInfoWithCharArg
            = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), [typeof(char)])!;

        private static readonly MethodInfo FirstOrDefaultMethodInfoWithoutArgs
            = typeof(Enumerable).GetRuntimeMethods().Single(
                m => m.Name == nameof(Enumerable.FirstOrDefault)
                    && m.GetParameters().Length == 1).MakeGenericMethod(typeof(char));

        private static readonly MethodInfo LastOrDefaultMethodInfoWithoutArgs
            = typeof(Enumerable).GetRuntimeMethods().Single(
                m => m.Name == nameof(Enumerable.LastOrDefault)
                    && m.GetParameters().Length == 1).MakeGenericMethod(typeof(char));

        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private readonly IActianSingletonOptions _ActianSingletonOptions;


        public ActianStringMethodTranslator(ISqlExpressionFactory sqlExpressionFactory, IActianSingletonOptions ActianSingletonOptions)
        {
            _sqlExpressionFactory = sqlExpressionFactory;

            _ActianSingletonOptions = ActianSingletonOptions;
        }

        public virtual SqlExpression? Translate(
            SqlExpression? instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance != null)
            {
                if (IndexOfMethodInfo.Equals(method))
                {
                    return TranslateIndexOf(instance, method, arguments[0], null);
                }

                if (IndexOfMethodInfoWithStartingPosition.Equals(method))
                {
                    return TranslateIndexOf(instance, method, arguments[0], arguments[1]);
                }

                if (ReplaceMethodInfo.Equals(method))
                {
                    var firstArgument = arguments[0];
                    var secondArgument = arguments[1];
                    var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, firstArgument, secondArgument);

                    instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);
                    firstArgument = _sqlExpressionFactory.ApplyTypeMapping(firstArgument, stringTypeMapping);
                    secondArgument = _sqlExpressionFactory.ApplyTypeMapping(secondArgument, stringTypeMapping);

                    return _sqlExpressionFactory.Function(
                        "REPLACE",
                        new[] { instance, firstArgument, secondArgument },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true, true, true },
                        method.ReturnType,
                        stringTypeMapping);
                }

                if (ToLowerMethodInfo.Equals(method)
                    || ToUpperMethodInfo.Equals(method))
                {
                    return _sqlExpressionFactory.Function(
                        ToLowerMethodInfo.Equals(method) ? "LOWER" : "UPPER",
                        new[] { instance },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true },
                        method.ReturnType,
                        instance.TypeMapping);
                }

                if (SubstringMethodInfoWithOneArg.Equals(method))
                {
                    return _sqlExpressionFactory.Function(
                        "SUBSTRING",
                        new[]
                        {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1)),
                        _sqlExpressionFactory.Function(
                            "LENGTH",
                            new[] { instance },
                            nullable: true,
                            argumentsPropagateNullability: new[] { true },
                            typeof(int))
                        },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true, true, true },
                        method.ReturnType,
                        instance.TypeMapping);
                }

                if (SubstringMethodInfoWithTwoArgs.Equals(method))
                {
                    return _sqlExpressionFactory.Function(
                        "SUBSTRING",
                        new[]
                        {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1)),
                        arguments[1]
                        },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true, true, true },
                        method.ReturnType,
                        instance.TypeMapping);
                }

                // There's single-parameter LTRIM/RTRIM for all versions (trims whitespace), but startin with SQL Server 2022 there's also
                // an overload that accepts the characters to trim.
                if (method == TrimStartMethodInfoWithoutArgs
                    || (method == TrimStartMethodInfoWithCharArrayArg && arguments[0] is SqlConstantExpression { Value: char[] { Length: 0 } })
                    && (method == TrimStartMethodInfoWithCharArg || method == TrimStartMethodInfoWithCharArrayArg))
                {
                    return ProcessTrimStartEnd(instance, arguments, "LTRIM");
                }

                if (method == TrimEndMethodInfoWithoutArgs
                    || (method == TrimEndMethodInfoWithCharArrayArg && arguments[0] is SqlConstantExpression { Value: char[] { Length: 0 } })
                    && (method == TrimEndMethodInfoWithCharArg || method == TrimEndMethodInfoWithCharArrayArg))
                {
                    return ProcessTrimStartEnd(instance, arguments, "RTRIM");
                }

                if (method == TrimMethodInfoWithoutArgs
                    || (method == TrimMethodInfoWithCharArrayArg && arguments[0] is SqlConstantExpression { Value: char[] { Length: 0 } }))
                {
                    return _sqlExpressionFactory.Function(
                        "LTRIM",
                        new[]
                        {
                        _sqlExpressionFactory.Function(
                            "RTRIM",
                            new[] { instance },
                            nullable: true,
                            argumentsPropagateNullability: new[] { true },
                            instance.Type,
                            instance.TypeMapping)
                        },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true },
                        instance.Type,
                        instance.TypeMapping);
                }
            }

            if (IsNullOrEmptyMethodInfo.Equals(method))
            {
                var argument = arguments[0];

                return _sqlExpressionFactory.OrElse(
                    _sqlExpressionFactory.IsNull(argument),
                    _sqlExpressionFactory.Like(
                        argument,
                        _sqlExpressionFactory.Constant(string.Empty)));
            }

            if (IsNullOrWhiteSpaceMethodInfo.Equals(method))
            {
                var argument = arguments[0];

                return _sqlExpressionFactory.OrElse(
                    _sqlExpressionFactory.IsNull(argument),
                    _sqlExpressionFactory.Equal(
                        argument,
                        _sqlExpressionFactory.Constant(string.Empty, argument.TypeMapping)));
            }

            if (FirstOrDefaultMethodInfoWithoutArgs.Equals(method))
            {
                var argument = arguments[0];
                return _sqlExpressionFactory.Function(
                    "SUBSTRING",
                    new[] { argument, _sqlExpressionFactory.Constant(1), _sqlExpressionFactory.Constant(1) },
                    nullable: true,
                    argumentsPropagateNullability: new[] { true, true, true },
                    method.ReturnType);
            }

            if (LastOrDefaultMethodInfoWithoutArgs.Equals(method))
            {
                var argument = arguments[0];
                return _sqlExpressionFactory.Function(
                    "SUBSTRING",
                    new[]
                    {
                    argument,
                    _sqlExpressionFactory.Function(
                        "LENGTH",
                        new[] { argument },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(1)
                    },
                    nullable: true,
                    argumentsPropagateNullability: new[] { true, true, true },
                    method.ReturnType);
            }

            return null;
        }

        private SqlExpression TranslateIndexOf(
            SqlExpression instance,
            MethodInfo method,
            SqlExpression searchExpression,
            SqlExpression? startIndex)
        {
            var stringTypeMapping = ExpressionExtensions.InferTypeMapping(instance, searchExpression)!;
            searchExpression = _sqlExpressionFactory.ApplyTypeMapping(searchExpression, stringTypeMapping);
            instance = _sqlExpressionFactory.ApplyTypeMapping(instance, stringTypeMapping);

            var charIndexArguments = new List<SqlExpression> { searchExpression, instance };

            if (startIndex is not null)
            {
                charIndexArguments.Add(
                    startIndex is SqlConstantExpression { Value: int constantStartIndex }
                        ? _sqlExpressionFactory.Constant(constantStartIndex + 1, typeof(int))
                        : _sqlExpressionFactory.Add(startIndex, _sqlExpressionFactory.Constant(1)));
            }

            var argumentsPropagateNullability = Enumerable.Repeat(true, charIndexArguments.Count);

            SqlExpression charIndexExpression;
            var storeType = stringTypeMapping.StoreType;
            if (string.Equals(storeType, "nvarchar(max)", StringComparison.OrdinalIgnoreCase)
                || string.Equals(storeType, "varchar(max)", StringComparison.OrdinalIgnoreCase))
            {
                charIndexExpression = _sqlExpressionFactory.Function(
                    "POSITION",
                    charIndexArguments,
                    nullable: true,
                    argumentsPropagateNullability,
                    typeof(long));

                charIndexExpression = _sqlExpressionFactory.Convert(charIndexExpression, typeof(int));
            }
            else
            {
                charIndexExpression = _sqlExpressionFactory.Function(
                    "POSITION",
                    charIndexArguments,
                    nullable: true,
                    argumentsPropagateNullability,
                    method.ReturnType);
            }

            // If the pattern is an empty string, we need to special case to always return 0 (since CHARINDEX return 0, which we'd subtract to
            // -1). Handle separately for constant and non-constant patterns.
            if (searchExpression is SqlConstantExpression { Value: "" })
            {
                return _sqlExpressionFactory.Case(
                    [new CaseWhenClause(_sqlExpressionFactory.IsNotNull(instance), _sqlExpressionFactory.Constant(0))],
                    elseResult: null
                );
            }

            var offsetExpression = searchExpression is SqlConstantExpression
                ? _sqlExpressionFactory.Constant(1)
                : _sqlExpressionFactory.Case(
                    new[]
                    {
                    new CaseWhenClause(
                        _sqlExpressionFactory.Equal(
                            searchExpression,
                            _sqlExpressionFactory.Constant(string.Empty, stringTypeMapping)),
                        _sqlExpressionFactory.Constant(0))
                    },
                    _sqlExpressionFactory.Constant(1));

            return _sqlExpressionFactory.Subtract(charIndexExpression, offsetExpression);
        }

        private SqlExpression? ProcessTrimStartEnd(SqlExpression instance, IReadOnlyList<SqlExpression> arguments, string functionName)
        {
            SqlExpression? charactersToTrim = null;
            if (arguments.Count > 0 && arguments[0] is SqlConstantExpression { Value: var charactersToTrimValue })
            {
                charactersToTrim = charactersToTrimValue switch
                {
                    char singleChar => _sqlExpressionFactory.Constant(singleChar.ToString(), instance.TypeMapping),
                    char[] charArray => _sqlExpressionFactory.Constant(new string(charArray), instance.TypeMapping),
                    _ => throw new UnreachableException("Invalid parameter type for string.TrimStart/TrimEnd")
                };
            }

            return _sqlExpressionFactory.Function(
                functionName,
                arguments: charactersToTrim is null ? [instance] : [instance, charactersToTrim],
                nullable: true,
                argumentsPropagateNullability: charactersToTrim is null ? [true] : [true, true],
                instance.Type,
                instance.TypeMapping);
        }
    }
}
