// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    /// <inheritdoc />
    public interface IActianConnection : IRelationalConnection
    {
        /// <summary>
        /// Creates a new connection to the iidbdb database for the current connection
        /// </summary>
        /// <returns>A new connection to the iidbdb database for the current connection</returns>
        IActianConnection CreateIIDbDbConnection();

        bool IsMultipleActiveResultSetsEnabled { get; }
    }
}
