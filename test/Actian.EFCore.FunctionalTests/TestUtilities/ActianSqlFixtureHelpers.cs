// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Actian.EFCore.TestUtilities
{
    public class ActianSqlFixtureHelpers
    {
        public ActianSqlFixtureHelpers(IActianSqlFixture fixture, ITestOutputHelper testOutputHelper)
            : this(fixture.TestSqlLoggerFactory, testOutputHelper)
        {
        }

        public ActianSqlFixtureHelpers(ListLoggerFactory listLoggerFactory, ITestOutputHelper testOutputHelper)
        {
            ListLoggerFactory = listLoggerFactory;
            TestOutputHelper = testOutputHelper;
            ListLoggerFactory.SetTestOutputHelper(testOutputHelper);
            ClearLog();
        }

        public ITestOutputHelper TestOutputHelper { get; }
        public ListLoggerFactory ListLoggerFactory { get; }

        public void LogSql()
        {
            if (ListLoggerFactory is TestSqlLoggerFactory testSqlLoggerFactory)
            {
                TestOutputHelper.WriteLine(NormalizeSqlStatements(testSqlLoggerFactory.SqlStatements));
            }
            else
            {
                TestOutputHelper.WriteLine($"ListLoggerFactory is not a TestSqlLoggerFactory");
            }
        }

        public void AssertSql(params string[] expected)
        {
            if (ListLoggerFactory is TestSqlLoggerFactory testSqlLoggerFactory)
            {
                var actual = NormalizeSqlStatements(testSqlLoggerFactory.SqlStatements.Take(expected.Length));
                TestOutputHelper.WriteLine(actual);
                actual.Should().NotDifferFrom(NormalizeSqlStatements(expected));
            }
            else
            {
                throw new Exception($"ListLoggerFactory is not a TestSqlLoggerFactory");
            }
        }

        public static void AssertSql(string expected, string actual)
        {
            actual.Should().NotDifferFrom(NormalizeSqlStatement(expected, 0));
        }

        private static string NormalizeSqlStatements(IEnumerable<string> statements)
        {
            return string.Join("", statements.Select(NormalizeSqlStatement));
        }

        private static string NormalizeSqlStatement(string statement, int index)
        {
            return index == 0
                ? $"<Statement {index + 1}>\n{statement.NormalizeText()}\n"
                : $"\n<Statement {index + 1}>\n{statement.NormalizeText()}\n";
        }

        public void ClearLog() => ListLoggerFactory.Clear();
    }
}
