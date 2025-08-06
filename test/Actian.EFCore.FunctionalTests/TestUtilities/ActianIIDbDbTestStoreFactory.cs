// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Actian.EFCore.TestUtilities
{
    public class ActianIIDbDbTestStoreFactory : ActianTestStoreFactory
    {
        public const string Name = "iidbdb";
        public static readonly string NorthwindConnectionString = TestEnvironment.GetConnectionString(Name);
        public static new ActianIIDbDbTestStoreFactory Instance { get; } = new ActianIIDbDbTestStoreFactory();

        protected ActianIIDbDbTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => ActianTestStore.GetOrCreateWithUser(Name, TestEnvironment.LoginUser);
    }
}
