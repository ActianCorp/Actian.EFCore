// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Globalization;

namespace Actian.EFCore.Extensions
{
    public static class ObjectExtensions
    {
        public static T ChangeType<T>(this object value)
        {
            if (value is null)
            {
                if (!typeof(T).IsNullableType())
                {
                    throw new InvalidCastException($"null can not be cast to {typeof(T)}");
                }
                return default;
            }

            return value switch
            {
                T typedValue => typedValue,
                IConvertible _ => (T)Convert.ChangeType(value, typeof(T).UnwrapNullableType(), CultureInfo.InvariantCulture),
                _ => (T)value,
            };
        }
    }
}
