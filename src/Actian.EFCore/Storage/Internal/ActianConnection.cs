// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Concurrent;
using System.Data.Common;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianConnection : RelationalConnection, IActianConnection
    {
        // Compensate for slow Actian database creation (when and if database creation is implemented)
        private const int DefaultIIDbDbConnectionCommandTimeout = 60 * 5;

        private static readonly ConcurrentDictionary<string, bool> MultipleActiveResultSetsEnabledMap = new();

        public ActianConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override DbConnection CreateDbConnection() => new IngresConnection(ConnectionString);

        /// <inheritdoc />
        public virtual IActianConnection CreateIIDbDbConnection()
        {
            var connectionString = new IngresConnectionStringBuilder(ConnectionString)
            {
                Database = "iidbdb",
                DbmsUser = "ingres",
                Pooling = false
            }.ToString();

            var contextOptions = new DbContextOptionsBuilder()
                .UseActian(
                    connectionString,
                    b => b.CommandTimeout(CommandTimeout ?? DefaultIIDbDbConnectionCommandTimeout))
                .Options;

            return new ActianConnection(Dependencies with { ContextOptions = contextOptions });
        }

        protected override bool SupportsAmbientTransactions => true;

        public override IDbContextTransaction UseTransaction(DbTransaction transaction)
        {
            return base.UseTransaction(transaction);
        }

        public virtual bool IsMultipleActiveResultSetsEnabled
        {
            get
            {
                return false;
            }
        }
    }
}
