// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Actian.TestLoggers
{
    public class ActianTestNamespace
    {
        public ActianTestNamespace() { }

        public ActianTestNamespace(ActianTestResult firstResult)
        {
            Namespace = firstResult.Namespace;
        }

        public string Namespace { get; set; }
        public DateTimeOffset? StartTime => Results.StartTime();
        public DateTimeOffset? EndTime => Results.EndTime();
        public TimeSpan Duration => Results.Duration();
        public List<ActianTestClass> TestClasses { get; set; } = new List<ActianTestClass>();
        public IEnumerable<string> TestClassNames => TestClasses.OrderBy(c => c.ClassName).Select(c => c.ClassName);

        [JsonIgnore]
        public IEnumerable<ActianTestResult> Results => TestClasses.SelectMany(c => c.Results);

        public void AddTestResult(ActianTestResult result)
        {
            GetTestClass(result).AddTestResult(result);
        }

        public override string ToString() => Namespace;
        public string AnchorText => Namespace;

        private ActianTestClass GetTestClass(ActianTestResult result)
        {
            var testClass = TestClasses.FirstOrDefault(c => c.ClassName == result.TestClass);
            if (testClass is null)
            {
                TestClasses.Add(testClass = new ActianTestClass(result));
                TestClasses = TestClasses.OrderBy(c => c.ClassName).ToList();
            }
            return testClass;
        }
    }
}
