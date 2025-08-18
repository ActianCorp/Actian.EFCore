// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore.Tests
{
    public class SqlStatementTerminators
    {
        private static readonly Regex SqlStatementTerminatorRe = new Regex(@";\s*$|\\g", RegexOptions.Multiline);
        private static IEnumerable<string> SplitSqlStatements(string statements)
        {
            return SqlStatementTerminatorRe
                .Split(statements)
                .Select(statement => statement.Trim())
                .Where(statement => !string.IsNullOrWhiteSpace(statement));
        }

        public SqlStatementTerminators(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Fact]
        public void SQL_statements_should_be_terminated_by_semicolon()
        {
            SplitSqlStatements(@"
                select name, text = 'this; is; some; text'
                  from person ;

                select name, text = 'this; is; also; some; text'
                  from person ;
            ").Should().BeEquivalentTo(new[] {
                @"select name, text = 'this; is; some; text'
                  from person",
                @"select name, text = 'this; is; also; some; text'
                  from person"
            });
        }

        [Fact]
        public void SQL_statements_should_be_terminated_by_backslash_g()
        {
            SplitSqlStatements(@"
                select name, text = 'this; is; some; text'
                  from person \g

                select name, text = 'this; is; also; some; text'
                  from person \g
            ").Should().BeEquivalentTo(new[] {
                @"select name, text = 'this; is; some; text'
                  from person",
                @"select name, text = 'this; is; also; some; text'
                  from person"
            });
        }
    }
}
