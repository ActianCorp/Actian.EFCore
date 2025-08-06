// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Actian.EFCore.Utilities
{
    internal static class DisposableExtensions
    {
        public static ValueTask DisposeAsyncIfAvailable([CanBeNull] this IDisposable disposable)
        {
            if (disposable != null)
            {
                if (disposable is IAsyncDisposable asyncDisposable)
                {
                    return asyncDisposable.DisposeAsync();
                }

                disposable.Dispose();
            }

            return default;
        }
    }
}
