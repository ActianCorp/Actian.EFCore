// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Sprache;
using Xunit;
using Xunit.Abstractions;
using static Actian.EFCore.Parsing.Internal.ActianSqlGrammar;
using static Sprache.Parse;

namespace Actian.EFCore.Tests.Parsing.ActianSqlGrammar
{
    public class ActianSqlGrammar_Comma
    {
        public ActianSqlGrammar_Comma(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData(",")]
        [InlineData("   ,")]
        [InlineData(",   ")]
        [InlineData("   ,   ")]
        public void Can_parse(string str)
        {
            Comma.End().Parse(str).Should().Be(',');
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        [InlineData(" x  ,")]
        [InlineData(", x  ")]
        [InlineData(" x  , x  ")]
        public void Can_not_parse(string str)
        {
            Comma.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
