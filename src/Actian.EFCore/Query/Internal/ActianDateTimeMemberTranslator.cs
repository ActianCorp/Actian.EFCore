// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable once CheckNamespace
#nullable enable
namespace Actian.EFCore.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class ActianDateTimeMemberTranslator(
    ISqlExpressionFactory sqlExpressionFactory,
    IRelationalTypeMappingSource typeMappingSource)
    : IMemberTranslator
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var declaringType = member.DeclaringType;

        if (declaringType != typeof(DateTime) && declaringType != typeof(DateTimeOffset))
        {
            return null;
        }

        return member.Name switch
        {
            nameof(DateTime.Year) => DatePart("'YEAR'"),
            nameof(DateTime.Month) => DatePart("'MONTH'"),
            nameof(DateTime.DayOfYear) => DatePart("'DAYOFYEAR'"),
            nameof(DateTime.DayOfWeek) => DatePart("'DAYOFWEEK'"),
            nameof(DateTime.Day) => DatePart("'DAY'"),
            nameof(DateTime.Hour) => DatePart("'HOUR'"),
            nameof(DateTime.Minute) => DatePart("'MINUTE'"),
            nameof(DateTime.Second) => DatePart("'SECOND'"),
            nameof(DateTime.Millisecond) => DatePart("'MILLISECOND'"),
            nameof(DateTime.Microsecond) => DatePart("'MICROSECOND'"),
            nameof(DateTime.Nanosecond) => DatePart("'NANOSECOND'"),
            nameof(DateTime.UnixEpoch) => DatePart("'EPOCH'"),

            nameof (DateTime.Date)
                => sqlExpressionFactory.Function(
                    "CONVERT",
                    new[] { sqlExpressionFactory.Fragment("date"), instance! },
                    nullable: true,
                    argumentsPropagateNullability: [false, true],
                    returnType,
                    declaringType == typeof(DateTime)
                        ? instance!.TypeMapping
                        : typeMappingSource.FindMapping(typeof(DateTime))),

            nameof(DateTime.TimeOfDay)
                => sqlExpressionFactory.Function(
                    "CONVERT",
                    new[] { sqlExpressionFactory.Fragment("time"), instance! },
                    nullable: true,
                    argumentsPropagateNullability: [false, true],
                    returnType),

            nameof(DateTime.Now)
                => sqlExpressionFactory.Function(
                    declaringType == typeof(DateTime) ? "GETDATE" : "SYSDATETIMEOFFSET",
                    arguments: [],
                    nullable: false,
                    argumentsPropagateNullability: [],
                    returnType),

            nameof(DateTime.UtcNow)
                when declaringType == typeof(DateTime)
                => sqlExpressionFactory.Function(
                    "GETUTCDATE",
                    arguments: [],
                    nullable: false,
                    argumentsPropagateNullability: [],
                    returnType),

            nameof(DateTime.UtcNow)
                when declaringType == typeof(DateTimeOffset)
                => sqlExpressionFactory.Convert(
                    sqlExpressionFactory.Function(
                        "SYSUTCDATETIME",
                        arguments: [],
                        nullable: false,
                        argumentsPropagateNullability: [],
                        returnType), returnType),

            nameof(DateTime.Today)
                => sqlExpressionFactory.Function(
                    "CONVERT",
                    new[]
                    {
                        sqlExpressionFactory.Fragment("date"),
                        sqlExpressionFactory.Function(
                            "GETDATE",
                            arguments: [],
                            nullable: false,
                            argumentsPropagateNullability: [],
                            typeof(DateTime))
                    },
                    nullable: true,
                    argumentsPropagateNullability: [false, true],
                    returnType),

            _ => null
        };

        SqlExpression DatePart(string part)
            => sqlExpressionFactory.Function(
                "DATE_PART",
                arguments: [sqlExpressionFactory.Fragment(part), instance!],
                nullable: true,
                argumentsPropagateNullability: new[] { false, true },
                returnType);
    }
}
