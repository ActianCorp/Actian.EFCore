// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace Actian.EFCore.Update.Internal
{
    public class ActianModificationCommandBatch : AffectedCountModificationCommandBatch
    {
        private const int DefaultNetworkPacketSizeBytes = 4096;
        private const int MaxScriptLength = 65536 * DefaultNetworkPacketSizeBytes / 2;
        private const int MaxParameterCount = 2100 - 2;

        private readonly List<IReadOnlyModificationCommand> _pendingBulkInsertCommands = new();

        public ActianModificationCommandBatch(
        ModificationCommandBatchFactoryDependencies dependencies,
        int maxBatchSize)
        : base(dependencies, maxBatchSize)
        {
        }

        protected new virtual IActianUpdateSqlGenerator UpdateSqlGenerator
            => (IActianUpdateSqlGenerator)base.UpdateSqlGenerator;

        protected override void RollbackLastCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (_pendingBulkInsertCommands.Count > 0)
            {
                _pendingBulkInsertCommands.RemoveAt(_pendingBulkInsertCommands.Count - 1);
            }

            base.RollbackLastCommand(modificationCommand);
        }

        protected override bool IsValid()
        {
            if (ParameterValues.Count > MaxParameterCount)
            {
                return false;
            }

            var sqlLength = SqlBuilder.Length;

            if (_pendingBulkInsertCommands.Count > 0)
            {
                // Conservative heuristic for the length of the pending bulk insert commands.
                // See EXEC sp_server_info.
                var numColumns = _pendingBulkInsertCommands[0].ColumnModifications.Count;

                sqlLength +=
                    numColumns * 128 // column name lengths
                    + 128 // schema name length
                    + 128 // table name length
                    + _pendingBulkInsertCommands.Count * numColumns * 6 // column parameter placeholders
                    + 300; // some extra fixed overhead
            }

            return sqlLength < MaxScriptLength;
        }

        private void ApplyPendingBulkInsertCommands()
        {
            if (_pendingBulkInsertCommands.Count == 0)
            {
                return;
            }

            var commandPosition = ResultSetMappings.Count;

            var wasCachedCommandTextEmpty = IsCommandTextEmpty;

            var resultSetMapping = UpdateSqlGenerator.AppendBulkInsertOperation(
                SqlBuilder, _pendingBulkInsertCommands, commandPosition, out var requiresTransaction);

            SetRequiresTransaction(!wasCachedCommandTextEmpty || requiresTransaction);

            for (var i = 0; i < _pendingBulkInsertCommands.Count; i++)
            {
                ResultSetMappings.Add(resultSetMapping);
            }

            // All result mappings are marked as "not last", mark the last one as "last".
            if (resultSetMapping.HasFlag(ResultSetMapping.HasResultRow))
            {
                ResultSetMappings[^1] &= ~ResultSetMapping.NotLastInResultSet;
                ResultSetMappings[^1] |= ResultSetMapping.LastInResultSet;
            }
        }

        public override bool TryAddCommand(IReadOnlyModificationCommand modificationCommand)
        {
            // If there are any pending bulk insert commands and the new command is incompatible with them (not an insert, insert into a
            // separate table..), apply the pending commands.
            if (_pendingBulkInsertCommands.Count > 0
                && (modificationCommand.EntityState != EntityState.Added
                    || modificationCommand.StoreStoredProcedure is not null
                    || !CanBeInsertedInSameStatement(_pendingBulkInsertCommands[0], modificationCommand)))
            {
                ApplyPendingBulkInsertCommands();
                _pendingBulkInsertCommands.Clear();
            }

            return base.TryAddCommand(modificationCommand);
        }

        protected override void AddCommand(IReadOnlyModificationCommand modificationCommand)
        {
            // TryAddCommand above already applied any pending commands if the new command is incompatible with them.
            // So if the new command is an insert, just append it to pending, otherwise do the regular add logic.
            if (modificationCommand is { EntityState: EntityState.Added, StoreStoredProcedure: null })
            {
                _pendingBulkInsertCommands.Add(modificationCommand);
                AddParameters(modificationCommand);
            }
            else
            {
                base.AddCommand(modificationCommand);
            }
        }

        private static bool CanBeInsertedInSameStatement(
            IReadOnlyModificationCommand firstCommand,
            IReadOnlyModificationCommand secondCommand)
            => firstCommand.TableName == secondCommand.TableName
                && firstCommand.Schema == secondCommand.Schema
                && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        public override void Complete(bool moreBatchesExpected)
        {
            ApplyPendingBulkInsertCommands();

            base.Complete(moreBatchesExpected);
        }

        public override void Execute(IRelationalConnection connection)
        {
            try
            {
                base.Execute(connection);
            }
            catch (DbUpdateException e) 
            {
                throw new DbUpdateException(
                    "Exception: ",
                    e.InnerException,
                    e.Entries);
            }
        }

        public override async Task ExecuteAsync(
    IRelationalConnection connection,
    CancellationToken cancellationToken = default)
        {
            try
            {
                await base.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(
                    "Exception: ",
                    e.InnerException,
                    e.Entries);
            }
        }
    }
}
