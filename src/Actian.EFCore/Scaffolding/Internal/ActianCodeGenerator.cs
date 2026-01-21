// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Reflection;
using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Actian.EFCore.Infrastructure;

namespace Actian.EFCore.Scaffolding.Internal
{
    public class ActianCodeGenerator : ProviderCodeGenerator
    {
        private static readonly MethodInfo UseActianMethodInfo
            = typeof(ActianDbContextOptionsExtensions).GetRuntimeMethod(
                nameof(ActianDbContextOptionsExtensions.UseActian),
                new[] { typeof(DbContextOptionsBuilder), typeof(string), typeof(Action<ActianDbContextOptionsBuilder>) })!;

        public ActianCodeGenerator([NotNull] ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override MethodCallCodeFragment GenerateUseProvider(
            string connectionString,
            MethodCallCodeFragment providerOptions)
            => new MethodCallCodeFragment(
                nameof(ActianDbContextOptionsExtensions.UseActian),
                providerOptions == null
                    ? new object[] { connectionString }
                    : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
    }
}
