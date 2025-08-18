// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Actian.EFCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

#nullable enable

namespace Actian.EFCore.ValueGeneration.Internal
{
    public interface IActianSequenceValueGeneratorFactory
    {
        ValueGenerator? TryCreate(
            IProperty property,
            Type clrType,
            ActianSequenceValueGeneratorState generatorState,
            IActianConnection connection,
            IRawSqlCommandBuilder rawSqlCommandBuilder,
            IRelationalCommandDiagnosticsLogger commandLogger);
    }
}
