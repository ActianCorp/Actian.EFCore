// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Actian.TestLoggers
{
    public class ActianTestInstallation
    {
        public static ActianTestInstallation Create(IEnumerable<ActianTestProject> testProjects)
        {
            return new ActianTestInstallation(testProjects);
        }

        private readonly ActianTestProject _firstProject;
        private ActianTestInstallation(IEnumerable<ActianTestProject> testProjects)
        {
            TestProjects = testProjects?.ToList() ?? throw new ArgumentNullException(nameof(testProjects));
            _firstProject = TestProjects.First();
        }

        [JsonIgnore]
        public IEnumerable<ActianTestResult> Results => TestProjects.SelectMany(c => c.Results);

        public List<ActianTestProject> TestProjects { get; set; } = new List<ActianTestProject>();
        public string ActianServer => _firstProject.ActianServer;
        public string ActianServerPort => _firstProject.ActianServerPort;
        public string ActianServerVersion => _firstProject.ActianServerVersion;
        public string ActianServerCompatibilty => _firstProject.ActianServerCompatibilty;

        public override string ToString()
            => $"{ActianServer} / {ActianServerPort}";

        public string AnchorText => $"{ActianServer}/{ActianServerPort}";
    }
}
