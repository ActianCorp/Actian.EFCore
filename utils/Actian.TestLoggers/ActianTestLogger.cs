// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Actian.TestLoggers
{
    public abstract class ActianTestLogger : ITestLoggerWithParameters
    {
        public abstract string DefaultTestResultFileName { get; }

        public LoggerConfiguration Config { get; private set; }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            if (testRunDirectory is null)
                throw new ArgumentNullException(nameof(testRunDirectory));

            Initialize(events, new Dictionary<string, string>
            {
                { DefaultLoggerParameterNames.TestRunDirectory, testRunDirectory },
                { LoggerConfiguration.LogFileNameKey, DefaultTestResultFileName }
            });
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            if (!parameters.ContainsKey(LoggerConfiguration.LogFileNameKey))
                parameters[LoggerConfiguration.LogFileNameKey] = DefaultTestResultFileName;

            Config = new LoggerConfiguration(new Dictionary<string, string>(parameters));

            events.TestRunMessage += OnTestRunMessage;
            events.TestRunStart += OnTestRunStart;
            events.TestResult += OnTestResult;
            events.TestRunComplete += OnTestRunComplete;

            Initialize();
        }

        private ActianTestRun _testRun = null;

        protected virtual IEnumerable<IActianTestReporter> Reporters => Enumerable.Empty<IActianTestReporter>();

        protected virtual void Initialize()
        {
            _testRun = new ActianTestRun
            {
                Parameters = Config.Parameters
            };
        }

        protected virtual void OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
        }

        protected virtual void OnTestRunStart(object sender, TestRunStartEventArgs e)
        {
        }

        protected virtual void OnTestResult(object sender, TestResultEventArgs e)
        {
            _testRun.AddTestResult(e.Result);
        }

        protected virtual void OnTestRunComplete(object sender, TestRunCompleteEventArgs e)
        {
            Reporters.CreateReports(_testRun.TestProjects);
        }
    }
}
