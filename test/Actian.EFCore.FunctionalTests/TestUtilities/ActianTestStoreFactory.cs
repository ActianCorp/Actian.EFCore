// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Actian.EFCore.TestUtilities
{
    public class ActianTestStoreFactory : RelationalTestStoreFactory
    {
        public static ActianTestStoreFactory Instance { get; } = new ();

        protected ActianTestStoreFactory()
        {
        }

        public override TestStore Create(string storeName)
            => ActianTestStore.Create(storeName);

        public override TestStore GetOrCreate(string storeName)
            => ActianTestStore.GetOrCreate(storeName);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkActian();
    }
}
