// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Actian.EFCore.Query
{
    public class TPHInheritanceQueryActianFixture : TPHInheritanceQueryFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => ActianTestStoreFactory.Instance;
    }
}
