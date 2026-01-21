// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Runtime.CompilerServices;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ActianSkipAnsiAttribute : ActianSkipAttribute
    {
        public ActianSkipAnsiAttribute([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(ActianSkipReasons.TestFailsForAnsi, ActianCompatibility.Ansi, sourceFilePath, sourceLineNumber)
        {
        }
    }
}
