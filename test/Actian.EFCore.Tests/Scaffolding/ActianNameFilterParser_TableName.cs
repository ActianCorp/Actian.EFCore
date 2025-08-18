// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Scaffolding.Internal;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Sprache;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore.Tests.Scaffolding
{
    public class ActianNameFilterParser_TableName
    {
        public ActianNameFilterParser_TableName(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("table", null, false, "table", false)]
        [InlineData("\"table\"", null, false, "table", true)]
        [InlineData("\"my table\"", null, false, "my table", true)]
        [InlineData("\"my \"\"table\"\"\"", null, false, "my \"table\"", true)]
        [InlineData("schema.table", "schema", false, "table", false)]
        [InlineData("\"schema\".\"table\"", "schema", true, "table", true)]
        [InlineData("\"my schema\".table", "my schema", true, "table", false)]
        [InlineData("\"my \"\"schema\"\"\".table", "my \"schema\"", true, "table", false)]
        [InlineData("()", null, false, "()", false)]
        [InlineData("my schema.table", "my schema", false, "table", false)]
        [InlineData("my table", null, false, "my table", false)]
        public void Can_parse(string str, string expectedSchemaName, bool expectedSchemaDelimited, string expectedTableName, bool expectedTableDelimited)
        {
            var actual = ActianNameFilterParser.TableName.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().BeEquivalentTo((
                schema: (name: expectedSchemaName, delimited: expectedSchemaDelimited),
                name: (name: expectedTableName, delimited: expectedTableDelimited)
            ));
        }

        [Theory]
        [InlineData("")]
        [InlineData(@"""")]
        public void Can_not_parse(string str)
        {
            var actual = ActianNameFilterParser.TableName.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
