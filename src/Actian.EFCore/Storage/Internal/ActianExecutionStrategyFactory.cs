// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianExecutionStrategyFactory : RelationalExecutionStrategyFactory
    {
        public ActianExecutionStrategyFactory(
            [NotNull] ExecutionStrategyDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
            => new ActianExecutionStrategy(dependencies);
    }
}
