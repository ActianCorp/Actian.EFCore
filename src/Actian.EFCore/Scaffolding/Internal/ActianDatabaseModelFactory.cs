// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Utilities;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Sprache;
using static Sprache.Parse;

#nullable enable

namespace Actian.EFCore.Scaffolding.Internal
{
    /// <summary>
    /// The default database model factory for Actian.
    /// </summary>
    public class ActianDatabaseModelFactory : DatabaseModelFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;
        private readonly IRelationalTypeMappingSource _typeMappingSource;

        public ActianDatabaseModelFactory(
        IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger,
        IRelationalTypeMappingSource typeMappingSource)
        {
            _logger = logger;
            _typeMappingSource = typeMappingSource;
        }

        /// <inheritdoc />
        public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(options, nameof(options));

            using var connection = new IngresConnection(connectionString);
            return Create(connection, options);
        }

        /// <inheritdoc />
        public override DatabaseModel Create(DbConnection dbConnection, DatabaseModelFactoryOptions options)
        {
            Check.NotNull(dbConnection, nameof(dbConnection));
            Check.NotNull(options, nameof(options));

            var connection = (IngresConnection)dbConnection;

            var connectionStartedOpen = connection.State == ConnectionState.Open;

            if (!connectionStartedOpen)
                connection.Open();

            try
            {
                var (dbNameCase, dbDelimitedCase) = connection.GetDbCasing();

                var database = new DatabaseModel
                {
                    DatabaseName = connection.Database,
                    DefaultSchema = GetDefaultSchema(connection, dbNameCase, dbDelimitedCase)
                }
                .WithAnnotation(ActianAnnotationNames.DbNameCase, dbNameCase)
                .WithAnnotation(ActianAnnotationNames.DbDelimitedCase, dbDelimitedCase);

                var schemaList = options.Schemas
                    .Select(schema => ActianNameFilterParser.ParseSchemaName(schema, dbNameCase, dbDelimitedCase))
                    .ToList();

                var tableList = options.Tables
                    .Select(table => ActianNameFilterParser.ParseTableName(table, dbNameCase, dbDelimitedCase))
                    .ToList();

                var schemaFilter = GenerateSchemaFilter(schemaList);
                var tableFilter = GenerateTableFilter(tableList, schemaFilter);

                GetTables(connection, database, tableFilter, dbNameCase, dbDelimitedCase);
                GetSequences(connection, database, schemaFilter, dbNameCase, dbDelimitedCase);

                var missingSchemas = schemaList
                    .Except(database.Sequences.Select(s => s.Schema).Concat(database.Tables.Select(t => t.Schema)));

                foreach (var schema in missingSchemas)
                {
                    _logger.MissingSchemaWarning(schema);
                }

                var missingTables = tableList
                    .Where(table => string.IsNullOrEmpty(table.schema)
                        ? !database.Tables.Any(t => t.Name == table.name)
                        : !database.Tables.Any(t => t.Schema == table.schema && t.Name == table.name)
                    )
                    .Select(x => x.table);

                foreach (var table in missingTables)
                {
                    _logger.MissingTableWarning(table);
                }

                return database;
            }
            finally
            {
                if (!connectionStartedOpen)
                    connection.Close();
            }
        }

        private string GetDefaultSchema(
            [NotNull] IngresConnection connection,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var dbaNames = connection.Select($@"
                select dba_name
                  from $ingres.iidbconstants
            ", reader => reader.GetTrimmedChar("dba_name", dbNameCase));

            if (dbaNames.Any())
            {
                var defaultSchema = dbaNames.First();
                _logger.DefaultSchemaFound(defaultSchema);
                return defaultSchema;
            }
            return null!;
        }

        /// <summary>
        /// Queries the database for defined tables and registers them with the model.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="database">The database model</param>
        /// <param name="tableFilter">The table filter fragment.</param>
        /// <param name="dbNameCase">Case sensitivity for regular identifiers</param>
        /// <param name="dbDelimitedCase">Case sensitivity for delimited identifiers</param>
        /// <returns>
        /// A collection of tables defined in the database.
        /// </returns>
        private IEnumerable<DatabaseTable> GetTables(
            [NotNull] IngresConnection connection,
            [NotNull] DatabaseModel database,
            [CanBeNull] Func<string, string, string> tableFilter,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var iicolumnsColumns = new HashSet<string>(connection.Select($@"
                select column_name
                  from $ingres.iicolumns
                 where table_owner = '{dbNameCase.Normalize("$ingres")}'
                   and table_name  = '{dbNameCase.Normalize("iicolumns")}'
            ", reader => reader.GetTrimmedChar(0)));

            var filter = tableFilter != null ? $"and {tableFilter("t.table_owner", "t.table_name")}" : null;

            var column_always_ident = iicolumnsColumns.Contains(dbNameCase.Normalize("column_always_ident")) ? "c.column_always_ident" : "'N'";
            var column_bydefault_ident = iicolumnsColumns.Contains(dbNameCase.Normalize("column_bydefault_ident")) ? "c.column_bydefault_ident" : "'N'";

            var tableLocations = connection.Select($@"
                select t.table_owner,
                       t.table_name,
                       l.loc_sequence,
                       l.location_name
                  from $ingres.iitables t
                  join $ingres.iimulti_locations l on
                       l.table_owner = t.table_owner
                   and l.table_name  = t.table_name
                 where t.table_type in ('T', 'V')
                   and t.system_use = 'U'
                   and t.table_name not like 'iietab_%'
                   {filter}
                 order by t.table_owner, t.table_name, l.loc_sequence
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("table_owner", dbNameCase),
                TableName = reader.GetTrimmedChar("table_name", dbNameCase),
                LocationName = reader.GetTrimmedChar("location_name", dbNameCase)
            }).GroupBy(t => (t.Schema, t.TableName))
              .ToDictionary(t => t.Key, t => t);

            var tables = connection.Select($@"
                select t.table_owner,
                       t.table_name,
                       t.table_type,
                       t.storage_structure,
                       t.is_compressed,
                       t.key_is_compressed,
                       t.duplicate_rows,
                       t.unique_rule,
                       t.is_journalled,
                       t.location_name,
                       c.column_name,
                       c.column_datatype,
                       c.column_length,
                       c.column_scale,
                       c.column_nulls,
                       c.column_defaults,
                       c.key_sequence,
                       c.column_default_val,
                       column_always_ident = {column_always_ident},
                       column_bydefault_ident = {column_bydefault_ident}
                  from $ingres.iitables t
                  join $ingres.iicolumns c on
                       c.table_owner = t.table_owner
                   and c.table_name  = t.table_name
                 where t.table_type in ('T', 'V')
                   and t.system_use = 'U'
                   and t.table_name not like 'iietab_%'
                   {filter}
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("table_owner", dbNameCase),
                TableName = reader.GetTrimmedChar("table_name", dbNameCase),
                TableType = reader.GetTrimmedChar("table_type", dbNameCase),
                StorageStructure = reader.GetTrimmedChar("storage_structure", dbNameCase),
                IsCompressed = reader.GetTrimmedChar("is_compressed", dbNameCase),
                KeyIsCompressed = reader.GetTrimmedChar("key_is_compressed", dbNameCase),
                DuplicateRows = reader.GetTrimmedChar("duplicate_rows", dbNameCase),
                UniqueRule = reader.GetTrimmedChar("unique_rule", dbNameCase),
                IsJournalled = reader.GetTrimmedChar("is_journalled", dbNameCase),
                LocationName = reader.GetTrimmedChar("location_name", dbNameCase),
                ColumnName = reader.GetTrimmedChar("column_name", dbNameCase),
                IsNullable = reader.GetTrimmedChar("column_nulls", dbNameCase) == "Y",
                StoreType = GetStoreType(
                    reader.GetTrimmedChar("column_datatype", dbNameCase),
                    reader.GetValueOrDefault<int>("column_length", dbNameCase),
                    reader.GetValueOrDefault<int>("column_scale", dbNameCase)
                ),
                DataTypeName = GetDataTypeName(
                    reader.GetTrimmedChar("column_datatype", dbNameCase),
                    reader.GetValueOrDefault<int>("column_length", dbNameCase)
                ),
                DefaultValueSql = reader.GetTrimmedChar("column_defaults", dbNameCase) == "Y"
                    ? reader.GetTrimmedChar("column_default_val", dbNameCase)?.TrimEnd()
                    : null,
                ValueGenerated = GetValueGenerated(reader, dbNameCase, dbDelimitedCase)
            }).GroupBy(t => (t.Schema, t.TableName)).Select(t =>
            {
                _logger.TableFound(DisplayName(t.Key.Schema, t.Key.TableName));
                var table = t.First();

                var databaseTable = table.TableType switch
                {
                    "T" => database.AddTable(new DatabaseTable
                    {
                        Schema = t.Key.Schema,
                        Name = t.Key.TableName
                    }),
                    "V" => database.AddTable(new DatabaseView
                    {
                        Schema = t.Key.Schema,
                        Name = t.Key.TableName
                    }),
                    _ => throw new Exception($"Unknown table type: {table.TableType}")
                };

                var columns = t.Select(column => new DatabaseColumn
                {
                    Name = column.ColumnName,
                    IsNullable = column.IsNullable,
                    StoreType = column.StoreType,
                    DefaultValue = TryParseClrDefault(column.DataTypeName, column.DefaultValueSql!),
                    DefaultValueSql = column.DefaultValueSql,
                    ComputedColumnSql = null,
                    ValueGenerated = column.ValueGenerated
                });

                var locations = new[] { table.LocationName };
                if (tableLocations.TryGetValue(t.Key, out var otherLocations))
                {
                    locations = locations
                        .Concat(otherLocations.Select(l => l.LocationName))
                        .ToArray();
                }

                return databaseTable
                    .WithColumns(columns)
                    .WithAnnotation(ActianAnnotationNames.Locations, locations)
                    .WithAnnotation(ActianAnnotationNames.Journaling, table.IsJournalled == "Y")
                    .WithAnnotation(ActianAnnotationNames.Duplicates, table.DuplicateRows == "D");
            }).ToList();

            GetConstraints(connection, tables, tableFilter!, dbNameCase, dbDelimitedCase);
            GetIndexes(connection, tables, tableFilter!, dbNameCase, dbDelimitedCase);
            GetComments(connection, tables, dbNameCase, dbDelimitedCase);
            return tables;
        }

        private static readonly string DefaultDatePrefixRe = @"(?:(?:date|time|timestamp)\s+)";
        private static readonly string DefaultDateRe = @"0001-01-01";
        private static readonly string DefaultTimeRe = @"00:00:00(?:\.0+)?";
        private static readonly string DefaultDateTimeRe = @$"(?:{DefaultDateRe}\s*T\s*{DefaultTimeRe}|{DefaultDateRe}\s+{DefaultTimeRe})";

        private static readonly Regex DefaultTimestampRe = new Regex(
            @$"^\s*{DefaultDatePrefixRe}?'(?:{DefaultDateRe}|{DefaultTimeRe}|{DefaultDateTimeRe})'\s*$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
        );

        private static string? FilterClrDefaults(string dataTypeName, bool nullable, string defaultValue)
        {
            if (defaultValue == null || defaultValue == "NULL")
                return null;

            if (nullable)
                return defaultValue;

            switch (dataTypeName?.ToLowerInvariant())
            {
                case "boolean" when defaultValue == "FALSE":
                case "tinyint" when defaultValue == "0":
                case "smallint" when defaultValue == "0":
                case "integer" when defaultValue == "0":
                case "bigint" when defaultValue == "0":
                case "float" when defaultValue == "0" || defaultValue == "0.0":
                case "float4" when defaultValue == "0" || defaultValue == "0.0":
                case "decimal" when defaultValue == "0" || defaultValue == "0.0":
                case "money" when defaultValue == "0" || defaultValue == "0.0":
                case "ansidate" when DefaultTimestampRe.IsMatch(defaultValue):
                case "ingresdate" when DefaultTimestampRe.IsMatch(defaultValue):
                case "time" when DefaultTimestampRe.IsMatch(defaultValue):
                case "time without time zone" when DefaultTimestampRe.IsMatch(defaultValue):
                case "time with local zone" when DefaultTimestampRe.IsMatch(defaultValue):
                case "time with time zone" when DefaultTimestampRe.IsMatch(defaultValue):
                case "timestamp" when DefaultTimestampRe.IsMatch(defaultValue):
                case "timestamp without time zone" when DefaultTimestampRe.IsMatch(defaultValue):
                case "timestamp with local zone" when DefaultTimestampRe.IsMatch(defaultValue):
                case "timestamp with time zone" when DefaultTimestampRe.IsMatch(defaultValue):
                    return null;
            }

            return defaultValue;
        }

        private object? TryParseClrDefault(string dataTypeName, string? defaultValueSql)
        {
            defaultValueSql = defaultValueSql?.Trim();
            if (string.IsNullOrEmpty(defaultValueSql))
            {
                return null;
            }

            var mapping = _typeMappingSource.FindMapping(dataTypeName);
            if (mapping == null)
            {
                return null;
            }

            Unwrap();
            if (defaultValueSql.StartsWith("CONVERT", StringComparison.OrdinalIgnoreCase))
            {
                defaultValueSql = defaultValueSql.Substring(defaultValueSql.IndexOf(',') + 1);
                defaultValueSql = defaultValueSql.Substring(0, defaultValueSql.LastIndexOf(')'));
                Unwrap();
            }

            if (defaultValueSql.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var type = mapping.ClrType;
            if (type == typeof(bool)
                && int.TryParse(defaultValueSql, out var intValue))
            {
                return intValue != 0;
            }

            if (type == typeof(bool)
                && bool.TryParse(defaultValueSql, out var stringValue))
            {
                return stringValue;
            }

            if (type.IsNumeric())
            {
                try
                {
                    return Convert.ChangeType(defaultValueSql, type);
                }
                catch
                {
                    // Ignored
                    return null;
                }
            }

            if (type == typeof(Guid)
                && Guid.TryParse(defaultValueSql, out var uuid))
            {
                return uuid;
            }

            if ((defaultValueSql.StartsWith('\'') || defaultValueSql.StartsWith("N'", StringComparison.OrdinalIgnoreCase))
                && defaultValueSql.EndsWith('\''))
            {
                var startIndex = defaultValueSql.IndexOf('\'');
                defaultValueSql = defaultValueSql.Substring(startIndex + 1, defaultValueSql.Length - (startIndex + 2));

                if (type == typeof(string))
                {
                    return defaultValueSql;
                }

                if (type == typeof(bool)
                    && bool.TryParse(defaultValueSql, out var boolValue))
                {
                    return boolValue;
                }

                if (type == typeof(Guid)
                    && Guid.TryParse(defaultValueSql, out var guid))
                {
                    return guid;
                }

                if (type == typeof(DateTime)
                    && DateTime.TryParse(defaultValueSql, out var dateTime))
                {
                    return dateTime;
                }

                if (type == typeof(DateOnly)
                    && DateOnly.TryParse(defaultValueSql, out var dateOnly))
                {
                    return dateOnly;
                }

                if ((type == typeof(TimeOnly) || type == typeof(TimeSpan))
                    && TimeOnly.TryParse(defaultValueSql, out var timeOnly))
                {
                    return timeOnly;
                }

                if (type == typeof(DateTimeOffset)
                    && DateTimeOffset.TryParse(defaultValueSql, out var dateTimeOffset))
                {
                    return dateTimeOffset;
                }

                if (type == typeof(float))
                {
                    return defaultValueSql;
                }
            }

            return null;

            void Unwrap()
            {
                while (defaultValueSql.StartsWith('(') && defaultValueSql.EndsWith(')'))
                {
                    defaultValueSql = (defaultValueSql.Substring(1, defaultValueSql.Length - 2)).Trim();
                }
            }
        }

        /// <summary>
        /// Queries the database for defined constraints and registers them with the model.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="tables">The database tables.</param>
        /// <param name="tableFilter">The table filter fragment.</param>
        /// <param name="dbNameCase">Case sensitivity for regular identifiers</param>
        /// <param name="dbDelimitedCase">Case sensitivity for delimited identifiers</param>
        /// <exception cref="InvalidOperationException">Found varying lengths for column and principal column indices.</exception>
        private void GetConstraints(
            [NotNull] IngresConnection connection,
            [NotNull] IEnumerable<DatabaseTable> tables,
            [CanBeNull] Func<string, string, string> tableFilter,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var filter = tableFilter != null ? $"and {tableFilter("t.table_owner", "t.table_name")}" : null;

            var constraintKeys = connection.Select($@"
                select t.table_owner,
                       t.table_name,
                       c.constraint_name,
                       k.column_name,
                       k.key_position
                  from $ingres.iitables t
                  join $ingres.iiconstraints c on
                       c.schema_name   = t.table_owner
                   and c.table_name    = t.table_name
                   and c.system_use    = 'U'
                   and c.text_sequence = 1
                   and c.constraint_type in ('P', 'U', 'R')
                  join $ingres.iikeys k on
                       k.schema_name     = c.schema_name
                   and k.constraint_name = c.constraint_name
                 where t.table_type in ('T', 'V')
                   and t.system_use = 'U'
                   {filter}
                 order by t.table_owner, t.table_name, c.constraint_name
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("table_owner", dbNameCase),
                Table = reader.GetTrimmedChar("table_name", dbNameCase),
                Name = reader.GetTrimmedChar("constraint_name", dbNameCase),
                ColumnName = reader.GetTrimmedChar("column_name", dbNameCase),
                Position = reader.GetValueOrDefault<short>("key_position", dbNameCase)
            }).GroupBy(c => (c.Schema, c.Table, c.Name)).ToDictionary(c => c.Key, c => c);

            var constraints = connection.Select($@"
                select t.table_owner,
                       t.table_name,
                       c.constraint_name,
                       c.constraint_type,
                       c.text_sequence,
                       c.text_segment
                  from $ingres.iitables t
                  join $ingres.iiconstraints c on
                       c.schema_name   = t.table_owner
                   and c.table_name    = t.table_name
                   and c.system_use    = 'U'
                   and c.constraint_type in ('P', 'U', 'R')
                 where t.table_type in ('T', 'V')
                   and t.system_use = 'U'
                   {filter}
                 order by t.table_owner, t.table_name, c.constraint_name, c.text_sequence
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("table_owner", dbNameCase),
                Table = reader.GetTrimmedChar("table_name", dbNameCase),
                Name = reader.GetTrimmedChar("constraint_name", dbNameCase),
                Type = reader.GetTrimmedChar("constraint_type", dbNameCase),
                TextSequence = reader.GetValueOrDefault<long>("text_sequence", dbNameCase),
                TextSegment = reader.GetValueOrDefault<string>("text_segment", dbNameCase)
            }).GroupBy(c => (c.Schema, c.Table, c.Name)).Select(c => new
            {
                c.Key.Schema,
                c.Key.Table,
                c.Key.Name,
                c.First().Type,
                Text = string.Join("", c.OrderBy(c2 => c2.TextSequence).Select(c2 => c2.TextSegment)),
                Keys = constraintKeys[c.Key].OrderBy(k => k.Position).Select(k => k.ColumnName).ToList()
            }).ToList();

            foreach (var constraint in constraints.Where(c => c.Type == "P"))
            {
                _logger.PrimaryKeyFound(constraint.Name, DisplayName(constraint.Schema, constraint.Table));

                new DatabasePrimaryKey
                {
                    Name = constraint.Name
                }
                .WithTable(tables.GetTable(constraint.Schema, constraint.Table))
                .WithColumns(constraint.Keys);
            }

            foreach (var constraint in constraints.Where(c => c.Type == "U"))
            {
                _logger.UniqueConstraintFound(constraint.Name, DisplayName(constraint.Schema, constraint.Table));
                new DatabaseUniqueConstraint
                {
                    Name = constraint.Name
                }
                .WithTable(tables.GetTable(constraint.Schema, constraint.Table))
                .WithColumns(constraint.Keys);
            }

            foreach (var constraint in constraints.Where(c => c.Type == "R"))
            {
                if (ActianForeignKeyConstraint.TryParse(constraint.Text, out var fk))
                {
                    _logger.ForeignKeyFound(constraint.Name, DisplayName(constraint.Schema, constraint.Table), DisplayName(fk.ReferencesTableSchema, fk.ReferencesTableName), $"{fk.OnDelete}");

                    var table = tables.GetTable(constraint.Schema, constraint.Table, throwOnNotFound: true);
                    var principalTable = tables.GetTable(fk.ReferencesTableSchema, fk.ReferencesTableName, throwOnNotFound: false);

                    if (principalTable == null)
                    {
                        _logger.ForeignKeyReferencesMissingPrincipalTableWarning(
                            constraint.Name,
                            DisplayName(constraint.Schema, constraint.Table),
                            DisplayName(fk.ReferencesTableSchema, fk.ReferencesTableName)
                        );

                        continue;
                    }

                    var foreignKey = new DatabaseForeignKey
                    {
                        Name = constraint.Name,
                        Table = table,
                        PrincipalTable = principalTable,
                        OnDelete = fk.OnDelete
                    };

                    var invalid = false;
                    foreach (var (columnName, principalColumnName) in fk.Keys.Zip(fk.ReferencesKeys, (k, r) => (k, r)))
                    {
                        var column = table.Columns.FirstOrDefault(c => c.Name == columnName)
                            ?? table.Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                        var principalColumn = principalTable.Columns.FirstOrDefault(c => c.Name == principalColumnName)
                            ?? principalTable.Columns.FirstOrDefault(c => c.Name.Equals(principalColumnName, StringComparison.OrdinalIgnoreCase));

                        if (principalColumn == null)
                        {
                            invalid = true;
                            _logger.ForeignKeyPrincipalColumnMissingWarning(
                                constraint.Name,
                                DisplayName(constraint.Schema, constraint.Table),
                                principalColumnName,
                                DisplayName(fk.ReferencesTableSchema, fk.ReferencesTableName)
                            );
                            break;
                        }

                        foreignKey.Columns.Add(column!);
                        foreignKey.PrincipalColumns.Add(principalColumn);
                    }

                    if (invalid)
                        continue;

                    if (foreignKey.Columns.SequenceEqual(foreignKey.PrincipalColumns))
                    {
                        _logger.ReflexiveConstraintIgnored(
                            constraint.Name,
                            DisplayName(constraint.Schema, constraint.Table)
                        );
                        continue;
                    }
                    else
                    {
                        var duplicated = table.ForeignKeys
                            .FirstOrDefault(
                                k => k.Columns.SequenceEqual(foreignKey.Columns)
                                    && k.PrincipalColumns.SequenceEqual(foreignKey.PrincipalColumns)
                                    && k.PrincipalTable.Equals(foreignKey.PrincipalTable));
                        if (duplicated != null)
                        {
                            _logger.DuplicateForeignKeyConstraintIgnored(
                                foreignKey.Name!,
                                DisplayName(table.Schema!, table.Name),
                                duplicated.Name!);
                            continue;
                        }
                    }

                        table.ForeignKeys.Add(foreignKey);
                }
            }
        }

        /// <summary>
        /// Queries the database for defined indexes and registers them with the model.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="tables">The database tables.</param>
        /// <param name="tableFilter">The table filter fragment.</param>
        /// <param name="dbNameCase">Case sensitivity for regular identifiers</param>
        /// <param name="dbDelimitedCase">Case sensitivity for delimited identifiers</param>
        private void GetIndexes(
            [NotNull] IngresConnection connection,
            [NotNull] IEnumerable<DatabaseTable> tables,
            [CanBeNull] Func<string, string, string> tableFilter,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var filter = tableFilter != null ? $"and {tableFilter("t.table_owner", "t.table_name")}" : null;

            var indexes = connection.Select($@"
                select t.table_owner,
                       t.table_name,
                       i.index_owner,
                       i.index_name,
                       i.unique_rule,
                       i.storage_structure,
                       i.is_compressed,
                       i.key_is_compressed,
                       i.unique_rule,
                       i.unique_scope,
                       i.persistent,
                       c.column_name,
                       c.key_sequence
                  from $ingres.iitables t
                  join $ingres.iiindexes i on
                       i.base_owner    = t.table_owner
                   and i.base_name     = t.table_name
                   and i.system_use    = 'U'
                  left join $ingres.iiindex_columns c on
                       c.index_owner   = i.index_owner
                   and c.index_name    = i.index_name
                 where t.table_type in ('T', 'V')
                   and t.system_use = 'U'
                   {filter}
                 order by t.table_owner, t.table_name, i.index_owner, i.index_name, c.key_sequence
            ", reader => new
            {
                TableSchema = reader.GetTrimmedChar("table_owner", dbNameCase),
                TableName = reader.GetTrimmedChar("table_name", dbNameCase),
                IndexSchema = reader.GetTrimmedChar("index_owner", dbNameCase),
                IndexName = reader.GetTrimmedChar("index_name", dbNameCase),
                StorageStructure = reader.GetTrimmedChar("storage_structure", dbNameCase),
                IsCompressed = reader.GetTrimmedChar("is_compressed", dbNameCase),
                KeyIsCompressed = reader.GetTrimmedChar("key_is_compressed", dbNameCase),
                UniqueRule = reader.GetTrimmedChar("unique_rule", dbNameCase),
                UniqueScope = reader.GetTrimmedChar("unique_scope", dbNameCase),
                Persistent = reader.GetTrimmedChar("persistent", dbNameCase),
                ColumnName = reader.GetTrimmedChar("column_name", dbNameCase),
                KeySequence = reader.GetValueOrDefault<short>("key_sequence", dbNameCase),
            }).ToList().GroupBy(i => (i.TableSchema, i.TableName, i.IndexSchema, i.IndexName)).Select(i =>
            {
                var index = i.First();
                var databaseIndex = new DatabaseIndex
                {
                    Name = index.IndexName,
                    IsUnique = index.UniqueRule == "U",
                    Filter = null
                };
                return databaseIndex
                    .WithTable(tables.GetTable(i.Key.TableSchema, i.Key.TableName))
                    .WithColumns(i.OrderBy(c => c.KeySequence).Select(c => c.ColumnName))
                    .WithAnnotation(ActianAnnotationNames.Persistence, index.Persistent == "Y");
            }).ToList();
        }

        /// <summary>
        /// Queries the database for sequences and registers them with the model.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="database"></param>
        /// <param name="schemaFilter"></param>
        /// <param name="dbNameCase">Case sensitivity for regular identifiers</param>
        /// <param name="dbDelimitedCase">Case sensitivity for delimited identifiers</param>
        private void GetSequences(
            [NotNull] IngresConnection connection,
            [NotNull] DatabaseModel database,
            [CanBeNull] Func<string, string> schemaFilter,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            DatabaseSequence ReadDatabaseSequence(DbDataReader reader) => new DatabaseSequence
            {
                Name = reader.GetTrimmedChar("seq_name", dbNameCase),
                Schema = reader.GetTrimmedChar("seq_owner", dbNameCase),
                StoreType = GetSequenceStoreType(reader.GetTrimmedChar("data_type", dbNameCase), reader.GetValueOrDefault<int?>("seq_precision", dbNameCase)),
                StartValue = Convert.ToInt64(reader.GetValueOrDefault<decimal>("start_value", dbNameCase)),
                IncrementBy = Convert.ToInt32(reader.GetValueOrDefault<decimal>("increment_value", dbNameCase)),
                MinValue = Convert.ToInt64(reader.GetValueOrDefault<decimal>("min_value", dbNameCase)),
                MaxValue = Convert.ToInt64(reader.GetValueOrDefault<decimal>("max_value", dbNameCase)),
                IsCyclic = reader.GetTrimmedChar("cycle_flag", dbNameCase) == "Y"
            };

            var filter = schemaFilter != null ? $"where {schemaFilter("seq_owner")}" : null;
            var sequences = connection.Select($@"
                select seq_owner,
                       seq_name,
                       data_type,
                       seq_precision,
                       start_value,
                       increment_value,
                       min_value,
                       max_value,
                       cycle_flag
                  from $ingres.iisequences
                  {filter}
                 order by seq_owner, seq_name
            ", ReadDatabaseSequence);

            sequences = sequences
                .Where(sequence => !sequence.Name.StartsWith("$iiidentity", StringComparison.InvariantCultureIgnoreCase))
                .Select(sequence => sequence.WithDefaultValues().WithDatabase(database))
                .ToList();
        }

        /// <summary>
        /// Queries the database for comments and registers them with the model.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="tables">The database tables.</param>
        /// <param name="dbNameCase">Case sensitivity for regular identifiers</param>
        /// <param name="dbDelimitedCase">Case sensitivity for delimited identifiers</param>
        private void GetComments(
            [NotNull] IngresConnection connection,
            [NotNull] IEnumerable<DatabaseTable> tables,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var tableComments = connection.Select($@"
                select object_name,
                       object_owner,
                       short_remark,
                       long_remark
                  from $ingres.iidb_comments
                 order by object_name, object_owner
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("object_owner", dbNameCase),
                Table = reader.GetTrimmedChar("object_name", dbNameCase),
                Comment = GetComment(reader, dbNameCase, dbDelimitedCase)
            }).ToList();

            foreach (var comment in tableComments)
            {
                var table = tables.GetTable(comment.Schema, comment.Table, throwOnNotFound: false);
                if (table != null)
                    table.Comment = comment.Comment;
            }

            var columnComments = connection.Select($@"
                select object_name,
                       object_owner,
                       subobject_name,
                       short_remark,
                       long_remark
                  from $ingres.iidb_subcomments
                 order by object_name, object_owner
            ", reader => new
            {
                Schema = reader.GetTrimmedChar("object_owner", dbNameCase),
                Table = reader.GetTrimmedChar("object_name", dbNameCase),
                Column = reader.GetTrimmedChar("subobject_name", dbNameCase),
                Comment = GetComment(reader, dbNameCase, dbDelimitedCase)
            }).ToList();

            foreach (var comment in columnComments)
            {
                var column = tables.GetColumn(comment.Schema, comment.Table, comment.Column, throwOnNotFound: false);
                if (column != null)
                    column.Comment = comment.Comment;
            }
        }

        private static readonly Regex DecimalRe = new Regex(@"^\s*DECIMAL(?:\s*\(\s*(\d+)\s*(?:,\s*(\d+)\s*)?\))?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static string GetSequenceStoreType(string typeName, int? precision) => typeName?.ToUpperInvariant() switch
        {
            null => GetStoreType(typeName!),
            "INTEGER" => GetStoreType("INTEGER", 4),
            "INTEGER4" => GetStoreType("INTEGER", 4),
            "INTEGER8" => GetStoreType("INTEGER", 8),
            "BIGINT" => GetStoreType("INTEGER", 8),
            "DECIMAL" => GetStoreType("DECIMAL", precision),
            //_ when typeName.Matches(DecimalRe, out var match) => GetStoreType("DECIMAL", ParseInt(match.Groups[1].Value), ParseInt(match.Groups[2].Value)),
            _ => GetStoreType(typeName)
        };

        private static string GetDataTypeName(string typeName, int? length = null)
        {
            switch (typeName?.ToUpperInvariant())
            {
                case "CHAR":
                case "VARCHAR":
                case "NCHAR":
                case "NVARCHAR":
                case "BYTE":
                case "BYTE VARYING":
                    return typeName.ToLowerInvariant();

                case "TINYINT":
                case "INTEGER" when (length ?? 4) == 1:
                    return "tinyint";

                case "SMALLINT":
                case "INTEGER" when (length ?? 4) == 2:
                    return "smallint";

                case "INTEGER" when (length ?? 4) == 0:
                case "INTEGER" when (length ?? 4) == 4:
                    return "integer";

                case "BIGINT":
                case "INTEGER" when (length ?? 4) == 8:
                    return "bigint";

                case "INTEGER":
                    return $"integer{length}";

                case "FLOAT" when (length ?? 0) == 4:
                    return "float4";

                case "FLOAT" when (length ?? 0) == 0:
                case "FLOAT" when (length ?? 0) == 8:
                    return "float";

                case "FLOAT":
                    return $"float{length}";

                case "TIME":
                    return "time";
                case "TIME WITHOUT TIME ZONE":
                    return "time without time zone";
                case "TIME WITH LOCAL TIME ZONE":
                    return "time with local time zone";
                case "TIME WITH TIME ZONE":
                    return "time with time zone";

                case "TIMESTAMP":
                    return "timestamp";
                case "TIMESTAMP WITHOUT TIME ZONE":
                    return "timestamp without time zone";
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return "timestamp with local time zone";
                case "TIMESTAMP WITH TIME ZONE":
                    return "timestamp with time zone";

                case "INTERVAL DAY TO SECOND":
                    return $"interval day to second";

                case "DECIMAL":
                    return $"decimal";

                default:
                    return typeName?.ToLowerInvariant()!;
            };
        }

        private static string GetStoreType(string typeName, int? length = null, int? scale = null)
        {
            switch (typeName?.ToUpperInvariant())
            {
                case "CHAR":
                case "VARCHAR":
                case "NCHAR":
                case "NVARCHAR":
                case "BYTE":
                case "BYTE VARYING":
                    return $"{typeName.ToLowerInvariant()}({length ?? 1})";

                case "TINYINT":
                case "INTEGER" when (length ?? 4) == 1:
                    return "tinyint";

                case "SMALLINT":
                case "INTEGER" when (length ?? 4) == 2:
                    return "smallint";

                case "INTEGER" when (length ?? 4) == 0:
                case "INTEGER" when (length ?? 4) == 4:
                    return "integer";

                case "BIGINT":
                case "INTEGER" when (length ?? 4) == 8:
                    return "bigint";

                case "INTEGER":
                    return $"integer{length}";

                case "FLOAT" when (length ?? 0) == 4:
                    return "float4";

                case "FLOAT" when (length ?? 0) == 0:
                case "FLOAT" when (length ?? 0) == 8:
                    return "float";

                case "FLOAT":
                    return $"float{length}";

                case "TIME" when (scale ?? 0) == 0:
                    return "time";
                case "TIME WITHOUT TIME ZONE" when (scale ?? 0) == 0:
                    return "time without time zone";
                case "TIME WITH LOCAL TIME ZONE" when (scale ?? 0) == 0:
                    return "time with local time zone";
                case "TIME WITH TIME ZONE" when (scale ?? 0) == 0:
                    return "time with time zone";

                case "TIME":
                    return $"time({scale})";
                case "TIME WITHOUT TIME ZONE":
                    return $"time({scale}) without time zone";
                case "TIME WITH LOCAL TIME ZONE":
                    return $"time({scale}) with local time zone";
                case "TIME WITH TIME ZONE":
                    return $"time({scale}) with time zone";

                case "TIMESTAMP" when (scale ?? 6) == 6:
                    return "timestamp";
                case "TIMESTAMP WITHOUT TIME ZONE" when (scale ?? 6) == 6:
                    return "timestamp without time zone";
                case "TIMESTAMP WITH LOCAL TIME ZONE" when (scale ?? 6) == 6:
                    return "timestamp with local time zone";
                case "TIMESTAMP WITH TIME ZONE" when (scale ?? 6) == 6:
                    return "timestamp with time zone";

                case "TIMESTAMP":
                    return $"timestamp({scale})";
                case "TIMESTAMP WITHOUT TIME ZONE":
                    return $"timestamp({scale}) without time zone";
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return $"timestamp({scale}) with local time zone";
                case "TIMESTAMP WITH TIME ZONE":
                    return $"timestamp({scale}) with time zone";

                case "INTERVAL DAY TO SECOND" when (scale ?? 0) == 0:
                    return $"interval day to second";

                case "INTERVAL DAY TO SECOND":
                    return $"interval day to second({scale})";

                case "DECIMAL" when (length ?? 5) == 5 && (scale ?? 0) == 0:
                    return $"decimal";

                case "DECIMAL" when (scale ?? 0) == 0:
                    return $"decimal({length})";

                case "DECIMAL":
                    return $"decimal({length},{scale})";

                default:
                    return typeName?.ToLowerInvariant()!;
            };
        }

        private static ValueGenerated? GetValueGenerated(
            DbDataReader reader,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var always = reader.GetTrimmedChar("column_always_ident", dbNameCase) == "Y";
            var byDefault = reader.GetTrimmedChar("column_bydefault_ident", dbNameCase) == "Y";
            if (always || byDefault)
                return ValueGenerated.OnAdd;
            return null;
        }

        private static string GetComment(
            DbDataReader reader,
            ActianCasing dbNameCase,
            ActianCasing dbDelimitedCase)
        {
            var longRemark = reader.GetTrimmedChar("long_remark", dbNameCase);
            if (!string.IsNullOrWhiteSpace(longRemark))
                return longRemark;

            var shortRemark = reader.GetTrimmedChar("short_remark", dbNameCase);
            if (!string.IsNullOrWhiteSpace(shortRemark))
                return shortRemark;

            return null!;
        }

        /// <summary>
        /// Builds a delegate to generate a schema filter fragment.
        /// </summary>
        /// <param name="schemas">The list of schema names.</param>
        /// <returns>
        /// A delegate that generates a schema filter fragment.
        /// </returns>
        [CanBeNull]
        private static Func<string, string> GenerateSchemaFilter(
            [NotNull] IEnumerable<string> schemas)
        {
            if (!schemas.Any())
                return null!;

            return schema => $"{schema} in ({string.Join(", ", schemas.Select(EscapeLiteral))})";
        }

        /// <summary>
        /// Builds a delegate to generate a table filter fragment.
        /// </summary>
        /// <param name="tables">The list of tables parsed into tuples of schema name and table name.</param>
        /// <param name="schemaFilter">The delegate that generates a schema filter fragment.</param>
        /// <returns>
        /// A delegate that generates a table filter fragment.
        /// </returns>
        [CanBeNull]
        private static Func<string, string, string> GenerateTableFilter(
            [NotNull] IEnumerable<(string table, string schema, string name)> tables,
            [CanBeNull] Func<string, string> schemaFilter)
        {
            if (schemaFilter == null && !tables.Any())
                return null!;

            return (schema, table) =>
            {
                IEnumerable<string> GetPredicates()
                {
                    if (schemaFilter != null)
                    {
                        yield return schemaFilter(schema);
                    }

                    var tablesWithoutSchema = tables
                        .Where(e => string.IsNullOrEmpty(e.schema))
                        .Select(e => EscapeLiteral(e.name))
                        .ToList();
                    if (tablesWithoutSchema.Any())
                    {
                        yield return $"{table} in ({string.Join(", ", tablesWithoutSchema)})";
                    }

                    var tablesWithSchema = tables
                        .Where(e => !string.IsNullOrEmpty(e.schema))
                        .Select(e => EscapeLiteral($"{e.schema}.{e.name}"))
                        .ToList();
                    if (tablesWithSchema.Any())
                    {
                        yield return $"rtrim({schema}) + '.' + rtrim({table}) in ({string.Join(", ", tablesWithSchema)})"; ;
                    }
                }

                return $"({string.Join(" or ", GetPredicates())})";
            };
        }

        /// <summary>
        /// Constructs the display name given a schema and table name.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="name">The table name.</param>
        /// <returns>
        /// A display name in the form of 'schema.name' or 'name'.
        /// </returns>
        [NotNull]
        private static string DisplayName([CanBeNull] string schema, [NotNull] string name)
            => string.IsNullOrEmpty(schema) ? name : $"{schema}.{name}";

        /// <summary>
        /// Wraps a string literal in single quotes.
        /// </summary>
        /// <param name="str">The string literal.</param>
        /// <returns>
        /// The string literal wrapped in single quotes.
        /// </returns>
        [NotNull]
        private static string EscapeLiteral([CanBeNull] string str) => $"'{str.Replace("'", "''")}'";

        private static int? ParseInt(string str)
            => string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out var value) ? null : (int?)value;
    }
}
