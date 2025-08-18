// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Actian.TestLoggers
{
    public class ActianTestClass
    {
        public ActianTestClass() { }

        public ActianTestClass(ActianTestResult firstResult)
        {
            ClassName = firstResult.TestClass;
            ClassDisplayName = ClassName.Split('.').Last();
        }

        public string ClassName { get; set; }
        public string ClassDisplayName { get; set; }
        public string Implements { get; set; }
        public DateTimeOffset? StartTime => Results.StartTime();
        public DateTimeOffset? EndTime => Results.EndTime();
        public TimeSpan Duration => Results.Duration();
        public List<ActianTestResult> Results { get; set; } = new List<ActianTestResult>();

        public void AddTestResult(ActianTestResult result)
        {
            Results.Add(result);

            foreach (var message in result.Messages)
            {
                foreach (var line in message.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (ActianTestMessages.IsTestClassImplements(line, out var implements))
                        Implements = implements;
                }
            }
        }

        public override string ToString() => ClassDisplayName;
        public string AnchorText => ClassName;

        public string GetTestName(ActianTestResult testResult)
        {
            var others = Results
                .Where(r => r.MethodName == testResult.MethodName);

            if (others.Count() == 1)
                return testResult.MethodName;

            var number = others.TakeWhile(r => r != testResult).Count() + 1;

            return $"{testResult.MethodName}-{number}";
        }
    }
}
