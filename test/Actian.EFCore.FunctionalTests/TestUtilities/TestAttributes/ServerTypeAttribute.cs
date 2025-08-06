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
    public class ServerTypeAttribute : ActianTestAttribute, ITestCondition
    {
        public ServerTypeAttribute(string serverType, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
            ServerType = serverType ?? throw new ArgumentNullException(nameof(serverType));
        }

        public string ServerType { get; }

        public ValueTask<bool> IsMetAsync() => new ValueTask<bool>(TestEnvironment.ActianServerVersion.ServerType == ServerType);

        public string SkipReason => $"Requires server type \"{ServerType}\". The server type is \"{TestEnvironment.ActianServerVersion.ServerType}\".";
    }
}
