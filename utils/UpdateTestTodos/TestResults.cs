// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Actian.TestLoggers;

namespace UpdateTestTodos
{
    internal static class TestResults
    {
        public static IEnumerable<ActianTestResult> GetResults(Paths paths)
        {
            return Actian.TestLoggers.Program
                .ReadTestProjects(paths.ActianEFCoreFunctionalTestResults)
                .SelectMany(project => project.Results)
                .ToList();
        }
    }
}
