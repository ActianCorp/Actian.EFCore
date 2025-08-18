// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Actian.EFCore.TestUtilities
{
    public class ActianTestLoggerProvider : ILoggerProvider, ITestOutputHelper
    {
        public ITestOutputHelper Output { get; set; }

        public ILogger CreateLogger(string categoryName)
        {
            return new ActianTestLogger(this);
        }

        public void WriteLine(string message)
        {
            Output?.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            Output?.WriteLine(format, args);
        }

        public void Dispose()
        {
            Output = null;
        }
    }
}
