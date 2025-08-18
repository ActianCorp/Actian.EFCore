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
    public class ActianNameFilterParser_TableNameWithoutSchema
    {
        public ActianNameFilterParser_TableNameWithoutSchema(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("table", "table", false)]
        [InlineData("\"table\"", "table", true)]
        [InlineData("\"my table\"", "my table", true)]
        [InlineData("\"my \"\"table\"\"\"", "my \"table\"", true)]
        [InlineData("()", "()", false)]
        public void Can_parse(string str, string expectedTableName, bool expectedTableDelimited)
        {
            var actual = ActianNameFilterParser.TableNameWithoutSchema.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().BeEquivalentTo((
                schema: (name: (string)null, delimited: false),
                name: (name: expectedTableName, delimited: expectedTableDelimited)
            ));
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"")]
        [InlineData("my schema.table")]
        [InlineData("schema.table")]
        [InlineData("\"schema\".\"table\"")]
        [InlineData("\"my schema\".table")]
        [InlineData("\"my \"\"schema\"\"\".table")]
        public void Can_not_parse(string str)
        {
            var actual = ActianNameFilterParser.TableNameWithoutSchema.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
