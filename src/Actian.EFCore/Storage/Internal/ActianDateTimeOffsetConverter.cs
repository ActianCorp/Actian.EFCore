// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
    {
        public ActianDateTimeOffsetConverter()
            : this(null)
        {
        }

        public ActianDateTimeOffsetConverter([CanBeNull] ConverterMappingHints mappingHints)
            : base(
                value => ConvertFromDateTimeOffset(value),
                value => ConvertToDateTimeOffset(value),
                mappingHints
            )
        {
        }

        public static DateTime ConvertFromDateTimeOffset(DateTimeOffset value)
        {
            if (value.Offset.Equals(TimeSpan.Zero))
                return value.UtcDateTime;
            else if (value.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(value.DateTime)))
                return DateTime.SpecifyKind(value.DateTime, DateTimeKind.Local);
            else
                return value.DateTime;
        }

        public static DateTimeOffset ConvertToDateTimeOffset(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
                value = DateTime.SpecifyKind(value, DateTimeKind.Local);
            return value;
        }
    }
}
