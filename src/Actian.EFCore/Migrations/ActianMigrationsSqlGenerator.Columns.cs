﻿using System;
using System.Collections.Generic;
using System.Linq;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Migrations
{
    partial class ActianMigrationsSqlGenerator
    {
        protected override void Generate(
            [NotNull] AddColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ADD ");

            ColumnDefinition(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);

                ModifyToReconstruct(builder, operation.Schema, operation.Table);
            }
        }

        protected override void Generate(
            [NotNull] RenameColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" RENAME COLUMN ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(DelimitIdentifier(operation.NewName))
                .AppendLine(StatementTerminator);

            EndStatement(builder);

            ModifyToReconstruct(builder, operation.Schema, operation.Table);
        }

        protected override void Generate(
            [NotNull] AlterColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var indexesToRebuild = null as IEnumerable<IIndex>;
            var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);

            if (operation.ComputedColumnSql != null)
            {
                // TODO: Check that Actian databases do not support computed columns
                throw new NotSupportedException("Actian databases do not support computed columns");
            }

            var narrowed = false;
            if (IsOldColumnSupported(model))
            {
                if (IsIdentity(operation) != IsIdentity(operation.OldColumn))
                {
                    throw new InvalidOperationException(ActianStrings.AlterIdentityColumn);
                }

                var type = operation.ColumnType ?? GetColumnType(
                    operation.Schema,
                    operation.Table,
                    operation.Name,
                    operation,
                    model
                );

                var oldType = operation.OldColumn.ColumnType ?? GetColumnType(
                    operation.Schema,
                    operation.Table,
                    operation.Name,
                    operation.OldColumn,
                    model
                );

                narrowed = type != oldType || !operation.IsNullable && operation.OldColumn.IsNullable;
            }

            if (narrowed)
            {
                indexesToRebuild = GetIndexesToRebuild(property, operation).ToList();
                DropIndexes(indexesToRebuild, builder);
            }

            builder
                .Append("ALTER TABLE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" ALTER COLUMN ");

            // NB: Identity is handled elsewhere. Don't copy them here.
            var definitionOperation = new AlterColumnOperation
            {
                Schema = operation.Schema,
                Table = operation.Table,
                Name = operation.Name,
                ClrType = operation.ClrType,
                ColumnType = operation.ColumnType,
                IsUnicode = operation.IsUnicode,
                IsFixedLength = operation.IsFixedLength,
                MaxLength = operation.MaxLength,
                IsRowVersion = operation.IsRowVersion,
                IsNullable = operation.IsNullable,
                ComputedColumnSql = operation.ComputedColumnSql,
                OldColumn = operation.OldColumn,
                DefaultValue = operation.DefaultValue,
                DefaultValueSql = operation.DefaultValueSql
            };

            definitionOperation.AddAnnotations(operation.GetAnnotations()
                .Where(a => a.Name != ActianAnnotationNames.ValueGenerationStrategy && a.Name != ActianAnnotationNames.Identity)
            );

            ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                definitionOperation,
                model,
                builder
            );

            builder.AppendLine(StatementTerminator);

            if (operation.OldColumn.Comment != operation.Comment)
            {
                CommentOnColumn(
                    builder,
                    operation.Schema,
                    operation.Table,
                    operation.Name,
                    operation.Comment
                );
            }

            if (narrowed)
            {
                CreateIndexes(indexesToRebuild, builder);
            }

            EndStatement(builder);

            ModifyToReconstruct(builder, operation.Schema, operation.Table);
        }

        protected override void Generate(
            [NotNull] DropColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" DROP COLUMN ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT");

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);

                ModifyToReconstruct(builder, operation.Schema, operation.Table);
            }
        }

        protected override void CreateTableColumns(
            [NotNull] CreateTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            for (var i = 0; i < operation.Columns.Count; i++)
            {
                var column = operation.Columns[i];
                ColumnDefinition(column, model, builder);

                if (i != operation.Columns.Count - 1)
                {
                    builder.AppendLine(",");
                }
            }
        }

        protected override void ColumnDefinition(
            [NotNull] AddColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
            => ColumnDefinition(
                operation.Schema,
                operation.Table,
                operation.Name,
                operation,
                model,
                builder);

        protected override void ColumnDefinition(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] ColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.ComputedColumnSql != null)
            {
                ComputedColumnDefinition(schema, table, name, operation, model, builder);
                return;
            }

            var columnType = operation.ColumnType ?? GetColumnType(schema, table, name, operation, model);
            builder
                .Append(DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType);

            DefaultValue(operation.DefaultValue, operation.DefaultValueSql, columnType, builder);

            builder.Append(operation.IsNullable ? " WITH NULL" : " NOT NULL");
        }

        protected override void ComputedColumnDefinition(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] ColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            // TODO: Check that Actian databases do not support computed columns
            throw new NotSupportedException("Actian databases do not support computed columns");
        }

        protected override string GetColumnType(
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string name,
            [NotNull] ColumnOperation operation,
            [CanBeNull] IModel model)
        {
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));

            var keyOrIndex = false;

            var property = FindProperty(model, schema, table, name);
            if (property != null)
            {
                if (operation.IsUnicode == property.IsUnicode()
                    && operation.MaxLength == property.GetMaxLength()
                    && (operation.IsFixedLength ?? false) == property.IsFixedLength()
                    && operation.IsRowVersion == (property.IsConcurrencyToken && property.ValueGenerated == ValueGenerated.OnAddOrUpdate))
                {
                    return Dependencies.TypeMappingSource.FindMapping(property).StoreType;
                }

                keyOrIndex = property.IsKey() || property.IsForeignKey();
            }

            return Dependencies.TypeMappingSource.FindMapping(
                    operation.ClrType,
                    null,
                    keyOrIndex,
                    operation.IsUnicode,
                    operation.MaxLength,
                    operation.IsRowVersion,
                    operation.IsFixedLength)
                .StoreType;
        }

        protected override void DefaultValue(
            [CanBeNull] object defaultValue,
            [CanBeNull] string defaultValueSql,
            [CanBeNull] string columnType,
            [NotNull] MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (defaultValueSql != null)
            {
                builder
                    .Append(" DEFAULT (")
                    .Append(defaultValueSql)
                    .Append(")");
            }
            else if (defaultValue != null)
            {
                var typeMapping = columnType != null
                    ? Dependencies.TypeMappingSource.FindMapping(defaultValue.GetType(), columnType)
                    : null;

                if (typeMapping == null)
                {
                    typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);
                }

                builder
                    .Append(" DEFAULT ")
                    .Append(typeMapping.GenerateSqlLiteral(defaultValue));
            }
        }

        private void ModifyToReconstruct(
            [NotNull] MigrationCommandListBuilder builder,
            [CanBeNull] string schema,
            [NotNull] string tableName
            )
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotEmpty(tableName, nameof(tableName));

            SetCurrentUser(schema, builder);

            builder
                .Append("MODIFY ")
                .Append(DelimitIdentifier(tableName, schema))
                .Append(" TO RECONSTRUCT")
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        private static bool IsIdentity(ColumnOperation operation)
            => operation[ActianAnnotationNames.Identity] != null
            || operation[ActianAnnotationNames.ValueGenerationStrategy] as ActianValueGenerationStrategy? == ActianValueGenerationStrategy.IdentityColumn;
    }
}