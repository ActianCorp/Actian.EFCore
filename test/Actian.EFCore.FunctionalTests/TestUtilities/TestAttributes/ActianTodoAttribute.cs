// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActianTodoAttribute : ActianTestAttribute, ITestCondition
    {
        public ActianTodoAttribute(string skipReason = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
            SkipReason = string.IsNullOrWhiteSpace(skipReason)
                ? "Todo"
                : $"Todo:\n{skipReason.NormalizeText()}";
        }

        public string SkipReason { get; }
        private static bool Disabled => Environment.GetEnvironmentVariable("DISABLE_ACTIAN_TODO") == "true";

        public ValueTask<bool> IsMetAsync() => new ValueTask<bool>(IsMet());

        private bool IsMet()
            => Disabled || Member.DeclaringType.GetCustomAttributes<ActianIncludeTodosAttribute>().Any();
    }
}
