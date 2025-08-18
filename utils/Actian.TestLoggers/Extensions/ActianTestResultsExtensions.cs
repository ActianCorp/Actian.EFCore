// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Actian.TestLoggers
{
    public static class ActianTestResultsExtensions
    {
        public static long Total(this IEnumerable<ActianTestResult> results) => results
            .LongCount();

        public static long Failed(this IEnumerable<ActianTestResult> results) => results
            .LongCount(r => r.Outcome == ActianTestOutcome.Failed);

        public static long Passed(this IEnumerable<ActianTestResult> results) => results
            .LongCount(r => r.Outcome == ActianTestOutcome.Passed);

        public static long Skipped(this IEnumerable<ActianTestResult> results) => results
            .LongCount(r => r.Outcome == ActianTestOutcome.Skipped);

        public static long Todo(this IEnumerable<ActianTestResult> results) => results
            .LongCount(r => r.Outcome == ActianTestOutcome.Todo);

        public static ActianTestOutcome Outcome(this IEnumerable<ActianTestResult> results) => results
            .Select(r => r.Outcome).Outcome();

        public static DateTimeOffset? StartTime(this IEnumerable<ActianTestResult> results) => results.Any()
            ? results.Select(r => r.StartTime).Min()
            : (DateTimeOffset?)null;

        public static DateTimeOffset? EndTime(this IEnumerable<ActianTestResult> results) => results.Any()
            ? results.Select(r => r.EndTime).Max()
            : (DateTimeOffset?)null;

        public static TimeSpan Duration(this IEnumerable<ActianTestResult> results)
        {
            var startTime = results.StartTime();
            var endTime = results.EndTime();
            return startTime is null || endTime is null ? TimeSpan.Zero : endTime.Value - startTime.Value;
        }
    }
}
