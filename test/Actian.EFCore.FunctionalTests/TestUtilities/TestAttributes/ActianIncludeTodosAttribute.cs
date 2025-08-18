// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Runtime.CompilerServices;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActianIncludeTodosAttribute : ActianTestAttribute
    {
        public ActianIncludeTodosAttribute([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
        }
    }
}
