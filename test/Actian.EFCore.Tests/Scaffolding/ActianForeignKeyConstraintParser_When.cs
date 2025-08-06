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
    public class ActianForeignKeyConstraintParser_When
    {
        public ActianForeignKeyConstraintParser_When(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("update", ActianForeignKeyConstraintParser.RuleWhen.Update)]
        [InlineData("UPDATE", ActianForeignKeyConstraintParser.RuleWhen.Update)]
        [InlineData("upDate", ActianForeignKeyConstraintParser.RuleWhen.Update)]
        [InlineData("delete", ActianForeignKeyConstraintParser.RuleWhen.Delete)]
        [InlineData("DELETE", ActianForeignKeyConstraintParser.RuleWhen.Delete)]
        [InlineData("deLete", ActianForeignKeyConstraintParser.RuleWhen.Delete)]
        public void Can_parse(string str, ActianForeignKeyConstraintParser.RuleWhen expected)
        {
            var actual = ActianForeignKeyConstraintParser.When.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("updat")]
        [InlineData("updatee")]
        [InlineData("delet")]
        [InlineData("deletee")]
        public void Can_not_parse(string str)
        {
            var actual = ActianForeignKeyConstraintParser.When.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
