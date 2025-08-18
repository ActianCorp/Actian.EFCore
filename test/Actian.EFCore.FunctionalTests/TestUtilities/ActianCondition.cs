// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.EFCore.TestUtilities
{
    [Flags]
    public enum ActianCondition
    {
        IsSqlAzure = 1 << 0,
        IsNotSqlAzure = 1 << 1,
        SupportsMemoryOptimized = 1 << 2,
        SupportsAttach = 1 << 3,
        SupportsHiddenColumns = 1 << 4,
        IsNotCI = 1 << 5,
        SupportsFullTextSearch = 1,
        SupportsOnlineIndexes = 1 << 7,
        SupportsTemporalTablesCascadeDelete = 1 << 8,
        SupportsUtf8 = 1 << 9,
        SupportsFunctions2019 = 1 << 10,
        SupportsFunctions2017 = 1 << 11,
        SupportsJsonPathExpressions = 1 << 12,
        Todo,
        BooleanExpession = 1 << 13,
    }
}
