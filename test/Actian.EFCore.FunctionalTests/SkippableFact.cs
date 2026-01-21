// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Xunit;

namespace Actian.EFCore
{
    public class SkippableFactAttribute: FactAttribute
    {
        public SkippableFactAttribute()
        {
            if (true)
            {
                Skip = "Ignored because it takes too long to complete...";
            }
        }
    }
}
