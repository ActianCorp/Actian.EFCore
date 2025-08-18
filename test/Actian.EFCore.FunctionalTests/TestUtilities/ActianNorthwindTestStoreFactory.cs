// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Actian.EFCore.TestUtilities
{
    public class ActianNorthwindTestStoreFactory : ActianTestStoreFactory
    {
        public const string Name = "Northwind";
        public static readonly string NorthwindConnectionString = TestEnvironment.GetConnectionString(Name);
        public static new ActianNorthwindTestStoreFactory Instance { get; } = new ActianNorthwindTestStoreFactory();

        protected ActianNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => ActianTestStore.GetOrCreate(Name, "Northwind.sql");
    }
}
