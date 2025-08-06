// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿namespace Actian.EFCore.Parsing.Internal
{
    public enum ActianSqlTokenType
    {
        NewLine,
        WhiteSpace,
        LineComment,
        BlockComment,
        Word,
        Number,
        String,
        Symbol,
        Semicolon,
        Command,
        Unknown
    }
}
