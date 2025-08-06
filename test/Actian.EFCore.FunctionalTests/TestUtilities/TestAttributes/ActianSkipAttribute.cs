// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

namespace Actian.EFCore.TestUtilities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ActianSkipAttribute : ActianTestAttribute, ITestCondition
    {
        public IEnumerable<ActianCondition> Conditions { get; set; }

        public ActianSkipAttribute(string skipReason, ActianCompatibility compatibility = ActianCompatibility.Ansi | ActianCompatibility.Ingres, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base(sourceFilePath, sourceLineNumber)
        {
            SkipReason = skipReason.NormalizeText() ?? throw new ArgumentNullException(nameof(skipReason));
            Compatibility = compatibility;
        }

        public ValueTask<bool> IsMetAsync() => Compatibility switch
        {
            ActianCompatibility.Ingres => new ValueTask<bool>(TestEnvironment.ActianServerCompatibilty != ActianCompatibility.Ingres),
            ActianCompatibility.Ansi => new ValueTask<bool>(TestEnvironment.ActianServerCompatibilty != ActianCompatibility.Ansi),
            _ => new ValueTask<bool>(false)
        };

        public string SkipReason { get; }
        public ActianCompatibility Compatibility { get; }
    }
}
