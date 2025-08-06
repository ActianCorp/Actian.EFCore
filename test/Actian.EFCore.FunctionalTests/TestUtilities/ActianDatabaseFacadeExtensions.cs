// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Actian.EFCore.TestUtilities
{
    public static class ActianDatabaseFacadeExtensions
    {
        public static void EnsureClean(this DatabaseFacade databaseFacade)
            => databaseFacade.CreateExecutionStrategy()
                .Execute(databaseFacade, database => new ActianDatabaseCleaner().Clean(database));
    }
}
