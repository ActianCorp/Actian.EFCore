// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Actian.EFCore
{
    public class TextDiffLineAssertions : ReferenceTypeAssertions<IEnumerable<TextDiffLine>, TextDiffLineAssertions>
    {
        public TextDiffLineAssertions(IEnumerable<TextDiffLine> subject) : base(subject)
        {
        }

        protected override string Identifier => "TextDiff";

        public AndConstraint<TextDiffLineAssertions> HaveChanges(int? context = 3, int maxLines = int.MaxValue, int maxChanges = int.MaxValue, Func<string, string> formatLineNumbers = null, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.HasDifferences())
                .FailWith($"Texts do not differ:{Environment.NewLine}{Subject.Format(context, formatLineNumbers, maxLines: maxLines, maxChanges: maxChanges)}");

            return new AndConstraint<TextDiffLineAssertions>(this);
        }

        public AndConstraint<TextDiffLineAssertions> HaveNoChanges(int? context = 3, int maxLines = int.MaxValue, int maxChanges = int.MaxValue, Func<string, string> formatLineNumbers = null, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.HasNoDifferences())
                .FailWith($"Texts differ:{Environment.NewLine}{Subject.Format(context, formatLineNumbers, maxLines: maxLines, maxChanges: maxChanges)}");

            return new AndConstraint<TextDiffLineAssertions>(this);
        }
    }

    public static class TextDiffAssertionExtensions
    {
        public static AndConstraint<StringAssertions> NotDifferFrom(this StringAssertions assertions, string expected, int? context = 3, int maxLines = int.MaxValue, int maxChanges = int.MaxValue, Func<string, string> formatLineNumbers = null, bool normalize = true, string because = "", params object[] becauseArgs)
        {
            Text.Diff(expected, assertions.Subject, normalize)
                .Should()
                .HaveNoChanges(context, maxLines, maxChanges, formatLineNumbers, because, becauseArgs);

            return new AndConstraint<StringAssertions>(assertions);
        }
    }
}
