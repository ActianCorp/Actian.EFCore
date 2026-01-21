// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Actian.TestLoggers
{
    public enum ActianTestOutcome
    {
        None = 0,
        Passed = 1,
        Failed = 2,
        Skipped = 3,
        NotFound = 4,
        Todo = 5
    }

    public static class TestOutcomeExtensions
    {
        public static ActianTestOutcome ToActianTestOutcome(this TestOutcome testOutcome, IEnumerable<TestResultMessage> messages)
        {
            var actianTestOutcome = testOutcome switch
            {
                TestOutcome.None => ActianTestOutcome.None,
                TestOutcome.Passed => ActianTestOutcome.Passed,
                TestOutcome.Failed => ActianTestOutcome.Failed,
                TestOutcome.Skipped => ActianTestOutcome.Skipped,
                TestOutcome.NotFound => ActianTestOutcome.NotFound,
                _ => ActianTestOutcome.None
            };

            if (actianTestOutcome == ActianTestOutcome.Skipped && messages.Any(message => message.Text.StartsWith("Todo", StringComparison.InvariantCultureIgnoreCase)))
                return ActianTestOutcome.Todo;

            return actianTestOutcome;
        }
    }
}
