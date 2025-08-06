// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data.Common;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Infrastructure.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable enable

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Actian specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class ActianDbContextOptionsExtensions
    {
        /// <summary>
        /// Configures the context to connect to an Actian database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="actianOptionsAction">An optional action to allow additional Actian specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseActian(
            this DbContextOptionsBuilder optionsBuilder,
            string? connectionString,
            Action<ActianDbContextOptionsBuilder>? actianOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var extension = (ActianOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureWarnings(optionsBuilder);

            actianOptionsAction?.Invoke(new ActianDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        // Note: Decision made to use DbConnection not SqlConnection: Issue #772
        /// <summary>
        /// Configures the context to connect to an Actian database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        /// An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        /// in the open state then EF will not open or close the connection. If the connection is in the closed
        /// state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="actianOptionsAction">An optional action to allow additional Actian specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseActian(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<ActianDbContextOptionsBuilder>? actianOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var extension = (ActianOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureWarnings(optionsBuilder);

            actianOptionsAction?.Invoke(new ActianDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        /// Configures the context to connect to an Actian database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="actianOptionsAction">An optional action to allow additional Actian specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseActian<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<ActianDbContextOptionsBuilder>? actianOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseActian(
                (DbContextOptionsBuilder)optionsBuilder, connectionString, actianOptionsAction);

        // Note: Decision made to use DbConnection not SqlConnection: Issue #772
        /// <summary>
        /// Configures the context to connect to an Actian database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        /// An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        /// in the open state then EF will not open or close the connection. If the connection is in the closed
        /// state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="actianOptionsAction">An optional action to allow additional Actian specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseActian<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<ActianDbContextOptionsBuilder>? actianOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseActian(
                (DbContextOptionsBuilder)optionsBuilder, connection, actianOptionsAction);

        private static ActianOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<ActianOptionsExtension>()
                ?? new ActianOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
