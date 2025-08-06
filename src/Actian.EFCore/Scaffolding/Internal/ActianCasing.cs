// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.EFCore.Scaffolding.Internal
{
    public enum ActianCasing
    {
        Lower,
        Upper,
        Mixed
    }

    internal static class ActianCasingExtensions
    {
        public static ActianCasing ToActianCasing(this string value)
        {
            if (value is null)
                throw new Exception($@"Could not convert null to an ActianCasing value");

            switch (value?.ToUpperInvariant())
            {
                case "LOWER": return ActianCasing.Lower;
                case "UPPER": return ActianCasing.Upper;
                case "MIXED": return ActianCasing.Mixed;
                default:
                    throw new Exception($@"Could not convert ""${value}"" to an ActianCasing value");
            }
        }

        public static string Normalize(this ActianCasing casing, string value) => casing switch
        {
            ActianCasing.Lower => value?.ToLower(),
            ActianCasing.Upper => value?.ToUpper(),
            _ => value
        };
    }
}
