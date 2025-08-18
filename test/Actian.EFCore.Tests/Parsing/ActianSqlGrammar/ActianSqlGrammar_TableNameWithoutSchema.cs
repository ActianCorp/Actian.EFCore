// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Sprache;
using Xunit;
using Xunit.Abstractions;
using static Actian.EFCore.Parsing.Internal.ActianSqlGrammar;

namespace Actian.EFCore.Tests.Parsing.ActianSqlGrammar
{
    public class ActianSqlGrammar_TableNameWithoutSchema
    {
        public ActianSqlGrammar_TableNameWithoutSchema(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("table", "table")]
        [InlineData("\"table\"", "table")]
        [InlineData("\"my table\"", "my table")]
        [InlineData("\"my \"\"table\"\"\"", "my \"table\"")]
        public void Can_parse(string str, string expectedTable)
        {
            TableNameWithoutSchema.End().Parse(str).Should().BeEquivalentTo(((string)null, expectedTable));
        }

        [Theory]
        [InlineData("()")]
        [InlineData("my schema.table")]
        [InlineData("schema.table")]
        [InlineData("\"schema\".\"table\"")]
        [InlineData("\"my schema\".table")]
        [InlineData("\"my \"\"schema\"\"\".table")]
        public void Can_not_parse(string str)
        {
            TableNameWithoutSchema.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
