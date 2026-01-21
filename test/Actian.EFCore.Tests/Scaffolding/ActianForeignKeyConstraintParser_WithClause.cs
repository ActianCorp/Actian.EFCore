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
    public class ActianForeignKeyConstraintParser_WithClause
    {
        public ActianForeignKeyConstraintParser_WithClause(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("with anything can come after with")]
        public void Can_parse(string str)
        {
            var actual = ActianForeignKeyConstraintParser.WithClause.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
        }

        [Theory]
        [InlineData("withanything can come after with, but a space must come after with")]
        public void Can_not_parse(string str)
        {
            var actual = ActianForeignKeyConstraintParser.WithClause.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
