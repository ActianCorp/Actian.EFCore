// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Actian.EFCore.Scaffolding.Internal
{
    internal static class MutableAnnotatableExtensions
    {
        public static TAnnotatable WithAnnotation<TAnnotatable>(this TAnnotatable annotatable, string name, object value)
            where TAnnotatable : IMutableAnnotatable
        {
            annotatable.SetAnnotation(name, value);
            return annotatable;
        }
    }

    internal static class DatabaseModelExtensions
    {
        public static TDatabaseTable AddTable<TDatabaseTable>(this DatabaseModel database, TDatabaseTable table)
            where TDatabaseTable : DatabaseTable
        {
            table.Database = database;
            database.Tables.Add(table);
            return table;
        }

        public static DatabaseSequence AddSequence(this DatabaseModel database, DatabaseSequence sequence)
        {
            sequence.Database = database;
            database.Sequences.Add(sequence);
            return sequence;
        }
    }

    internal static class DatabaseTableExtensions
    {
        public static DatabaseTable GetTable(this IEnumerable<DatabaseTable> tables, string schema, string name, bool throwOnNotFound = true)
        {
            var table = tables.FirstOrDefault(t => t.Schema == schema && t.Name == name)
                ?? tables.FirstOrDefault(t => t.Schema == schema && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? tables.FirstOrDefault(t => t.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase) && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (throwOnNotFound)
                Debug.Assert(table != null, "table is null.");
            return table;
        }

        public static DatabaseColumn GetColumn(this IEnumerable<DatabaseTable> tables, string schema, string tableName, string columnName, bool throwOnNotFound = true)
        {
            return tables.GetTable(schema, tableName, throwOnNotFound)?.GetColumn(columnName, throwOnNotFound);
        }

        public static DatabaseColumn GetColumn(this DatabaseTable table, string columnName, bool throwOnNotFound = true)
        {
            var column = table?.Columns.FirstOrDefault(c => c.Name == columnName)
                ?? table?.Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (throwOnNotFound)
                Debug.Assert(column != null, "column is null.");
            return column;
        }

        public static TDatabaseTable WithColumns<TDatabaseTable>(this TDatabaseTable table, IEnumerable<DatabaseColumn> columns)
            where TDatabaseTable : DatabaseTable
        {
            foreach (var column in columns)
            {
                column.Table = table;
                table.Columns.Add(column);
            }
            return table;
        }
    }

    internal static class DatabasePrimaryKeyExtensions
    {
        public static DatabasePrimaryKey WithTable(this DatabasePrimaryKey primaryKey, DatabaseTable table)
        {
            primaryKey.Table = table;
            table.PrimaryKey = primaryKey;
            return primaryKey;
        }

        public static DatabasePrimaryKey WithColumns(this DatabasePrimaryKey primaryKey, IEnumerable<string> columnNames)
        {
            foreach (var columnName in columnNames)
            {
                primaryKey.Columns.Add(primaryKey.Table.GetColumn(columnName));
            }
            return primaryKey;
        }
    }

    internal static class DatabaseUniqueConstraintExtensions
    {
        public static DatabaseUniqueConstraint WithTable(this DatabaseUniqueConstraint constraint, DatabaseTable table)
        {
            constraint.Table = table;
            table.UniqueConstraints.Add(constraint);
            return constraint;
        }

        public static DatabaseUniqueConstraint WithColumns(this DatabaseUniqueConstraint constraint, IEnumerable<string> columnNames)
        {
            foreach (var columnName in columnNames)
            {
                constraint.Columns.Add(constraint.Table.GetColumn(columnName));
            }
            return constraint;
        }
    }

    internal static class DatabaseIndexExtensions
    {
        public static DatabaseIndex WithTable(this DatabaseIndex index, DatabaseTable table)
        {
            index.Table = table;
            table.Indexes.Add(index);
            return index;
        }

        public static DatabaseIndex WithColumns(this DatabaseIndex index, IEnumerable<string> columnNames)
        {
            foreach (var columnName in columnNames)
            {
                index.Columns.Add(index.Table.GetColumn(columnName));
            }
            return index;
        }
    }

    internal static class DatabaseSequenceExtensions
    {
        public static DatabaseSequence WithDatabase(this DatabaseSequence sequence, DatabaseModel database)
        {
            sequence.Database = database;
            database.Sequences.Add(sequence);
            return sequence;
        }

        private static DatabaseSequence WithDefaultMinValue(this DatabaseSequence sequence)
        {
            switch (sequence.StoreType?.ToLowerInvariant())
            {
                case "integer" when sequence.IncrementBy > 0 && sequence.MinValue == 1L:
                case "bigint" when sequence.IncrementBy > 0 && sequence.MinValue == 1L:
                case "integer" when sequence.IncrementBy < 0 && sequence.MinValue == -2147483647L:
                case "bigint" when sequence.IncrementBy < 0 && sequence.MinValue == -9223372036854775807L:
                    sequence.MinValue = null;
                    break;
            }
            return sequence;
        }

        private static DatabaseSequence WithDefaultMaxValue(this DatabaseSequence sequence)
        {
            switch (sequence.StoreType?.ToLowerInvariant())
            {
                case "integer" when sequence.IncrementBy > 0 && sequence.MaxValue == 2147483647L:
                case "bigint" when sequence.IncrementBy > 0 && sequence.MaxValue == 9223372036854775807L:
                case "integer" when sequence.IncrementBy < 0 && sequence.MaxValue == -1L:
                case "bigint" when sequence.IncrementBy < 0 && sequence.MaxValue == -1L:
                    sequence.MaxValue = null;
                    break;
            }
            return sequence;
        }

        private static DatabaseSequence WithDefaultStartValue(this DatabaseSequence sequence)
        {
            switch (sequence.StoreType?.ToLowerInvariant())
            {
                case "integer" when sequence.IncrementBy > 0 && sequence.StartValue == 1L:
                case "bigint" when sequence.IncrementBy > 0 && sequence.StartValue == 1L:
                case "integer" when sequence.IncrementBy < 0 && sequence.StartValue == -1L:
                case "bigint" when sequence.IncrementBy < 0 && sequence.StartValue == -1L:
                    sequence.StartValue = null;
                    break;
            }
            return sequence;
        }

        public static DatabaseSequence WithDefaultValues(this DatabaseSequence sequence)
        {
            return sequence
                .WithDefaultStartValue()
                .WithDefaultMinValue()
                .WithDefaultMaxValue();
        }
    }
}
