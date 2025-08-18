// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Actian.Infrastructure.Internal;

public class ActianSingletonOptions : IActianSingletonOptions
{
    public virtual int CompatibilityLevel { get; private set; } = ActianOptionsExtension.DefaultCompatibilityLevel;

    public virtual int? CompatibilityLevelWithoutDefault { get; private set; }

    public virtual void Initialize(IDbContextOptions options)
    {
        var actianOptions = options.FindExtension<ActianOptionsExtension>();
        if (actianOptions != null)
        {
            CompatibilityLevel = actianOptions.CompatibilityLevel;
            CompatibilityLevelWithoutDefault = actianOptions.CompatibilityLevelWithoutDefault;
        }
    }

    public virtual void Validate(IDbContextOptions options)
    {
        var actianOptions = options.FindExtension<ActianOptionsExtension>();

        if (actianOptions != null
            && (CompatibilityLevelWithoutDefault != actianOptions.CompatibilityLevelWithoutDefault
                || CompatibilityLevel != actianOptions.CompatibilityLevel))
        {
            throw new InvalidOperationException(
                CoreStrings.SingletonOptionChanged(
                    nameof(ActianDbContextOptionsExtensions.UseActian),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
        }
    }
}
