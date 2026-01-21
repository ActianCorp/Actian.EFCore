// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Actian.EFCore.TestUtilities
{
    internal class ActianTestStoreLogger : ITestOutputHelper
    {
        private static readonly Encoding UTF8 = new UTF8Encoding(false);
        private static readonly HashSet<string> _names = new HashSet<string>();

        public ActianTestStoreLogger(string name)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            if (!_names.Contains(name))
            {
                File.Delete(LogFilePath);
                _names.Add(name);
            }
        }

        public string Name { get; }
        public string LogFilePath => Path.Combine(TestEnvironment.LogDirectory, $"{Name}.log");

        public void WriteLine(string message)
        {
            File.AppendAllLines(LogFilePath, new[] { message }, UTF8);
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
    }
}
