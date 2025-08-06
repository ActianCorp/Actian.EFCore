// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Actian.EFCore.Infrastructure.Internal
{
    public interface IActianOptions : ISingletonOptions
    {
        ///// <summary>
        ///// Reflects the option set by <see cref="ActianDbContextOptionsBuilder.UseRowNumberForPaging" />.
        ///// </summary>
        //bool RowNumberPagingEnabled { get; }
    }
}
