// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Actian.EFCore.TestUtilities
{
    internal static class ActianModelTestHelpers
    {
        public static readonly ActianEntityPropertyNormalizer<string> MaxLengthStringKeys = new ActianEntityPropertyNormalizer<string>()
            .Where(property =>
            {
                if (!property.IsPrimaryKey() && !property.IsKey() && !property.IsForeignKey() && !property.IsIndex())
                    return false;

                if (property.GetMaxLength().HasValue)
                    return false;

                return true;
            })
            .SetMaxLength(30);

        public static readonly ActianEntityPropertyNormalizer<byte[]> MaxLengthBinaryKeys = new ActianEntityPropertyNormalizer<byte[]>()
            .Where(property =>
            {
                if (!property.IsPrimaryKey() && !property.IsKey() && !property.IsForeignKey() && !property.IsIndex())
                    return false;

                if (property.GetMaxLength().HasValue)
                    return false;

                return true;
            })
            .SetMaxLength(30);

        public static readonly ActianEntityPropertyNormalizer<Guid> Guids = new ActianEntityPropertyNormalizer<Guid>()
            .SetColumnType("char(36)")
            .SetValueConverter(new GuidToStringConverter());


        public static void SetColumnType(IMutableProperty property, string value)
        {
            property.SetOrRemoveAnnotation(RelationalAnnotationNames.ColumnType, value);
        }

        public static string GetColumnType(IMutableProperty property)
        {
            return property.FindAnnotation(RelationalAnnotationNames.ColumnType)?.Value as string;
        }
    }
}
