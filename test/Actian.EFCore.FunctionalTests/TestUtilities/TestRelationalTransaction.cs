// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.TestUtilities
{
    public class TestRelationalTransactionFactory : IRelationalTransactionFactory
    {
        public RelationalTransaction Create(
            IRelationalConnection connection,
            DbTransaction transaction,
            Guid transactionId,
            IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
            bool transactionOwned)
            => new TestRelationalTransaction(connection, transaction, logger, transactionOwned);
    }

    public class TestRelationalTransaction : RelationalTransaction
    {
        private readonly TestActianConnection _testConnection;

        public TestRelationalTransaction(
            IRelationalConnection connection,
            DbTransaction transaction,
            IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
            bool transactionOwned)
            : base(connection, transaction, new Guid(), logger, transactionOwned, null)
        {
            _testConnection = (TestActianConnection)connection;
        }

        public override void Commit()
        {
            if (_testConnection.CommitFailures.Count > 0)
            {
                var fail = _testConnection.CommitFailures.Dequeue();
                if (fail.HasValue)
                {
                    if (fail.Value)
                    {
                        this.GetDbTransaction().Rollback();
                    }
                    else
                    {
                        this.GetDbTransaction().Commit();
                    }

                    _testConnection.DbConnection.Close();
                    throw IngresExceptionFactory.CreateIngresException(_testConnection.ErrorNumber, _testConnection.ConnectionId);
                }
            }

            base.Commit();
        }
    }
}
