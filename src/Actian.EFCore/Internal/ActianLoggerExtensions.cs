// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

#nullable enable
namespace Actian.EFCore.Internal
{
    public static class ActianLoggerExtensions
    {
        public static void DecimalTypeDefaultWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            IProperty property)
        {
            var definition = ActianResources.LogDefaultDecimalTypeColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, property.Name, property.DeclaringType.DisplayName());
            }

            if (diagnostics.NeedsEventData(definition, out var diagnosticSourceEnabled, out var simpleLogEnabled))
            {
                var eventData = new PropertyEventData(
                    definition,
                    DecimalTypeDefaultWarning,
                    property);

                diagnostics.DispatchEventData(definition, eventData, diagnosticSourceEnabled, simpleLogEnabled);
            }
        }

        private static string DecimalTypeDefaultWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (PropertyEventData)payload;
            return d.GenerateMessage(
                p.Property.Name,
                p.Property.DeclaringType.DisplayName());
        }

        public static void ByteIdentityColumnWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            IProperty property)
        {
            var definition = ActianResources.LogByteIdentityColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, property.Name, property.DeclaringType.DisplayName());
            }

            if (diagnostics.NeedsEventData(definition, out var diagnosticSourceEnabled, out var simpleLogEnabled))
            {
                var eventData = new PropertyEventData(
                    definition,
                    ByteIdentityColumnWarning,
                    property);

                diagnostics.DispatchEventData(definition, eventData, diagnosticSourceEnabled, simpleLogEnabled);
            }
        }

        private static string ByteIdentityColumnWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (PropertyEventData)payload;
            return d.GenerateMessage(
                p.Property.Name,
                p.Property.DeclaringType.DisplayName());
        }

        public static void ColumnFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName,
            string columnName,
            int ordinal,
            string dataTypeName,
            int maxLength,
            int precision,
            int scale,
            bool nullable,
            bool identity,
            string? defaultValue,
            string? computedValue,
            bool? stored)
        {
            var definition = ActianResources.LogFoundColumn(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(
                    diagnostics,
                    l => l.LogDebug(
                        definition.EventId,
                        null,
                        definition.MessageFormat,
                        tableName,
                        columnName,
                        ordinal,
                        dataTypeName,
                        maxLength,
                        precision,
                        scale,
                        nullable,
                        identity,
                        defaultValue,
                        computedValue,
                        stored));
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ForeignKeyFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName,
            string tableName,
            string principalTableName,
            string onDeleteAction)
        {
            var definition = ActianResources.LogFoundForeignKey(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalTableName, onDeleteAction);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void DefaultSchemaFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string schemaName)
        {
            var definition = ActianResources.LogFoundDefaultSchema(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, schemaName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void TypeAliasFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string typeAliasName,
            string systemTypeName)
        {
            var definition = ActianResources.LogFoundTypeAlias(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, typeAliasName, systemTypeName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void PrimaryKeyFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string primaryKeyName,
            string tableName)
        {
            var definition = ActianResources.LogFoundPrimaryKey(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, primaryKeyName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void UniqueConstraintFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string uniqueConstraintName,
            string tableName)
        {
            var definition = ActianResources.LogFoundUniqueConstraint(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, uniqueConstraintName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void IndexFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string indexName,
            string tableName,
            bool unique)
        {
            var definition = ActianResources.LogFoundIndex(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, indexName, tableName, unique);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ForeignKeyReferencesUnknownPrincipalTableWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string? foreignKeyName,
            string? tableName)
        {
            var definition = ActianResources.LogPrincipalTableInformationNotFound(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ForeignKeyReferencesMissingPrincipalTableWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string? foreignKeyName,
            string? tableName,
            string? principalTableName)
        {
            var definition = ActianResources.LogPrincipalTableNotInSelectionSet(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalTableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ForeignKeyPrincipalColumnMissingWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName,
            string tableName,
            string principalColumnName,
            string principalTableName)
        {
            var definition = ActianResources.LogPrincipalColumnNotFound(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, principalColumnName, principalTableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void MissingSchemaWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string? schemaName)
        {
            var definition = ActianResources.LogMissingSchema(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, schemaName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void MissingTableWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string? tableName)
        {
            var definition = ActianResources.LogMissingTable(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ColumnWithoutTypeWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName,
            string columnName)
        {
            var definition = ActianResources.LogColumnWithoutType(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, tableName, columnName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void SequenceFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string sequenceName,
            string sequenceTypeName,
            bool cyclic,
            int increment,
            long start,
            long min,
            long max,
            bool cached,
            int? cacheSize)
        {
            // No DiagnosticsSource events because these are purely design-time messages
            var definition = ActianResources.LogFoundSequence(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(
                    diagnostics,
                    l => l.LogDebug(
                        definition.EventId,
                        null,
                        definition.MessageFormat,
                        sequenceName,
                        sequenceTypeName,
                        cyclic,
                        increment,
                        start,
                        min,
                        max,
                        cached,
                        cacheSize));
            }
        }

        public static void TableFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName)
        {
            var definition = ActianResources.LogFoundTable(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void ReflexiveConstraintIgnored(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName,
            string tableName)
        {
            var definition = ActianResources.LogReflexiveConstraintIgnored(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void DuplicateForeignKeyConstraintIgnored(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName,
            string tableName,
            string duplicateForeignKeyName)
        {
            var definition = ActianResources.LogDuplicateForeignKeyConstraintIgnored(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics, foreignKeyName, tableName, duplicateForeignKeyName);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }

        public static void MissingViewDefinitionRightsWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics)
        {
            var definition = ActianResources.LogMissingViewDefinitionRights(diagnostics);

            if (diagnostics.ShouldLog(definition))
            {
                definition.Log(diagnostics);
            }

            // No DiagnosticsSource events because these are purely design-time messages
        }
    }
}
