// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Threading.Tasks;

namespace Actian.EFCore.Build
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await new BuildContext().Run(args);
        }
    }
}
