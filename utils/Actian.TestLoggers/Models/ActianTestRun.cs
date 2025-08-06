// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;

namespace Actian.TestLoggers
{
    public class ActianTestRun
    {
        [JsonIgnore]
        public Dictionary<string, string> Parameters { get; set; }

        [JsonIgnore]
        public IEnumerable<ActianTestResult> Results => TestProjects.SelectMany(c => c.Results);

        public List<ActianTestProject> TestProjects { get; set; } = new List<ActianTestProject>();

        public void AddTestResult(TestResult result)
        {
            AddTestResult(new ActianTestResult(result));
        }

        public void AddTestResult(ActianTestResult result)
        {
            GetTestProject(result).AddTestResult(result);
        }

        private ActianTestProject GetTestProject(ActianTestResult result)
        {
            var project = TestProjects.FirstOrDefault(c => c.ProjectName == result.ProjectName);
            if (project is null)
            {
                TestProjects.Add(project = new ActianTestProject(Parameters, result));
            }
            return project;
        }
    }
}
