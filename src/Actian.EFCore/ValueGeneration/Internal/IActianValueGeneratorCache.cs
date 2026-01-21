// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Actian.EFCore.ValueGeneration.Internal
{
    public interface IActianValueGeneratorCache : IValueGeneratorCache
    {
        ActianSequenceValueGeneratorState GetOrAddSequenceState(
            [NotNull] IProperty property,
            [NotNull] IRelationalConnection connection);
    }
}
