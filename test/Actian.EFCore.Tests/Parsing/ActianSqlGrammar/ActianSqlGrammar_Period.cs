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
    public class ActianSqlGrammar_Period
    {
        public ActianSqlGrammar_Period(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("   .")]
        [InlineData(".   ")]
        [InlineData("   .   ")]
        public void Can_parse(string str)
        {
            Period.End().Parse(str).Should().Be('.');
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" x  .")]
        [InlineData(". x  ")]
        [InlineData(" x  . x  ")]
        public void Can_not_parse(string str)
        {
            Period.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
