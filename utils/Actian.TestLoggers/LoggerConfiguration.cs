// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Actian.TestLoggers
{
    public class LoggerConfiguration
    {
        public const string LogFileNameKey = "LogFileName";
        public const string LogFilePathKey = "LogFilePath";
        public const string TestRunDirectoryKey = DefaultLoggerParameterNames.TestRunDirectory;

        public LoggerConfiguration(Dictionary<string, string> parameters)
        {
            Parameters = new Dictionary<string, string>(parameters);

            if (!string.IsNullOrEmpty(LogFilePath))
            {
                Parameters[TestRunDirectoryKey] = Path.GetDirectoryName(LogFilePath);
                Parameters[LogFileNameKey] = Path.GetFileName(LogFilePath);
            }
            else if (!string.IsNullOrEmpty(TestRunDirectory) && !string.IsNullOrEmpty(LogFileName))
            {
                Parameters[LogFilePathKey] = Path.Combine(TestRunDirectory, LogFileName);
            }
        }

        public Dictionary<string, string> Parameters { get; }

        public string this[string key] => GetParameter(key);

        public string TestRunDirectory
        {
            get => GetParameter(TestRunDirectoryKey);
            set
            {
                Parameters[TestRunDirectoryKey] = string.IsNullOrEmpty(value) ? null : value;
                if (string.IsNullOrEmpty(TestRunDirectory) || string.IsNullOrEmpty(LogFileName))
                {
                    Parameters[LogFilePathKey] = null;
                }
                else
                {
                    Parameters[LogFilePathKey] = Path.Combine(TestRunDirectoryKey, LogFileName);
                }
            }
        }

        public string LogFileName
        {
            get => GetParameter(LogFileNameKey);
            set
            {
                Parameters[TestRunDirectoryKey] = string.IsNullOrEmpty(value) ? null : value;
                if (string.IsNullOrEmpty(TestRunDirectory) || string.IsNullOrEmpty(LogFileName))
                {
                    Parameters[LogFilePathKey] = null;
                }
                else
                {
                    Parameters[LogFilePathKey] = Path.Combine(TestRunDirectoryKey, LogFileName);
                }
            }
        }

        public string LogFilePath
        {
            get => GetParameter(LogFilePathKey);
            set
            {
                Parameters[LogFilePathKey] = string.IsNullOrEmpty(value) ? null : value;
                if (!string.IsNullOrEmpty(LogFilePathKey))
                {
                    Parameters[LogFileNameKey] = Path.GetFileName(LogFilePathKey);
                    Parameters[TestRunDirectoryKey] = Path.GetDirectoryName(LogFilePathKey);
                }
            }
        }

        private string GetParameter(string key)
            => Parameters.TryGetValue(key, out var value) ? value : null;
    }
}
