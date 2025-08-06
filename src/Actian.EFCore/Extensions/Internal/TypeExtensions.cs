// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.EFCore.Extensions.Internal
{
    internal static class TypeExtensions
    {
        public static long MinLongValue(this Type type)
        {
            if (type == typeof(int))
                return int.MinValue;
            if (type == typeof(long))
                return long.MinValue;
            if (type == typeof(short))
                return short.MinValue;
            throw new ArgumentOutOfRangeException();
        }

        public static long MaxLongValue(this Type type)
        {
            if (type == typeof(int))
                return int.MaxValue;
            if (type == typeof(long))
                return long.MaxValue;
            if (type == typeof(short))
                return short.MaxValue;
            throw new ArgumentOutOfRangeException();
        }
    }
}
