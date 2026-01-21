// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Actian.TestLoggers
{
    public class ActianJsonReporter : IActianTestReporter
    {
        public ActianJsonReporter(string logFilePath)
        {
            LogFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
        }

        public string LogFilePath { get; }

        public void CreateReport(IEnumerable<ActianTestProject> testProjects)
        {
            WriteJson(LogFilePath, testProjects
                .OrderBy(p => p.ActianServerVersion)
                .ThenByDescending(p => p.ActianServerCompatibilty)
                .ThenBy(p => p.ProjectName)
            );
        }

        private void WriteJson(string path, object value)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
            writer.Write(JsonConvert.SerializeObject(value, Formatting.Indented, new StringEnumConverter()));
            Console.WriteLine($"Test results written to {path}");
        }
    }
}
