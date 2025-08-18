// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Actian.EFCore.TestUtilities
{
    public static class ActianDbContextOptionsBuilderExtensions
    {
        public static ActianDbContextOptionsBuilder ApplyConfiguration(this ActianDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);

            optionsBuilder.ExecutionStrategy(d => new TestActianRetryingExecutionStrategy(d));

            optionsBuilder.CommandTimeout(ActianTestStore.CommandTimeout);

            return optionsBuilder;
        }
    }
}
