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
    public class ActianSqlGrammar_TableNameWithSchema
    {
        public ActianSqlGrammar_TableNameWithSchema(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("schema.table", "schema", "table")]
        [InlineData("\"schema\".\"table\"", "schema", "table")]
        [InlineData("\"my schema\".table", "my schema", "table")]
        [InlineData("\"my \"\"schema\"\"\".table", "my \"schema\"", "table")]
        public void Can_parse(string str, string expectedSchema, string expectedTable)
        {
            TableNameWithSchema.End().Parse(str).Should().BeEquivalentTo((expectedSchema, expectedTable));
        }

        [Theory]
        [InlineData("()")]
        [InlineData("my schema.table")]
        [InlineData("table")]
        public void Can_not_parse(string str)
        {
            TableNameWithSchema.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
