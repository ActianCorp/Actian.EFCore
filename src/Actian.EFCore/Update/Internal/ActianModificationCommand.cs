// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Actian.EFCore.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace Actian.EFCore.Update.Internal;

public class ActianModificationCommand : ModificationCommand
{
    public ActianModificationCommand(in ModificationCommandParameters modificationCommandParameters)
        : base(modificationCommandParameters)
    {
    }

    public ActianModificationCommand(in NonTrackedModificationCommandParameters modificationCommandParameters)
        : base(modificationCommandParameters)
    {
    }

    protected override void ProcessSinglePropertyJsonUpdate(ref ColumnModificationParameters parameters)
    {
        var property = parameters.Property!;
        var mapping = property.GetRelationalTypeMapping();
        var propertyProviderClrType = (mapping.Converter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
        var value = parameters.Value;

        // JSON-compatible non-string values (bool, numeric, null) are sent directly as non-string parameters.
        if (value is null
            || ((propertyProviderClrType == typeof(bool)
                || propertyProviderClrType.IsNumeric())
                && !property.IsPrimitiveCollection))
        {
            parameters = parameters with { Value = value, TypeMapping = mapping };

            return;
        }

        var jsonValueReaderWriter = mapping.JsonValueReaderWriter;
        if (jsonValueReaderWriter != null)
        {
            if (property.IsPrimitiveCollection)
            {
                // This is a JSON array, so send with the original type mapping, which may indicate the column type is JSON.
                parameters = parameters with { Value = jsonValueReaderWriter.ToJsonString(value) };

                return;
            }

            // Actian stores JSON as plain nvarchar strings; wrap the value in a simple JSON object
            // to avoid double escaping, and use the default unicode string type mapping.
            parameters = parameters with
            {
                Value = jsonValueReaderWriter.ToJsonObjectString("", value),
                TypeMapping = ActianStringTypeMapping.UnicodeDefault
            };

            return;
        }
        else if (mapping.Converter != null)
        {
            value = mapping.Converter.ConvertToProvider(value);
        }

        parameters = parameters with
        {
            Value = value,
            TypeMapping = ActianStringTypeMapping.UnicodeDefault
        };
    }
}
