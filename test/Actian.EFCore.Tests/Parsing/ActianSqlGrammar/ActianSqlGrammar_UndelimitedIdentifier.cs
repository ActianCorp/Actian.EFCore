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
    public class ActianSqlGrammar_UndelimitedIdentifier
    {
        public ActianSqlGrammar_UndelimitedIdentifier(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("_1table")]
        [InlineData("a2")]
        [InlineData("æøå")]
        public void Can_parse(string str)
        {
            UndelimitedIdentifier.End().Parse(str).Should().Be(str);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1table")]
        public void Can_not_parse(string str)
        {
            UndelimitedIdentifier.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
