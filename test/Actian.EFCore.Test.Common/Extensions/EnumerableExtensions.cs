// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;

namespace Actian.EFCore.TestUtilities
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

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            static IEnumerable<TSource> batch(IEnumerator<TSource> source, int size)
            {
                yield return source.Current;
                for (var i = 1; i < size && source.MoveNext(); i++)
                    yield return source.Current;
            }

            using var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return batch(enumerator, size);
            }
        }
    }
}
