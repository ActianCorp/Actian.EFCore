// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Actian.EFCore.Infrastructure.Internal
{
    public interface IActianSingletonOptions : ISingletonOptions
    {
        int CompatibilityLevel { get; }

        int? CompatibilityLevelWithoutDefault { get; }

        bool ExpandCollectionParameters { get; }

        bool? ExpandCollectionParametersWithoutDefault { get; }
    }
}
