// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Actian.EFCore.Extensions;

namespace Actian.EFCore
{
    public static class DbCommandExtensions
    {
        public static T ExecuteScalar<T>(this DbCommand command)
        {
            return command.ExecuteScalar().ChangeType<T>();
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbCommand command, CancellationToken cancellationToken = default)
        {
            var value = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            return value.ChangeType<T>();
        }
    }
}
