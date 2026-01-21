// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.EFCore.TestUtilities
{
    [Flags]
    public enum ActianCompatibility
    {
        Unknown = 0,
        Ingres = 1,
        Ansi = 2
    }

    public static class ActianCompatibilityExtensions
    {
        public static string AsString(this ActianCompatibility compatibility)
        {
            return compatibility switch
            {
                ActianCompatibility.Ingres => "Ingres",
                ActianCompatibility.Ansi => "ANSI/ISO Entry SQL-92",
                ActianCompatibility.Ingres | ActianCompatibility.Ansi => "Ingres or ANSI/ISO Entry SQL-92",
                _ => ""
            };
        }
    }
}
