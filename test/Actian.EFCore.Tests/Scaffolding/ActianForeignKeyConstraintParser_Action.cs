// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Scaffolding.Internal;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations;
using Sprache;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore.Tests.Scaffolding
{
    public class ActianForeignKeyConstraintParser_Action
    {
        public ActianForeignKeyConstraintParser_Action(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("cascade", ReferentialAction.Cascade)]
        [InlineData("set null", ReferentialAction.SetNull)]
        [InlineData("set    null", ReferentialAction.SetNull)]
        [InlineData("restrict", ReferentialAction.Restrict)]
        [InlineData("no action", ReferentialAction.NoAction)]
        [InlineData("no    action", ReferentialAction.NoAction)]
        [InlineData("set default", ReferentialAction.SetDefault)]
        [InlineData("set    default", ReferentialAction.SetDefault)]
        public void Can_parse(string str, ReferentialAction expected)
        {
            var actual = ActianForeignKeyConstraintParser.Action.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("cascad")]
        [InlineData("cascadee")]
        [InlineData("setnull")]
        [InlineData("noaction")]
        [InlineData("setdefault")]
        public void Can_not_parse(string str)
        {
            var actual = ActianForeignKeyConstraintParser.Action.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
