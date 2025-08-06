// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq;

namespace Actian.EFCore.TestUtilities
{
    internal interface ISetSource
    {
        IQueryable<TEntity> Set<TEntity>()
            where TEntity : class;
    }
}
