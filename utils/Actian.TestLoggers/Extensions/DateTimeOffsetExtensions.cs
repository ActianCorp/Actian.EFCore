// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.TestLoggers
{
    public static class DateTimeOffsetExtensions
    {
        public static string Format(this DateTimeOffset value)
            => value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm UTC");

        public static string Format(this DateTimeOffset? value)
            => value is null ? "" : value.Value.Format();

        public static string Format(this TimeSpan value)
        {
            if (value == TimeSpan.Zero)
                return "0s";

            if (value.TotalDays >= 1)
            {
                return value.Hours >= 1
                    ? $"{value.TotalDays:0}d {value.Hours:0}h"
                    : $"{value.TotalDays:0}d";
            }

            if (value.TotalHours >= 1)
            {
                return value.Minutes >= 1
                    ? $"{value.TotalHours:0}h {value.Minutes:0}m"
                    : $"{value.TotalHours:0}h";
            }

            if (value.TotalMinutes >= 1)
            {
                return value.Seconds >= 1
                    ? $"{value.TotalMinutes:0}m {value.Seconds:0}s"
                    : $"{value.TotalMinutes:0}m";
            }

            if (value.TotalSeconds >= 9.5)
                return $"{Math.Round(value.TotalSeconds):0}s";

            if (value.TotalSeconds >= 1)
            {
                return value.TotalMilliseconds >= 1
                    ? $"{value.TotalSeconds:0}s {value.Milliseconds:0}ms"
                    : $"{value.TotalSeconds:0}s";
            }

            if (value.Milliseconds >= 1)
                return $"{value.Milliseconds:0}ms";

            return $"&lt;1ms";
        }
    }
}
