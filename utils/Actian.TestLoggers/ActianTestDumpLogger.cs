// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Actian.TestLoggers
{
    [FriendlyName("actian-dump")] // Alternate user friendly string to uniquely identify the console logger.
    [ExtensionUri("logger://actian.com/TestPlatform/ActianTestDumpLogger/v1")] // Uri used to uniquely identify the logger.
    public class ActianTestDumpLogger : ActianTestLogger
    {
        public override string DefaultTestResultFileName => "TestResults.dump.json";

        protected override IEnumerable<IActianTestReporter> Reporters
        {
            get
            {
                yield break;
            }
        }

        private readonly List<TestResult> Results = new List<TestResult>();

        protected override void OnTestRunStart(object sender, TestRunStartEventArgs e)
        {
        }

        protected override void OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
        }

        protected override void OnTestResult(object sender, TestResultEventArgs e)
        {
            Results.Add(e.Result);
        }

        protected override void OnTestRunComplete(object sender, TestRunCompleteEventArgs e)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Config.LogFilePath));
            using var writer = new StreamWriter(Config.LogFilePath, false, new UTF8Encoding(false));
            writer.Write(JsonConvert.SerializeObject(Results, Formatting.Indented, new StringEnumConverter()));
            Console.WriteLine($"Test results written to {Config.LogFilePath}");
        }
    }
}
