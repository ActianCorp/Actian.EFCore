// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Reflection;
using System.Resources;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Actian.EFCore.Internal
{
#nullable enable
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class ActianStrings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Actian.EFCore.Properties.ActianStrings", typeof(ActianStrings).GetTypeInfo().Assembly);

        /// <summary>
        ///     Identity value generation cannot be used for the property '{property}' on entity type '{entityType}' because the property type is '{propertyType}'. Identity value generation can only be used with signed integer properties.
        /// </summary>
        public static string IdentityBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
            => string.Format(
                GetString("IdentityBadType", nameof(property), nameof(entityType), nameof(propertyType)),
                property, entityType, propertyType);

        /// <summary>
        ///     Data type '{dataType}' is not supported in this form. Either specify the length explicitly in the type name, for example as '{dataType}(16)', or remove the data type and use APIs such as HasMaxLength to allow EF choose the data type.
        /// </summary>
        public static string UnqualifiedDataType([CanBeNull] object dataType)
            => string.Format(
                GetString("UnqualifiedDataType", nameof(dataType)),
                dataType);

        /// <summary>
        ///     Data type '{dataType}' for property '{property}' is not supported in this form. Either specify the length explicitly in the type name, for example as '{dataType}(16)', or remove the data type and use APIs such as HasMaxLength to allow EF choose the data type.
        /// </summary>
        public static string UnqualifiedDataTypeOnProperty([CanBeNull] object dataType, [CanBeNull] object property)
            => string.Format(
                GetString("UnqualifiedDataTypeOnProperty", nameof(dataType), nameof(property)),
                dataType, property);

        /// <summary>
        ///     SQL Server sequences cannot be used to generate values for the property '{property}' on entity type '{entityType}' because the property type is '{propertyType}'. Sequences can only be used with integer properties.
        /// </summary>
        public static string SequenceBadType([CanBeNull] object property, [CanBeNull] object entityType, [CanBeNull] object propertyType)
            => string.Format(
                GetString("SequenceBadType", nameof(property), nameof(entityType), nameof(propertyType)),
                property, entityType, propertyType);

        /// <summary>
        ///     SQL Server requires the table name to be specified for rename index operations. Specify table name in the call to MigrationBuilder.RenameIndex.
        /// </summary>
        public static string IndexTableRequired
            => GetString("IndexTableRequired");

        /// <summary>
        ///     To set memory-optimized on a table on or off the table needs to be dropped and recreated.
        /// </summary>
        public static string AlterMemoryOptimizedTable
            => GetString("AlterMemoryOptimizedTable");

        /// <summary>
        ///     To change the IDENTITY property of a column, the column needs to be dropped and recreated.
        /// </summary>
        public static string AlterIdentityColumn
            => GetString("AlterIdentityColumn");

        /// <summary>
        ///     An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure()' to the 'UseSqlServer' call.
        /// </summary>
        public static string TransientExceptionDetected
            => GetString("TransientExceptionDetected");

        /// <summary>
        ///     The property '{property}' on entity type '{entityType}' is configured to use 'SequenceHiLo' value generator, which is only intended for keys. If this was intentional configure an alternate key on the property, otherwise call 'ValueGeneratedNever' or configure store generation for this property.
        /// </summary>
        public static string NonKeyValueGeneration([CanBeNull] object property, [CanBeNull] object entityType)
            => string.Format(
                GetString("NonKeyValueGeneration", nameof(property), nameof(entityType)),
                property, entityType);

        /// <summary>
        ///     The properties {properties} are configured to use 'Identity' value generator and are mapped to the same table '{table}'. Only one column per table can be configured as 'Identity'. Call 'ValueGeneratedNever' for properties that should not use 'Identity'.
        /// </summary>
        public static string MultipleIdentityColumns([CanBeNull] object properties, [CanBeNull] object table)
            => string.Format(
                GetString("MultipleIdentityColumns", nameof(properties), nameof(table)),
                properties, table);

        /// <summary>
        ///     Cannot use table '{table}' for entity type '{entityType}' since it is being used for entity type '{otherEntityType}' and entity type '{memoryOptimizedEntityType}' is marked as memory-optimized, but entity type '{nonMemoryOptimizedEntityType}' is not.
        /// </summary>
        public static string IncompatibleTableMemoryOptimizedMismatch([CanBeNull] object table, [CanBeNull] object entityType, [CanBeNull] object otherEntityType, [CanBeNull] object memoryOptimizedEntityType, [CanBeNull] object nonMemoryOptimizedEntityType)
            => string.Format(
                GetString("IncompatibleTableMemoryOptimizedMismatch", nameof(table), nameof(entityType), nameof(otherEntityType), nameof(memoryOptimizedEntityType), nameof(nonMemoryOptimizedEntityType)),
                table, entityType, otherEntityType, memoryOptimizedEntityType, nonMemoryOptimizedEntityType);

        /// <summary>
        ///     The database name could not be determined. To use EnsureDeleted, the connection string must specify Initial Catalog.
        /// </summary>
        public static string NoInitialCatalog
            => GetString("NoInitialCatalog");

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}', but are configured with different identity increment values.
        /// </summary>
        public static string DuplicateColumnIdentityIncrementMismatch(object? entityType1, object? property1, object? entityType2, object? property2, object? columnName, object? table)
            => string.Format(
                GetString("DuplicateColumnIdentityIncrementMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}', but are configured with different identity seed values.
        /// </summary>
        public static string DuplicateColumnIdentitySeedMismatch(object? entityType1, object? property1, object? entityType2, object? property2, object? columnName, object? table)
            => string.Format(
                GetString("DuplicateColumnIdentitySeedMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}', but are configured with different hi-lo sequences.
        /// </summary>
        public static string DuplicateColumnSequenceMismatch(object? entityType1, object? property1, object? entityType2, object? property2, object? columnName, object? table)
            => string.Format(
                GetString("DuplicateColumnSequenceMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     '{entityType1}.{property1}' and '{entityType2}.{property2}' are both mapped to column '{columnName}' in '{table}' but are configured with different value generation strategies.
        /// </summary>
        public static string DuplicateColumnNameValueGenerationStrategyMismatch([CanBeNull] object entityType1, [CanBeNull] object property1, [CanBeNull] object entityType2, [CanBeNull] object property2, [CanBeNull] object columnName, [CanBeNull] object table)
            => string.Format(
                GetString("DuplicateColumnNameValueGenerationStrategyMismatch", nameof(entityType1), nameof(property1), nameof(entityType2), nameof(property2), nameof(columnName), nameof(table)),
                entityType1, property1, entityType2, property2, columnName, table);

        /// <summary>
        ///     The specified table '{table}' is not valid. Specify tables using the format '[schema].[table]'.
        /// </summary>
        public static string InvalidTableToIncludeInScaffolding([CanBeNull] object table)
            => string.Format(
                GetString("InvalidTableToIncludeInScaffolding", nameof(table)),
                table);

        /// <summary>
        ///     The expression passed to the 'propertyReference' parameter of the 'FreeText' method is not a valid reference to a property. The expression should represent a reference to a full-text indexed property on the object referenced in the from clause: 'from e in context.Entities where EF.Functions.FreeText(e.SomeProperty, textToSearchFor) select e'
        /// </summary>
        public static string InvalidColumnNameForFreeText
            => GetString("InvalidColumnNameForFreeText");

        /// <summary>
        ///     Include property '{entityType}.{property}' cannot be defined multiple times
        /// </summary>
        public static string IncludePropertyDuplicated([CanBeNull] object entityType, [CanBeNull] object property)
            => string.Format(
                GetString("IncludePropertyDuplicated", nameof(entityType), nameof(property)),
                entityType, property);

        /// <summary>
        ///     Include property '{entityType}.{property}' is already included in the index
        /// </summary>
        public static string IncludePropertyInIndex([CanBeNull] object entityType, [CanBeNull] object property)
            => string.Format(
                GetString("IncludePropertyInIndex", nameof(entityType), nameof(property)),
                entityType, property);

        /// <summary>
        ///     Include property '{entityType}.{property}' not found
        /// </summary>
        public static string IncludePropertyNotFound([CanBeNull] object entityType, [CanBeNull] object property)
            => string.Format(
                GetString("IncludePropertyNotFound", nameof(entityType), nameof(property)),
                entityType, property);

        /// <summary>
        ///     The keys {key1} on '{entityType1}' and {key2} on '{entityType2}' are both mapped to '{table}.{keyName}' but with different clustering.
        /// </summary>
        public static string DuplicateKeyMismatchedClustering([CanBeNull] object key1, [CanBeNull] object entityType1, [CanBeNull] object key2, [CanBeNull] object entityType2, [CanBeNull] object table, [CanBeNull] object keyName)
            => string.Format(
                GetString("DuplicateKeyMismatchedClustering", nameof(key1), nameof(entityType1), nameof(key2), nameof(entityType2), nameof(table), nameof(keyName)),
                key1, entityType1, key2, entityType2, table, keyName);

        /// <summary>
        ///     The '{methodName}' method is not supported because the query has switched to client-evaluation. Inspect the log to determine which query expressions are triggering client-evaluation.
        /// </summary>
        public static string FunctionOnClient([CanBeNull] object methodName)
            => string.Format(
                GetString("FunctionOnClient", nameof(methodName)),
                methodName);

        /// <summary>
        ///     Generating idempotent scripts for migration is not currently supported by Actian databases
        /// </summary>
        public static string MigrationScriptGenerationNotSupported
            => GetString("MigrationScriptGenerationNotSupported");

        /// <summary>
        ///     Table '{schema}.{table}' can not be renamed to '{newschema}.{newtable}' because it has a different schema.
        /// </summary>
        public static string RenameTableToDifferentSchema([CanBeNull] object schema, [CanBeNull] object table, [CanBeNull] object newschema, [CanBeNull] object newtable)
            => string.Format(
                GetString("RenameTableToDifferentSchema", nameof(schema), nameof(table), nameof(newschema), nameof(newtable)),
                schema, table, newschema, newtable);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name)!;
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }

        /// <summary>
        ///     Could not save changes because the target table has computed column with a function that performs data access. Please configure your table accordingly, see https://aka.ms/efcore-docs-sqlserver-save-changes-and-output-clause for more information.
        /// </summary>
        public static string SaveChangesFailedBecauseOfComputedColumnWithFunction
            => GetString("SaveChangesFailedBecauseOfComputedColumnWithFunction");

        /// <summary>
        ///     Could not save changes because the target table has database triggers. Please configure your table accordingly, see https://aka.ms/efcore-docs-sqlserver-save-changes-and-output-clause for more information.
        /// </summary>
        public static string SaveChangesFailedBecauseOfTriggers
            => GetString("SaveChangesFailedBecauseOfTriggers");

        /// <summary>
        ///     EF Core's compatibility level is set to {compatibilityLevel}; compatibility level 130 is the minimum for most forms of querying of JSON arrays.
        /// </summary>
        public static string CompatibilityLevelTooLowForScalarCollections(object? compatibilityLevel)
            => string.Format(
                GetString("CompatibilityLevelTooLowForScalarCollections", nameof(compatibilityLevel)),
                compatibilityLevel);

        /// <summary>
        ///     The query is attempting to query a JSON collection of binary data in a context that requires preserving the ordering of the collection; this isn't supported by SQL Server.
        /// </summary>
        public static string QueryingOrderedBinaryJsonCollectionsNotSupported
            => GetString("QueryingOrderedBinaryJsonCollectionsNotSupported");

        /// <summary>
        ///     The query uses 'Skip' without specifying ordering and uses split query mode. This generates incorrect results. Either provide ordering or run query in single query mode using `AsSingleQuery()`. See https://go.microsoft.com/fwlink/?linkid=2196526 for more information.
        /// </summary>
        public static string SplitQueryOffsetWithoutOrderBy
            => GetString("SplitQueryOffsetWithoutOrderBy");
    }
}

namespace Actian.EFCore.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class ActianResources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Actian.EFCore.Properties.ActianStrings", typeof(ActianResources).GetTypeInfo().Assembly);

        /// <summary>
        ///     No type was specified for the decimal column '{property}' on entity type '{entityType}'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values using 'HasColumnType()'.
        /// </summary>
        public static EventDefinition<string, string> LogDefaultDecimalTypeColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogDefaultDecimalTypeColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogDefaultDecimalTypeColumn,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.DecimalTypeDefaultWarning,
                        LogLevel.Warning,
                        "ActianEventId.DecimalTypeDefaultWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.DecimalTypeDefaultWarning,
                            _resourceManager.GetString("LogDefaultDecimalTypeColumn")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     The property '{property}' on entity type '{entityType}' is of type 'byte', but is set up to use a SQL Server identity column. This requires that values starting at 255 and counting down will be used for temporary key values. A temporary key value is needed for every entity inserted in a single call to 'SaveChanges'. Care must be taken that these values do not collide with real key values.
        /// </summary>
        public static EventDefinition<string, string> LogByteIdentityColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogByteIdentityColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogByteIdentityColumn,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.ByteIdentityColumnWarning,
                        LogLevel.Warning,
                        "ActianEventId.ByteIdentityColumnWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.ByteIdentityColumnWarning,
                            _resourceManager.GetString("LogByteIdentityColumn")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     A database type for column '{columnName}' on table '{tableName}' could not be found, the column will be skipped.
        /// </summary>
        public static EventDefinition<string, string> LogColumnWithoutType(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogColumnWithoutType;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogColumnWithoutType,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.ColumnWithoutTypeWarning,
                        LogLevel.Warning,
                        "ActianEventId.ColumnWithoutTypeWarning",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.ColumnWithoutTypeWarning,
                            _resourceManager.GetString("LogColumnWithoutType")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found default schema {defaultSchema}.
        /// </summary>
        public static EventDefinition<string> LogFoundDefaultSchema([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundDefaultSchema;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundDefaultSchema,
                    () => new EventDefinition<string>(
                        logger.Options,
                        ActianEventId.DefaultSchemaFound,
                        LogLevel.Debug,
                        "ActianEventId.DefaultSchemaFound",
                        level => LoggerMessage.Define<string>(
                            level,
                            ActianEventId.DefaultSchemaFound,
                            _resourceManager.GetString("LogFoundDefaultSchema")!)));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Found type alias with name: {alias} which maps to underlying data type {dataType}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundTypeAlias([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundTypeAlias;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundTypeAlias,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.TypeAliasFound,
                        LogLevel.Debug,
                        "ActianEventId.TypeAliasFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.TypeAliasFound,
                            _resourceManager.GetString("LogFoundTypeAlias")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found column with table: {tableName}, column name: {columnName}, ordinal: {ordinal}, data type: {dataType}, maximum length: {maxLength}, precision: {precision}, scale: {scale}, nullable: {isNullable}, identity: {isIdentity}, default value: {defaultValue}, computed value: {computedValue}
        /// </summary>
        public static FallbackEventDefinition LogFoundColumn([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundColumn;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundColumn,
                    () => new FallbackEventDefinition(
                        logger.Options,
                        ActianEventId.ColumnFound,
                        LogLevel.Debug,
                        "ActianEventId.ColumnFound",
                        _resourceManager.GetString("LogFoundColumn")!));
            }

            return (FallbackEventDefinition)definition;
        }

        /// <summary>
        ///     Found foreign key on table: {tableName}, name: {foreignKeyName}, principal table: {principalTableName}, delete action: {deleteAction}.
        /// </summary>
        public static EventDefinition<string, string, string, string> LogFoundForeignKey([NotNull] IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundForeignKey;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundForeignKey,
                    () => new EventDefinition<string, string, string, string>(
                        logger.Options,
                        ActianEventId.ForeignKeyFound,
                        LogLevel.Debug,
                        "ActianEventId.ForeignKeyFound",
                        level => LoggerMessage.Define<string, string, string, string>(
                            level,
                            ActianEventId.ForeignKeyFound,
                            _resourceManager.GetString("LogFoundForeignKey")!)));
            }

            return (EventDefinition<string, string, string, string>)definition;
        }

        /// <summary>
        ///     For foreign key {fkName} on table {tableName}, unable to model the end of the foreign key on principal table {principaltableName}. This is usually because the principal table was not included in the selection set.
        /// </summary>
        public static EventDefinition<string?, string?, string?> LogPrincipalTableNotInSelectionSet(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalTableNotInSelectionSet;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalTableNotInSelectionSet,
                    () => new EventDefinition<string, string, string>(
                        logger.Options,
                        ActianEventId.ForeignKeyReferencesMissingPrincipalTableWarning,
                        LogLevel.Warning,
                        "ActianEventId.ForeignKeyReferencesMissingPrincipalTableWarning",
                        level => LoggerMessage.Define<string, string, string>(
                            level,
                            ActianEventId.ForeignKeyReferencesMissingPrincipalTableWarning,
                            _resourceManager.GetString("LogPrincipalTableNotInSelectionSet")!)));
            }

            return (EventDefinition<string?, string?, string?>)definition;
        }

        /// <summary>
        ///     Unable to find a schema in the database matching the selected schema {schema}.
        /// </summary>
        public static EventDefinition<string?> LogMissingSchema(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingSchema;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingSchema,
                    () => new EventDefinition<string>(
                        logger.Options,
                        ActianEventId.MissingSchemaWarning,
                        LogLevel.Warning,
                        "ActianEventId.MissingSchemaWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            ActianEventId.MissingSchemaWarning,
                            _resourceManager.GetString("LogMissingSchema")!)));
            }

            return (EventDefinition<string?>)definition;
        }

        /// <summary>
        ///     Unable to find a table in the database matching the selected table {table}.
        /// </summary>
        public static EventDefinition<string?> LogMissingTable(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingTable;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingTable,
                    () => new EventDefinition<string>(
                        logger.Options,
                        ActianEventId.MissingTableWarning,
                        LogLevel.Warning,
                        "ActianEventId.MissingTableWarning",
                        level => LoggerMessage.Define<string>(
                            level,
                            ActianEventId.MissingTableWarning,
                            _resourceManager.GetString("LogMissingTable")!)));
            }

            return (EventDefinition<string?>)definition;
        }

        /// <summary>
        ///     Found sequence name: {name}, data type: {dataType}, cyclic: {isCyclic}, increment: {increment}, start: {start}, minimum: {min}, maximum: {max}.
        /// </summary>
        public static FallbackEventDefinition LogFoundSequence(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundSequence;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundSequence,
                    () => new FallbackEventDefinition(
                        logger.Options,
                        ActianEventId.SequenceFound,
                        LogLevel.Debug,
                        "ActianEventId.SequenceFound",
                        _resourceManager.GetString("LogFoundSequence")!));
            }

            return (FallbackEventDefinition)definition;
        }

        /// <summary>
        ///     Found table with name: {name}.
        /// </summary>
        public static EventDefinition<string> LogFoundTable(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundTable;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundTable,
                    () => new EventDefinition<string>(
                        logger.Options,
                        ActianEventId.TableFound,
                        LogLevel.Debug,
                        "ActianEventId.TableFound",
                        level => LoggerMessage.Define<string>(
                            level,
                            ActianEventId.TableFound,
                            _resourceManager.GetString("LogFoundTable")!)));
            }

            return (EventDefinition<string>)definition;
        }

        /// <summary>
        ///     Found index with name: {indexName}, table: {tableName}, is unique: {isUnique}.
        /// </summary>
        public static EventDefinition<string, string, bool> LogFoundIndex(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundIndex;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundIndex,
                    () => new EventDefinition<string, string, bool>(
                        logger.Options,
                        ActianEventId.IndexFound,
                        LogLevel.Debug,
                        "ActianEventId.IndexFound",
                        level => LoggerMessage.Define<string, string, bool>(
                            level,
                            ActianEventId.IndexFound,
                            _resourceManager.GetString("LogFoundIndex")!)));
            }

            return (EventDefinition<string, string, bool>)definition;
        }

        /// <summary>
        ///     Found primary key with name: {primaryKeyName}, table: {tableName}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundPrimaryKey(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundPrimaryKey,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.PrimaryKeyFound,
                        LogLevel.Debug,
                        "ActianEventId.PrimaryKeyFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.PrimaryKeyFound,
                            _resourceManager.GetString("LogFoundPrimaryKey")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Found unique constraint with name: {uniqueConstraintName}, table: {tableName}.
        /// </summary>
        public static EventDefinition<string, string> LogFoundUniqueConstraint(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogFoundUniqueConstraint,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.UniqueConstraintFound,
                        LogLevel.Debug,
                        "ActianEventId.UniqueConstraintFound",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.UniqueConstraintFound,
                            _resourceManager.GetString("LogFoundUniqueConstraint")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     For foreign key {foreignKeyName} on table {tableName}, unable to find the column called {principalColumnName} on the foreign key's principal table, {principaltableName}. Skipping foreign key.
        /// </summary>
        public static EventDefinition<string, string, string, string> LogPrincipalColumnNotFound(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalColumnNotFound,
                    () => new EventDefinition<string, string, string, string>(
                        logger.Options,
                        ActianEventId.ForeignKeyPrincipalColumnMissingWarning,
                        LogLevel.Warning,
                        "ActianEventId.ForeignKeyPrincipalColumnMissingWarning",
                        level => LoggerMessage.Define<string, string, string, string>(
                            level,
                            ActianEventId.ForeignKeyPrincipalColumnMissingWarning,
                            _resourceManager.GetString("LogPrincipalColumnNotFound")!)));
            }

            return (EventDefinition<string, string, string, string>)definition;
        }

        /// <summary>
        ///     Skipping foreign key '{foreignKeyName}' on table '{tableName}' since principal table information is not available. This usually happens when the user doesn't have permission to read data about principal table.
        /// </summary>
        public static EventDefinition<string?, string?> LogPrincipalTableInformationNotFound(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalTableInformationNotFound;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogPrincipalTableInformationNotFound,
                    () => new EventDefinition<string?, string?>(
                        logger.Options,
                        ActianEventId.ForeignKeyReferencesUnknownPrincipalTableWarning,
                        LogLevel.Warning,
                        "ActianEventId.ForeignKeyReferencesUnknownPrincipalTableWarning",
                        level => LoggerMessage.Define<string?, string?>(
                            level,
                            ActianEventId.ForeignKeyReferencesUnknownPrincipalTableWarning,
                            _resourceManager.GetString("LogPrincipalTableInformationNotFound")!)));
            }

            return (EventDefinition<string?, string?>)definition;
        }

        /// <summary>
        ///     Skipping foreign key '{foreignKeyName}' on table '{tableName}' since all of its columns reference themselves.
        /// </summary>
        public static EventDefinition<string, string> LogReflexiveConstraintIgnored(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogReflexiveConstraintIgnored;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized<EventDefinitionBase>(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogReflexiveConstraintIgnored,
                    () => new EventDefinition<string, string>(
                        logger.Options,
                        ActianEventId.ReflexiveConstraintIgnored,
                        LogLevel.Debug,
                        "ActianEventId.ReflexiveConstraintIgnored",
                        level => LoggerMessage.Define<string, string>(
                            level,
                            ActianEventId.ReflexiveConstraintIgnored,
                            _resourceManager.GetString("LogReflexiveConstraintIgnored")!)));
            }

            return (EventDefinition<string, string>)definition;
        }

        /// <summary>
        ///     Skipping foreign key '{foreignKeyName}' on table '{tableName}' since it is a duplicate of '{duplicateForeignKeyName}'.
        /// </summary>
        public static EventDefinition<string, string, string> LogDuplicateForeignKeyConstraintIgnored(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogDuplicateForeignKeyConstraintIgnored;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogDuplicateForeignKeyConstraintIgnored,
                    () => new EventDefinition<string, string, string>(
                        logger.Options,
                        ActianEventId.DuplicateForeignKeyConstraintIgnored,
                        LogLevel.Warning,
                        "ActianEventId.DuplicateForeignKeyConstraintIgnored",
                        level => LoggerMessage.Define<string, string, string>(
                            level,
                            ActianEventId.DuplicateForeignKeyConstraintIgnored,
                            _resourceManager.GetString("LogDuplicateForeignKeyConstraintIgnored")!)));
            }

            return (EventDefinition<string, string, string>)definition;
        }

        /// <summary>
        ///     The database user has not been granted 'VIEW DEFINITION' rights. Scaffolding requires these rights to construct the Entity Framework model correctly. Without these rights, parts of the scaffolded model may be missing, resulting in incorrect interactions between Entity Framework and the database at runtime.
        /// </summary>
        public static EventDefinition LogMissingViewDefinitionRights(IDiagnosticsLogger logger)
        {
            var definition = ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingViewDefinitionRights;
            if (definition == null)
            {
                definition = LazyInitializer.EnsureInitialized(
                    ref ((Diagnostics.Internal.ActianLoggingDefinitions)logger.Definitions).LogMissingViewDefinitionRights,
                    () => new EventDefinition(
                        logger.Options,
                        ActianEventId.MissingViewDefinitionRightsWarning,
                        LogLevel.Warning,
                        "ActianEventId.MissingViewDefinitionRightsWarning",
                        level => LoggerMessage.Define(
                            level,
                            ActianEventId.MissingViewDefinitionRightsWarning,
                            _resourceManager.GetString("LogMissingViewDefinitionRights")!)));
            }

            return (EventDefinition)definition;
        }
    }
}
