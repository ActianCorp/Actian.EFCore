// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Actian.EFCore.TestUtilities
{
    internal static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseActianTestLoggerFactory(this DbContextOptionsBuilder builder, ILoggerFactory loggerFactory)
        {
            return builder
                .ConfigureActianTestWarnings()
                .UseLoggerFactory(loggerFactory);
        }

        public static DbContextOptionsBuilder ConfigureActianTestWarnings(this DbContextOptionsBuilder builder) => builder.ConfigureWarnings(b => b.Log(
            (RelationalEventId.ConnectionOpening, LogLevel.Information),
            (RelationalEventId.ExplicitTransactionEnlisted, LogLevel.Information),
            (RelationalEventId.DataReaderDisposing, LogLevel.Information),
            (RelationalEventId.MigrateUsingConnection, LogLevel.Information),
            (RelationalEventId.MigrationReverting, LogLevel.Information),
            (RelationalEventId.MigrationApplying, LogLevel.Information),
            (RelationalEventId.MigrationGeneratingDownScript, LogLevel.Information),
            (RelationalEventId.MigrationGeneratingUpScript, LogLevel.Information),
            (RelationalEventId.MigrationsNotApplied, LogLevel.Information),
            (RelationalEventId.MigrationsNotFound, LogLevel.Information),
            (RelationalEventId.MigrationAttributeMissingWarning, LogLevel.Information),
            (RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning, LogLevel.Information),
            (RelationalEventId.ModelValidationKeyDefaultValueWarning, LogLevel.Information),
            (RelationalEventId.BoolWithDefaultWarning, LogLevel.Information),
            (RelationalEventId.AmbientTransactionEnlisted, LogLevel.Information),
            (RelationalEventId.AmbientTransactionWarning, LogLevel.Information),
            (RelationalEventId.TransactionError, LogLevel.Information),
            (RelationalEventId.TransactionDisposed, LogLevel.Information),
            (RelationalEventId.ConnectionOpened, LogLevel.Information),
            (RelationalEventId.ConnectionClosing, LogLevel.Information),
            (RelationalEventId.ConnectionClosed, LogLevel.Information),
            (RelationalEventId.ConnectionError, LogLevel.Information),
            (RelationalEventId.CommandCreating, LogLevel.Information),
            (RelationalEventId.CommandCreated, LogLevel.Information),
            (RelationalEventId.CommandExecuting, LogLevel.Information),
            (RelationalEventId.BatchReadyForExecution, LogLevel.Information),
            (RelationalEventId.CommandExecuted, LogLevel.Information),
            (RelationalEventId.TransactionStarted, LogLevel.Information),
            (RelationalEventId.TransactionStarting, LogLevel.Information),
            (RelationalEventId.TransactionUsed, LogLevel.Information),
            (RelationalEventId.TransactionCommitting, LogLevel.Information),
            (RelationalEventId.TransactionCommitted, LogLevel.Information),
            (RelationalEventId.TransactionRollingBack, LogLevel.Information),
            (RelationalEventId.TransactionRolledBack, LogLevel.Information),
            (RelationalEventId.CommandError, LogLevel.Information),
            (RelationalEventId.BatchSmallerThanMinBatchSize, LogLevel.Information)
        ));
    }
}
