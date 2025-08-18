// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;

namespace Actian.EFCore.TestGenerators
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                    return index;
                index += 1;
            }
            return -1;
        }
    }
}
