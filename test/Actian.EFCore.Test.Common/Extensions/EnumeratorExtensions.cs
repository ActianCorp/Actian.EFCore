// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Actian.EFCore.TestUtilities
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
        {
            return new EnumeratorEnumerable<T>(enumerator);
        }

        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            return enumerator.AsEnumerable().ToList();
        }

        public static T[] ToArray<T>(this IEnumerator<T> enumerator)
        {
            return enumerator.AsEnumerable().ToArray();
        }

        private class EnumeratorEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerator<T> _enumerator;
            public EnumeratorEnumerable(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
