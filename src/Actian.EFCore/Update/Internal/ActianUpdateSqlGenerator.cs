// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using Actian.EFCore.Extensions;
using Actian.EFCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

#nullable enable

namespace Actian.EFCore.Update.Internal
{

    public class ActianUpdateSqlGenerator : UpdateAndSelectSqlGenerator, IActianUpdateSqlGenerator
    {
        public ActianUpdateSqlGenerator(
            UpdateSqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     The minimum number of insertions which are executed using MERGE ... OUTPUT INTO. Below this threshold, multiple batched INSERT
        ///     statements are more efficient.
        /// </summary>
        protected virtual int MergeIntoMinimumThreshold
            => 4;

        public override ResultSetMapping AppendInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            // Filter out table_key columns before processing
            var filteredCommand = FilterSystemMaintainedColumns(command);
            return AppendInsertAndSelectOperation(commandStringBuilder, filteredCommand, commandPosition, out requiresTransaction);
        }

        /// <summary>
        /// Filters out system-maintained columns (like table_key) from the modification command
        /// </summary>
        private IReadOnlyModificationCommand FilterSystemMaintainedColumns(IReadOnlyModificationCommand command)
        {
            // Check if any columns need to be filtered
            var hasSystemMaintainedColumns = command.ColumnModifications.Any(IsSystemMaintainedColumn);
            
            if (!hasSystemMaintainedColumns)
            {
                // No filtering needed, return original command
                return command;
            }

            // Create a filtered modification command wrapper
            return new FilteredModificationCommand(command, IsSystemMaintainedColumn);
        }

        /// <summary>
        /// Checks if a column is system-maintained (table_key type)
        /// </summary>
        private bool IsSystemMaintainedColumn(IColumnModification columnModification)
        {
            var property = columnModification.Property;
            if (property != null)
            {
                // Get the column type configured in the model
                var storeType = property.GetColumnType();
                
                // Check for table_key data type (case-insensitive)
                // The type might include length like "table_key(8)", so use StartsWith
                if (storeType != null && storeType.StartsWith("table_key", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Wrapper class to filter out system-maintained columns from a modification command
        /// </summary>
        private class FilteredModificationCommand : IReadOnlyModificationCommand
        {
            private readonly IReadOnlyModificationCommand _innerCommand;
            private readonly Func<IColumnModification, bool> _shouldExclude;
            private readonly IReadOnlyList<IColumnModification> _filteredColumnModifications;

            public FilteredModificationCommand(
                IReadOnlyModificationCommand innerCommand,
                Func<IColumnModification, bool> shouldExclude)
            {
                _innerCommand = innerCommand;
                _shouldExclude = shouldExclude;
                
                // Filter out system-maintained columns from write operations
                _filteredColumnModifications = innerCommand.ColumnModifications
                    .Select(cm => 
                    {
                        if (cm.IsWrite && shouldExclude(cm))
                        {
                            // Create a wrapper that marks this column as not-write
                            return new FilteredColumnModification(cm, isWrite: false);
                        }
                        return cm;
                    })
                    .ToList();
            }

            public string TableName => _innerCommand.TableName;
            public string? Schema => _innerCommand.Schema;
            public IReadOnlyList<IColumnModification> ColumnModifications => _filteredColumnModifications;
            public EntityState EntityState => _innerCommand.EntityState;
            // Forward other members that exist on the IReadOnlyModificationCommand interface in this EF Core version
            public IReadOnlyList<IUpdateEntry> Entries => _innerCommand.Entries;
            public IStoreStoredProcedure? StoreStoredProcedure => _innerCommand.StoreStoredProcedure;
            public IColumnBase? RowsAffectedColumn => _innerCommand.RowsAffectedColumn;
            public ITable? Table => _innerCommand.Table;
            
            public void PropagateResults(RelationalDataReader relationalReader)
                => _innerCommand.PropagateResults(relationalReader);
            
            public void PropagateOutputParameters(DbParameterCollection parameterCollection, int baseParameterIndex)
                => _innerCommand.PropagateOutputParameters(parameterCollection, baseParameterIndex);

            // Wrapper for column modifications to override IsWrite property
            private class FilteredColumnModification : IColumnModification
            {
                private readonly IColumnModification _inner;
                private readonly bool _isWrite;

                public FilteredColumnModification(IColumnModification inner, bool isWrite)
                {
                    _inner = inner;
                    _isWrite = isWrite;
                }

                public IProperty? Property => _inner.Property;
                public IColumnBase? Column => _inner.Column;
                public string ColumnName => _inner.ColumnName;
                public string? ColumnType => _inner.ColumnType;
                public bool? IsNullable => _inner.IsNullable;

                // These interface members have setters on newer EF Core versions; forward to inner for getter and no-op for setter
                public bool IsKey
                {
                    get => _inner.IsKey;
                    set { /* no-op: we don't need to change this on the inner */ }
                }

                public bool IsCondition
                {
                    get => _inner.IsCondition;
                    set { /* no-op */ }
                }

                public bool IsRead
                {
                    get => _inner.IsRead;
                    set { /* no-op */ }
                }

                public bool IsWrite
                {
                    get => _isWrite;
                    set { /* no-op: keep write override */ }
                }

                public object? Value
                {
                    get => _inner.Value;
                    set { /* no-op */ }
                }

                public object? OriginalValue
                {
                    get => _inner.OriginalValue;
                    set { /* no-op */ }
                }

                public string? ParameterName => _inner.ParameterName;
                public string? OriginalParameterName => _inner.OriginalParameterName;
                public bool UseCurrentValue => _inner.UseCurrentValue;
                public bool UseCurrentValueParameter => _inner.UseCurrentValueParameter;
                public bool UseOriginalValue => _inner.UseOriginalValue;
                public bool UseOriginalValueParameter => _inner.UseOriginalValueParameter;
                public bool UseParameter => _inner.UseParameter;
                public string? JsonPath => _inner.JsonPath;

                // Entry on IColumnModification expects IUpdateEntry
                public IUpdateEntry Entry => (IUpdateEntry)_inner.Entry!;

                public void AddSharedColumnModification(IColumnModification sharedColumnModification)
                 => _inner.AddSharedColumnModification(sharedColumnModification);

                public void ResetParameterNames()
                 => _inner.ResetParameterNames();

                // TypeMapping property exists on newer EF Core - use safe dynamic access
                public RelationalTypeMapping? TypeMapping
                {
                    get
                    {
                        try
                        {
                            return (_inner as dynamic)?.TypeMapping as RelationalTypeMapping;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }

        protected override void AppendInsertCommand(
            StringBuilder commandStringBuilder,
            string name,
            string? schema,
            IReadOnlyList<IColumnModification> writeOperations,
            IReadOnlyList<IColumnModification> readOperations)
        {
            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendOutputClause(commandStringBuilder, readOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, name, schema, writeOperations);
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
        }

    private const string InsertedTableBaseName = "@inserted";
    private const string ToInsertTableAlias = "i";
    private const string PositionColumnName = "_Position";
    private const string PositionColumnDeclaration = "[" + PositionColumnName + "] [int]";
    private const string FullPositionColumnName = ToInsertTableAlias + "." + PositionColumnName;

    private ResultSetMapping AppendInsertSingleRowWithOutputInto(
        StringBuilder commandStringBuilder,
        IReadOnlyModificationCommand command,
        IReadOnlyList<IColumnModification> keyOperations,
        IReadOnlyList<IColumnModification> readOperations,
        int commandPosition,
        out bool requiresTransaction)
    {
            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToList();

            AppendDeclareTable(commandStringBuilder, InsertedTableBaseName, commandPosition, keyOperations);

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendOutputIntoClause(commandStringBuilder, keyOperations, InsertedTableBaseName, commandPosition);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, name, schema, writeOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator);

            requiresTransaction = true;

            return AppendSelectCommand(
                commandStringBuilder, readOperations, keyOperations, InsertedTableBaseName, commandPosition, name, schema);
        }

        public virtual ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition,
            out bool requiresTransaction)
        {
            var firstCommand = modificationCommands[0];

            if (modificationCommands.Count == 1)
            {
                return AppendInsertOperation(commandStringBuilder, firstCommand, commandPosition, out requiresTransaction);
            }

            var table = StoreObjectIdentifier.Table(firstCommand.TableName, modificationCommands[0].Schema);

            var readOperations = firstCommand.ColumnModifications.Where(o => o.IsRead).ToList();
            var writeOperations = firstCommand.ColumnModifications.Where(o => o.IsWrite).ToList();
            var keyOperations = firstCommand.ColumnModifications.Where(o => o.IsKey).ToList();

            var writableOperations = modificationCommands[0].ColumnModifications
                .Where(
                    o =>
                        o.Property?.GetValueGenerationStrategy(table) != ActianValueGenerationStrategy.IdentityColumn
                        && o.Property?.GetValueGenerationStrategy(table) != ActianValueGenerationStrategy.IdentityByDefaultColumn
                        && o.Property?.GetComputedColumnSql() is null
                        && o.Property?.GetColumnType() is not "rowversion" and not "timestamp")
                .ToList();

            if (writeOperations.Count == 0)
            {
                // We have no values to write; MERGE and multi-row INSERT cannot be used without writing at least a single column.
                // But as long as there's at least one writable column (non-identity/computed), we can use it to send DEFAULT in a multi-row
                // INSERT.
                if (writableOperations.Count > 0)
                {
                    if (writableOperations.Count > 1)
                    {
                        writableOperations.RemoveRange(1, writableOperations.Count - 1);
                    }

                    return readOperations.Count == 0
                        ? AppendInsertMultipleDefaultRows(
                            commandStringBuilder, modificationCommands, writableOperations, out requiresTransaction)
                        : AppendInsertMultipleDefaultRowsWithOutputInto(
                            commandStringBuilder, modificationCommands, commandPosition, writableOperations, keyOperations, readOperations,
                            out requiresTransaction);
                }

                // There are no writeable columns, fall back to sending multiple single-row INSERTs (there is no way to insert multiple
                // all-default rows in a single INSERT).
                requiresTransaction = modificationCommands.Count > 1;
                foreach (var modification in modificationCommands)
                {
                    AppendInsertOperation(commandStringBuilder, modification, commandPosition++, out var localRequiresTransaction);
                    requiresTransaction = requiresTransaction || localRequiresTransaction;
                }

                return readOperations.Count == 0
                    ? ResultSetMapping.NoResults
                    : ResultSetMapping.LastInResultSet;
            }

            if (readOperations.Count == 0)
            {
                // We have no values to read, just use a plain old multi-row INSERT.
                return AppendInsertMultipleRows(
                    commandStringBuilder, modificationCommands, writeOperations, out requiresTransaction);
            }

            // We can't use the OUTPUT (without INTO) clause (e.g. triggers are defined).
            // If we have an IDENTITY column, then multiple batched SELECT+INSERTs are faster up to a certain threshold (4), and then
            // MERGE ... OUTPUT INTO is faster.
            if (modificationCommands.Count < MergeIntoMinimumThreshold
                && firstCommand.ColumnModifications.All(
                    o =>
                        !o.IsKey
                        || !o.IsRead
                        || o.Property?.GetValueGenerationStrategy(table) == ActianValueGenerationStrategy.IdentityColumn
                        || o.Property?.GetValueGenerationStrategy(table) == ActianValueGenerationStrategy.IdentityByDefaultColumn))
            {
                requiresTransaction = true;

                foreach (var command in modificationCommands)
                {
                    AppendInsertAndSelectOperation(commandStringBuilder, command, commandPosition++, out _);
                }

                return ResultSetMapping.LastInResultSet;
            }

            return AppendMergeWithOutputInto(
                commandStringBuilder, modificationCommands, commandPosition, writeOperations, keyOperations, readOperations,
                out requiresTransaction);
        }

        private ResultSetMapping AppendMergeWithOutputInto(
    StringBuilder commandStringBuilder,
    IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
    int commandPosition,
    List<IColumnModification> writeOperations,
    List<IColumnModification> keyOperations,
    List<IColumnModification> readOperations,
    out bool requiresTransaction)
        {
            AppendDeclareTable(
                commandStringBuilder,
                InsertedTableBaseName,
                commandPosition,
                keyOperations,
                PositionColumnDeclaration);

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            AppendMergeCommandHeader(
                commandStringBuilder,
                name,
                schema,
                ToInsertTableAlias,
                modificationCommands,
                writeOperations,
                PositionColumnName);
            AppendOutputIntoClause(
                commandStringBuilder,
                keyOperations,
                InsertedTableBaseName,
                commandPosition,
                FullPositionColumnName);
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            AppendSelectCommand(
                commandStringBuilder, readOperations, keyOperations, InsertedTableBaseName, commandPosition, name, schema,
                orderColumn: PositionColumnName);

            requiresTransaction = true;

            return ResultSetMapping.NotLastInResultSet;
        }


        private ResultSetMapping AppendInsertMultipleDefaultRows(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            List<IColumnModification> writeableOperations,
            out bool requiresTransaction)
        {
            Check.DebugAssert(writeableOperations.Count > 0, $"writeableOperations.Count is {writeableOperations.Count}");

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeableOperations);
            AppendValuesHeader(commandStringBuilder, writeableOperations);
            AppendValues(commandStringBuilder, name, schema, writeableOperations);
            for (var i = 1; i < modificationCommands.Count; i++)
            {
                commandStringBuilder.AppendLine(",");
                AppendValues(commandStringBuilder, name, schema, writeableOperations);
            }

            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            requiresTransaction = false;

            return ResultSetMapping.NoResults;
        }

        private ResultSetMapping AppendInsertMultipleDefaultRowsWithOutputInto(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition,
            List<IColumnModification> writableOperations,
            List<IColumnModification> keyOperations,
            List<IColumnModification> readOperations,
            out bool requiresTransaction)
        {
            AppendDeclareTable(commandStringBuilder, InsertedTableBaseName, commandPosition, keyOperations);

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;
            AppendInsertCommandHeader(commandStringBuilder, name, schema, writableOperations);
            AppendOutputIntoClause(commandStringBuilder, keyOperations, InsertedTableBaseName, commandPosition);
            AppendValuesHeader(commandStringBuilder, writableOperations);
            AppendValues(commandStringBuilder, name, schema, writableOperations);
            for (var i = 1; i < modificationCommands.Count; i++)
            {
                commandStringBuilder.AppendLine(",");
                AppendValues(commandStringBuilder, name, schema, writableOperations);
            }

            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator);

            AppendSelectCommand(commandStringBuilder, readOperations, keyOperations, InsertedTableBaseName, commandPosition, name, schema);

            requiresTransaction = true;

            return ResultSetMapping.NotLastInResultSet;
        }

        private ResultSetMapping AppendMergeWithOutput(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            List<IColumnModification> writeOperations,
            List<IColumnModification> readOperations,
            out bool requiresTransaction)
        {
            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            AppendMergeCommandHeader(
                commandStringBuilder,
                name,
                schema,
                ToInsertTableAlias,
                modificationCommands,
                writeOperations,
                PositionColumnName);
            AppendOutputClause(
                commandStringBuilder,
                readOperations,
                FullPositionColumnName);
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            requiresTransaction = false;

            return ResultSetMapping.NotLastInResultSet | ResultSetMapping.IsPositionalResultMappingEnabled;
        }

        private void AppendMergeCommandHeader(
    StringBuilder commandStringBuilder,
    string name,
    string? schema,
    string toInsertTableAlias,
    IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
    IReadOnlyList<IColumnModification> writeOperations,
    string? additionalColumns = null)
        {
            commandStringBuilder.Append("MERGE ");
            SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, name, schema);

            commandStringBuilder
                .Append(" USING (");

            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, writeOperations, "0");
            for (var i = 1; i < modificationCommands.Count; i++)
            {
                commandStringBuilder.AppendLine(",");
                AppendValues(
                    commandStringBuilder,
                    modificationCommands[i].ColumnModifications.Where(o => o.IsWrite).ToList(),
                    i.ToString(CultureInfo.InvariantCulture));
            }

            commandStringBuilder
                .Append(") AS ").Append(toInsertTableAlias)
                .Append(" (")
                .AppendJoin(
                    writeOperations,
                    SqlGenerationHelper,
                    (sb, o, helper) => helper.DelimitIdentifier(sb, o.ColumnName));
            if (additionalColumns != null)
            {
                commandStringBuilder
                    .Append(", ")
                    .Append(additionalColumns);
            }

            commandStringBuilder
                .Append(')')
                .AppendLine(" ON 1=0")
                .AppendLine("WHEN NOT MATCHED THEN")
                .Append("INSERT ")
                .Append('(')
                .AppendJoin(
                    writeOperations,
                    SqlGenerationHelper,
                    (sb, o, helper) => helper.DelimitIdentifier(sb, o.ColumnName))
                .Append(')');

            AppendValuesHeader(commandStringBuilder, writeOperations);
            commandStringBuilder
                .Append('(')
                .AppendJoin(
                    writeOperations,
                    (toInsertTableAlias, SqlGenerationHelper),
                    static (sb, o, state) =>
                    {
                        var (alias, helper) = state;
                        sb.Append(alias).Append('.');
                        helper.DelimitIdentifier(sb, o.ColumnName);
                    })
                .Append(')');
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void AppendOutputClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations,
            string? additionalReadValues = null)
        {
            if (operations.Count > 0 || additionalReadValues is not null)
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("OUTPUT ")
                    .AppendJoin(
                        operations,
                        SqlGenerationHelper,
                        (sb, o, helper) =>
                        {
                            sb.Append("INSERTED.");
                            helper.DelimitIdentifier(sb, o.ColumnName);
                        });

                if (additionalReadValues is not null)
                {
                    if (operations.Count > 0)
                    {
                        commandStringBuilder.Append(", ");
                    }

                    commandStringBuilder.Append(additionalReadValues);
                }
            }
        }

        private void AppendValues(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations,
            string additionalLiteral)
        {
            if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append('(')
                    .AppendJoin(
                        operations,
                        SqlGenerationHelper,
                        (sb, o, helper) =>
                        {
                            if (o.IsWrite)
                            {
                                helper.GenerateParameterName(sb, o.ParameterName!);
                            }
                            else
                            {
                                sb.Append("DEFAULT");
                            }
                        })
                    .Append(", ")
                    .Append(additionalLiteral)
                    .Append(')');
            }
        }

        private void AppendDeclareTable(
            StringBuilder commandStringBuilder,
            string name,
            int index,
            IReadOnlyList<IColumnModification> operations,
            string? additionalColumns = null)
        {
            commandStringBuilder
                .Append("DECLARE ")
                .Append(name)
                .Append(index)
                .Append(" TABLE (")
                .AppendJoin(
                    operations,
                    this,
                    (sb, o, generator) =>
                    {
                        generator.SqlGenerationHelper.DelimitIdentifier(sb, o.ColumnName);
                        sb.Append(' ').Append(GetTypeNameForCopy(o.Property!));
                    });

            if (additionalColumns != null)
            {
                commandStringBuilder
                    .Append(", ")
                    .Append(additionalColumns);
            }

            commandStringBuilder
                .Append(')')
                .AppendLine(SqlGenerationHelper.StatementTerminator);
        }

        private static string GetTypeNameForCopy(IProperty property)
        {
            var typeName = property.GetColumnType();

            return property.ClrType == typeof(byte[])
                && (typeName.Equals("rowversion", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("timestamp", StringComparison.OrdinalIgnoreCase))
                    ? property.IsNullable ? "varbinary(8)" : "binary(8)"
                    : typeName;
        }

        protected override void AppendReturningClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations,
            string? additionalValues = null)
            => AppendOutputClause(commandStringBuilder, operations, additionalValues);

 
        private void AppendOutputIntoClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IColumnModification> operations,
            string tableName,
            int tableIndex,
            string? additionalColumns = null)
        {
            if (operations.Count > 0 || additionalColumns is not null)
            {
                AppendOutputClause(commandStringBuilder, operations, additionalColumns);

                commandStringBuilder.AppendLine()
                    .Append("INTO ").Append(tableName).Append(tableIndex);
            }
        }

        private ResultSetMapping AppendSelectCommand(
             StringBuilder commandStringBuilder,
             IReadOnlyList<IColumnModification> readOperations,
             IReadOnlyList<IColumnModification> keyOperations,
             string insertedTableName,
             int insertedTableIndex,
             string tableName,
             string? schema,
             string? orderColumn = null)
        {
            if (readOperations.SequenceEqual(keyOperations))
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("SELECT ")
                    .AppendJoin(
                        readOperations,
                        SqlGenerationHelper,
                        (sb, o, helper) => helper.DelimitIdentifier(sb, o.ColumnName, "i"))
                    .Append(" FROM ")
                    .Append(insertedTableName).Append(insertedTableIndex).Append(" i");
            }
            else
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("SELECT ")
                    .AppendJoin(
                        readOperations,
                        SqlGenerationHelper,
                        (sb, o, helper) => helper.DelimitIdentifier(sb, o.ColumnName, "t"))
                    .Append(" FROM ");
                SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, tableName, schema);
                commandStringBuilder
                    .AppendLine(" t")
                    .Append("INNER JOIN ")
                    .Append(insertedTableName).Append(insertedTableIndex)
                    .Append(" i")
                    .Append(" ON ")
                    .AppendJoin(
                        keyOperations, (sb, c) =>
                        {
                            sb.Append('(');
                            SqlGenerationHelper.DelimitIdentifier(sb, c.ColumnName, "t");
                            sb.Append(" = ");
                            SqlGenerationHelper.DelimitIdentifier(sb, c.ColumnName, "i");
                            sb.Append(')');
                        }, " AND ");
            }

            if (orderColumn != null)
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("ORDER BY ");
                SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, orderColumn, "i");
            }

            commandStringBuilder
                .AppendLine(SqlGenerationHelper.StatementTerminator)
                .AppendLine();

            return ResultSetMapping.LastInResultSet;
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, IColumnModification columnModification)
        {
            SqlGenerationHelper.DelimitIdentifier(commandStringBuilder, columnModification.ColumnName);
            commandStringBuilder
                .Append(" = ")
                .Append("LAST_IDENTITY()");
        }


        private ResultSetMapping AppendInsertMultipleRows(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            List<IColumnModification> writeOperations,
            out bool requiresTransaction)
        {
            Check.DebugAssert(writeOperations.Count > 0, $"writeOperations.Count is {writeOperations.Count}");

            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, name, schema, writeOperations);
            for (var i = 1; i < modificationCommands.Count; i++)
            {
                commandStringBuilder.AppendLine(",");
                AppendValues(commandStringBuilder, name, schema, modificationCommands[i].ColumnModifications.Where(o => o.IsWrite).ToList());
            }

            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);

            requiresTransaction = false;

            return ResultSetMapping.NoResults;
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(
            StringBuilder commandStringBuilder,
            string name,
            string? schema,
            int commandPosition)
        {
            commandStringBuilder
                .Append("SELECT @@ROW_COUNT")
                .AppendLine(SqlGenerationHelper.StatementTerminator)
                .AppendLine();

            return ResultSetMapping.LastInResultSet | ResultSetMapping.ResultSetWithRowsAffectedOnly;
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
            => commandStringBuilder
                .Append("@@ROW_COUNT = ")
                .Append(expectedRowsAffected.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        ///     Appends a SQL command for selecting affected data.
        /// </summary>
        /// <param name="commandStringBuilder">The builder to which the SQL should be appended.</param>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The table schema, or <see langword="null" /> to use the default schema.</param>
        /// <param name="readOperations">The operations representing the data to be read.</param>
        /// <param name="conditionOperations">The operations used to generate the <c>WHERE</c> clause for the select.</param>
        /// <param name="commandPosition">The ordinal of the command for which rows affected it being returned.</param>
        /// <returns>The <see cref="ResultSetMapping" /> for this command.</returns>
        protected override ResultSetMapping AppendSelectAffectedCommand(
            StringBuilder commandStringBuilder,
            string name,
            string? schema,
            IReadOnlyList<IColumnModification> readOperations,
            IReadOnlyList<IColumnModification> conditionOperations,
            int commandPosition)
        {
            AppendSelectCommandHeader(commandStringBuilder, readOperations);
            AppendFromClause(commandStringBuilder, name, schema);
            AppendWhereAffectedClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator)
                .AppendLine();

            return ResultSetMapping.LastInResultSet;
        }
    }
}
