// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿namespace Actian.EFCore
{
    /// <summary>
    ///     Indicates type of data compression used on a index.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public enum DataCompressionType
    {
        /// <summary>
        ///     Index is not compressed.
        /// </summary>
        None,

        /// <summary>
        ///     Index is compressed by using row compression.
        /// </summary>
        Row,

        /// <summary>
        ///     Index is compressed by using page compression.
        /// </summary>
        Page
    }
}
