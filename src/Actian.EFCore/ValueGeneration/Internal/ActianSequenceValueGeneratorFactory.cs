// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.Update.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Actian.EFCore.ValueGeneration.Internal
{
    public class ActianSequenceValueGeneratorFactory : IActianSequenceValueGeneratorFactory
    {
        private readonly IActianUpdateSqlGenerator _sqlGenerator;

        public ActianSequenceValueGeneratorFactory(
            [NotNull] IActianUpdateSqlGenerator sqlGenerator)
        {
            _sqlGenerator = sqlGenerator;
        }

        public virtual ValueGenerator TryCreate(
            IProperty property,
            Type type,
            ActianSequenceValueGeneratorState generatorState,
            IActianConnection connection,
            IRawSqlCommandBuilder rawSqlCommandBuilder,
            IRelationalCommandDiagnosticsLogger commandLogger)
        {
            if (type == typeof(long))
            {
                return new ActianSequenceHiLoValueGenerator<long>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(int))
            {
                return new ActianSequenceHiLoValueGenerator<int>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(decimal))
            {
                return new ActianSequenceHiLoValueGenerator<decimal>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(short))
            {
                return new ActianSequenceHiLoValueGenerator<short>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(byte))
            {
                return new ActianSequenceHiLoValueGenerator<byte>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(char))
            {
                return new ActianSequenceHiLoValueGenerator<char>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(ulong))
            {
                return new ActianSequenceHiLoValueGenerator<ulong>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(uint))
            {
                return new ActianSequenceHiLoValueGenerator<uint>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(ushort))
            {
                return new ActianSequenceHiLoValueGenerator<ushort>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            if (type == typeof(sbyte))
            {
                return new ActianSequenceHiLoValueGenerator<sbyte>(
                    rawSqlCommandBuilder, _sqlGenerator, generatorState, connection, commandLogger);
            }

            throw new ArgumentException(
                CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(ActianSequenceValueGeneratorFactory), property.Name, type.FullName));
        }
    }
}
