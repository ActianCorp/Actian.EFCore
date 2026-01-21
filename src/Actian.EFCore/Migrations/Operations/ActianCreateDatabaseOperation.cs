// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#nullable enable
namespace Actian.EFCore.Migrations.Operations
{
    /// <summary>
    /// An Actian-specific <see cref="MigrationOperation" /> to create a database.
    /// </summary>
    public class ActianCreateDatabaseOperation : MigrationOperation
    {
        /// <summary>
        /// The name of the database.
        /// </summary>
        public virtual string? Name { get; [param: NotNull] set; }
    }
}
