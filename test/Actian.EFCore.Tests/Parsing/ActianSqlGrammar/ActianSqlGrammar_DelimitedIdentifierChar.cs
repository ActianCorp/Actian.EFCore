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
    public class ActianSqlGrammar_DelimitedIdentifierChar
    {
        public ActianSqlGrammar_DelimitedIdentifierChar(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData("æ", 'æ')]
        [InlineData("\"\"", '"')]
        public void Can_parse(string str, char expected)
        {
            DelimitedIdentifierChar.End().Parse(str).Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"")]
        public void Can_not_parse(string str)
        {
            DelimitedIdentifierChar.End().TryParse(str).WasSuccessful.Should().Be(false);
        }
    }
}
