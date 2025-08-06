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
    public class ActianNameFilterParser_UndelimitedIdentifier
    {
        public ActianNameFilterParser_UndelimitedIdentifier(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("_1table")]
        [InlineData("a2")]
        [InlineData("1table")]
        [InlineData("æøå")]
        [InlineData(@"a""")]
        public void Can_parse(string str)
        {
            var actual = ActianNameFilterParser.UndelimitedIdentifier.End().TryParse(str);
            actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Value.Should().Be(str);
        }

        [Theory]
        [InlineData("")]
        [InlineData(@"""a")]
        [InlineData(@"""a""")]
        public void Can_not_parse(string str)
        {
            var actual = ActianNameFilterParser.UndelimitedIdentifier.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
