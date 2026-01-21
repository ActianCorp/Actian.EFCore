// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;

namespace Actian.TestLoggers
{
    public class ActianTestResult
    {
        public ActianTestResult() { }
        public ActianTestResult(TestResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            FullyQualifiedName = result.TestCase.FullyQualifiedName;
            TestClass = string.Join(".", FullyQualifiedName.Split('.').SkipLast(1));
            Namespace = string.Join(".", TestClass.Split('.').SkipLast(1));
            DisplayName = result.DisplayName.StartsWith($"{TestClass}.")
                ? result.DisplayName.Substring(TestClass.Length + 1)
                : result.DisplayName;
            Source = result.TestCase.Source;
            ProjectName = Regex.Replace(Source.Split('/', '\\').Last(), @"\.(?:dll|exe)$", "");
            CodeFilePath = result.TestCase.CodeFilePath;
            LineNumber = result.TestCase.LineNumber;
            Outcome = result.Outcome.ToActianTestOutcome(result.Messages);
            ErrorMessage = result.ErrorMessage;
            ErrorStackTrace = result.ErrorStackTrace;
            Messages = new List<TestResultMessage>(result.Messages);
            Duration = result.Duration;
            StartTime = result.StartTime;
            EndTime = result.EndTime;
        }

        public string FullyQualifiedName { get; set; }
        public string Namespace { get; set; }
        public string TestClass { get; set; }
        public string DisplayName { get; set; }
        public string Source { get; set; }
        public string ProjectName { get; set; }
        public string CodeFilePath { get; set; }
        public int LineNumber { get; set; }
        public ActianTestOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }
        public IEnumerable<TestResultMessage> Messages { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        [JsonIgnore]
        public string MethodName => FullyQualifiedName.Split('.').Last();
    }
}
