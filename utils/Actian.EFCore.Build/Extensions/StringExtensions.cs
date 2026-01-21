// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq;

namespace Actian.EFCore.Build
{
    public static class StringExtensions
    {
        public static string Repeat(this string str, int count)
        {
            if (count <= 0)
                return "";
            return string.Concat(Enumerable.Repeat(str, count));
        }
    }
}
