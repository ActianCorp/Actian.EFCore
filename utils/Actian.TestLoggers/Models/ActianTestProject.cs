// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Actian.TestLoggers
{
    public class ActianTestProject
    {
        public ActianTestProject() { }

        public ActianTestProject(Dictionary<string, string> parameters, ActianTestResult firstResult)
        {
            Parameters = parameters;
            ProjectName = firstResult.ProjectName;
        }

        public string ProjectName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string ActianServer { get; set; }
        public string ActianServerPort { get; set; }
        public string ActianServerVersion { get; set; }
        public string ActianServerCompatibilty { get; set; }
        public DateTimeOffset? StartTime => Results.StartTime();
        public DateTimeOffset? EndTime => Results.EndTime();
        public TimeSpan Duration => Results.Duration();
        public List<ActianTestNamespace> TestNamespaces { get; set; } = new List<ActianTestNamespace>();
        public IEnumerable<string> TestNamespaceNames => TestNamespaces.Select(ns => ns.Namespace).OrderBy(n => n);
        public IEnumerable<string> TestClassNames => TestNamespaces.SelectMany(ns => ns.TestClassNames).OrderBy(n => n);

        [JsonIgnore]
        public IEnumerable<ActianTestResult> Results => TestNamespaces.SelectMany(c => c.Results);

        public void AddTestResult(ActianTestResult result)
        {
            GetTestNamespace(result).AddTestResult(result);

            foreach (var message in result.Messages)
            {
                foreach (var line in message.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (ActianTestMessages.IsActianServer(line, out var actianServer))
                        ActianServer = actianServer;

                    if (ActianTestMessages.IsActianServerPort(line, out var actianServerPort))
                        ActianServerPort = actianServerPort;

                    if (ActianTestMessages.IsActianServerVersion(line, out var actianServerVersion))
                        ActianServerVersion = actianServerVersion;

                    if (ActianTestMessages.IsActianServerCompatibilty(line, out var actianServerCompatibilty))
                        ActianServerCompatibilty = actianServerCompatibilty;
                }
            }
        }

        private ActianTestNamespace GetTestNamespace(ActianTestResult result)
        {
            var testNamespace = TestNamespaces.FirstOrDefault(c => c.Namespace == result.Namespace);
            if (testNamespace is null)
            {
                TestNamespaces.Add(testNamespace = new ActianTestNamespace(result));
                TestNamespaces = TestNamespaces.OrderBy(c => c.Namespace).ToList();
            }
            return testNamespace;
        }

        public override string ToString() => ProjectName;

        public string AnchorText => ProjectName;
    }
}
