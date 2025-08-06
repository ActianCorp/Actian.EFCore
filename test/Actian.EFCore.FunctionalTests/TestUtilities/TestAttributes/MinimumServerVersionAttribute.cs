// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class MinimumServerVersionAttribute : ActianTestAttribute, ITestCondition
    {
        public MinimumServerVersionAttribute(int major, int minor, string serverType = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
            MinimumServerVersion = new ActianServerVersion(major, minor, serverType);
        }

        public ActianServerVersion MinimumServerVersion { get; }

        public ValueTask<bool> IsMetAsync() => new ValueTask<bool>(TestEnvironment.ActianServerVersion.Supports(MinimumServerVersion));

        public string SkipReason => $"Requires {MinimumServerVersion} or later. The server version is \"{TestEnvironment.ServerVersion}\".";
    }
}
