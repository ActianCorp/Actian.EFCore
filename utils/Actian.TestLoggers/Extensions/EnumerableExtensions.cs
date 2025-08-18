// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Actian.TestLoggers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] items)
        {
            return source.Concat(items.AsEnumerable());
        }

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

        public static IEnumerable<TSource> SkipLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var skipBuffer = new List<TSource>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    skipBuffer.Add(item);
                }
                else
                {
                    if (skipBuffer.Count > 0)
                    {
                        foreach (var skipped in skipBuffer)
                            yield return skipped;
                        skipBuffer.Clear();
                    }
                    yield return item;
                }
            }
        }

        public static IEnumerable<TSource> Trim<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => source.SkipWhile(predicate).SkipLastWhile(predicate);
    }
}
