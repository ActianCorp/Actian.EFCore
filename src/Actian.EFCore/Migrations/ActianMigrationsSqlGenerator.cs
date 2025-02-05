using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Actian.EFCore.Extensions.Internal;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Migrations.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace Actian.EFCore.Migrations
{
#nullable enable
    public class ActianMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private IReadOnlyList<MigrationOperation> _operations = null!;

        private readonly ICommandBatchPreparer _commandBatchPreparer;

        private string? _currentUserName = null;
        private string StatementTerminator => Dependencies.SqlGenerationHelper.StatementTerminator;

        public ActianMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        ICommandBatchPreparer commandBatchPreparer)
        : base(dependencies)
        {
            _commandBatchPreparer = commandBatchPreparer;
        }

        /// <summary>
        ///     Generates commands from a list of operations.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="model">The target model which may be <see langword="null" /> if the operations exist without a model.</param>
        /// <param name="options">The options to use when generating commands.</param>
        /// <returns>The list of commands to be executed or scripted.</returns>
        public override IReadOnlyList<MigrationCommand> Generate(
            IReadOnlyList<MigrationOperation> operations,
            IModel? model = null,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        {
            _operations = operations;
            try
            {
                return base.Generate(RewriteOperations(operations, model, options), model, options);
            }
            finally
            {
                _operations = null!;
            }
        }

        #region Databases

        protected override void Generate(
            AlterDatabaseOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(model, nameof(model));
            Check.NotNull(builder, nameof(builder));

            throw new NotSupportedException("Actian does not support altering an existing database.");
        }

        #endregion Databases

        #region Schemas

        protected override void Generate(
            EnsureSchemaOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

        }

        protected override void Generate(
            DropSchemaOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Name, builder);

            builder
                .Append("DROP SCHEMA ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT")
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        #endregion Schemas

        #region Tables

        protected override void Generate(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE TABLE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!))
                .AppendLine(" (");

            using (builder.Indent())
            {
                CreateTableColumns(operation, model, builder);
                CreateTableConstraints(operation, model, builder);
                builder.AppendLine();
            }

            builder.Append(")");

            builder.WithClause()
                .Locations(operation[ActianAnnotationNames.Locations] as IEnumerable<string>)
                .Journaling(operation[ActianAnnotationNames.Journaling] as bool?)
                .Duplicates(operation[ActianAnnotationNames.Duplicates] as bool?)
                .Build();

            builder.AppendLine(StatementTerminator);

            if (!string.IsNullOrEmpty(operation.Comment))
            {
                CommentOnTable(builder, operation.Schema!, operation.Name, operation.Comment);
            }

            if (terminate)
            {
                EndStatement(builder);
            }
        }

        protected override void Generate(
            RenameTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (string.IsNullOrEmpty(operation.NewSchema))
            {
                operation.NewSchema = operation.Schema;
            }

            if (operation.NewSchema != operation.Schema)
                throw new InvalidOperationException(ActianStrings.RenameTableToDifferentSchema(operation.Schema!, operation.Name, operation.NewSchema!, operation.NewName!));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!))
                .Append(" RENAME TO ")
                .Append(DelimitIdentifier(operation.NewName!));

            builder.AppendLine(StatementTerminator);
            EndStatement(builder);

            ModifyToReconstruct(builder, operation.Schema!, operation.NewName!);
        }

        protected override void Generate(
            AlterTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
                CommentOnTable(builder, operation.Schema!, operation.Name, operation.Comment!);
            EndStatement(builder);
        }

        protected override void Generate(
            DropTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                        .Append("DROP ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!));

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void CreateTableConstraints(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            CreateTablePrimaryKeyConstraint(operation, model, builder);
            CreateTableUniqueConstraints(operation, model, builder);
            CreateTableCheckConstraints(operation, model, builder);
            CreateTableForeignKeys(operation, model, builder);
        }

        #endregion Tables

        #region Columns

        protected override void Generate(
            AddColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            ClearDefaultValueIfIdentity(operation);

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" ADD ");

            ColumnDefinition(operation, model!, builder);

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);

                ModifyToReconstruct(builder, operation.Schema!, operation.Table);
            }
        }

        protected override void Generate(
            RenameColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" RENAME COLUMN ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(DelimitIdentifier(operation.NewName))
                .AppendLine(StatementTerminator);

            EndStatement(builder);

            ModifyToReconstruct(builder, operation.Schema, operation.Table);
        }

        protected override void Generate(
            AlterColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            IEnumerable<ITableIndex>? indexesToRebuild = null;
            var property = model?.GetRelationalModel().FindTable(operation.Table, operation.Schema)?.FindColumn(operation.Name);

            if (operation.ComputedColumnSql != null)
            {
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

            definitionOperation.AddAnnotations(operation.GetAnnotations());

            ClearDefaultValueIfIdentity(definitionOperation);

            ColumnDefinition(
                operation.Schema!,
                operation.Table,
                operation.Name,
                definitionOperation,
                model!,
                builder
            );

            builder.AppendLine(StatementTerminator);

            if (operation.OldColumn.Comment != operation.Comment)
            {
                CommentOnColumn(
                    builder,
                    operation.Schema!,
                    operation.Table,
                    operation.Name,
                    operation.Comment!
                );
            }

            if (narrowed)
            {
                CreateIndexes(indexesToRebuild!, builder);
            }

            EndStatement(builder);

            ModifyToReconstruct(builder, operation.Schema!, operation.Table);
        }

        protected override void Generate(
            DropColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" DROP COLUMN ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT");

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);

                ModifyToReconstruct(builder, operation.Schema!, operation.Table);
            }
        }

        protected override void CreateTableColumns(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            for (var i = 0; i < operation.Columns.Count; i++)
            {
                var column = operation.Columns[i];
                ColumnDefinition(column, model!, builder);

                if (i != operation.Columns.Count - 1)
                {
                    builder.AppendLine(",");
                }
            }
        }

        protected override void ColumnDefinition(
            AddColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
            => ColumnDefinition(
                operation.Schema!,
                operation.Table,
                operation.Name,
                operation,
                model!,
                builder);

        protected override void ColumnDefinition(
            string? schema,
            string table,
            string name,
            ColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.ComputedColumnSql != null)
            {
                ComputedColumnDefinition(schema!, table, name, operation, model!, builder);
                return;
            }

            var columnType = operation.ColumnType ?? GetColumnType(schema, table, name, operation, model);
            builder
                .Append(DelimitIdentifier(name))
                .Append(" ")
                .Append(columnType!);

            builder.Append(operation.IsNullable ? " WITH NULL" : " NOT NULL");

            if (IdentityDefinition(operation, builder))
                return;

            if (!operation.IsNullable && operation.DefaultValue is null && operation.DefaultValueSql is null)
            {
                operation.DefaultValueSql = GetDefaultValueSql(columnType!);
            }

            DefaultValue(operation.DefaultValue!, operation.DefaultValueSql!, columnType!, builder);
        }

        protected virtual bool IdentityDefinition(ColumnOperation operation, MigrationCommandListBuilder builder)
        {
            var identity = operation[ActianAnnotationNames.Identity] as string;
            var valueGenerationStrategy = operation[ActianAnnotationNames.ValueGenerationStrategy] as ActianValueGenerationStrategy?;

            if (identity != null)
            {
                builder.Append(" GENERATED BY DEFAULT AS IDENTITY");
            }
            else if (valueGenerationStrategy.IsIdentity())
            {
                builder.Append(valueGenerationStrategy switch
                {
                    ActianValueGenerationStrategy.IdentityAlwaysColumn => " GENERATED ALWAYS AS IDENTITY",
                    ActianValueGenerationStrategy.IdentityByDefaultColumn => " GENERATED BY DEFAULT AS IDENTITY",
                    _ => ""
                });
            }
            else
            {
                return false;
            }

            if (operation[ActianAnnotationNames.IdentityOptions] is string identitySequenceOptions)
            {
                var options = IdentitySequenceOptionsData.Deserialize(identitySequenceOptions);

                var prefix = " (";

                var incrementBy = options.IncrementBy;

                var defaultMinValue = incrementBy > 0 ? 1 : operation.ClrType.MinLongValue();
                var defaultMaxValue = incrementBy > 0 ? operation.ClrType.MaxLongValue() : -1;

                var minValue = options.MinValue ?? defaultMinValue;
                var maxValue = options.MaxValue ?? defaultMaxValue;

                var defaultStartValue = incrementBy > 0 ? minValue : maxValue;
                if (options.StartValue.HasValue && options.StartValue != defaultStartValue)
                    AppendWithPrefix("START WITH ", options.StartValue);

                if (incrementBy != 1)
                    AppendWithPrefix("INCREMENT BY ", incrementBy);

                if (minValue != defaultMinValue)
                    AppendWithPrefix("MINVALUE ", minValue);

                if (maxValue != defaultMaxValue)
                    AppendWithPrefix("MAXVALUE ", maxValue);

                if (options.IsCyclic)
                    AppendWithPrefix("CYCLE");

                if (options.NumbersToCache != 1)
                    AppendWithPrefix("CACHE ", options.NumbersToCache);

                if (prefix != " (")
                    builder.Append(")");

                void AppendWithPrefix(params object[] values)
                {
                    builder.Append(prefix);
                    prefix = " ";
                    foreach (string value in values)
                    {
                        builder.Append(value);
                    }
                }
            }

            return true;
        }


        private string? GetDefaultValueSql(string columnType) => columnType!.ToLowerInvariant() switch
        {
            "money" => "0",
            _ when columnType.Contains("interval") => "'0 00:00:00.000'",
            _ when columnType.Contains("int") => "0",
            _ when columnType.Contains("float") => "0",
            _ when columnType.Contains("decimal") => "0",
            _ when columnType.Contains("char") => "''",
            _ => null
        };

        protected override void ComputedColumnDefinition(
            string? schema,
            string table,
            string name,
            ColumnOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
                throw new NotSupportedException("Actian databases do not support computed columns");
        }

        /// <summary>
        ///     Gets the store/database type of a column given the provided metadata.
        /// </summary>
        /// <param name="schema">The schema that contains the table, or <see langword="null" /> to use the default schema.</param>
        /// <param name="tableName">The table that contains the column.</param>
        /// <param name="name">The column name.</param>
        /// <param name="operation">The column metadata.</param>
        /// <param name="model">The target model which may be <see langword="null" /> if the operations exist without a model.</param>
        /// <returns>The database/store type for the column.</returns>
        protected override string? GetColumnType(
            string? schema,
            string tableName,
            string name,
            ColumnOperation operation,
            IModel? model)
        {
            var keyOrIndex = false;

            var table = model?.GetRelationalModel().FindTable(tableName, schema);
            var column = table?.FindColumn(name);
            if (column != null)
            {
                if (operation.IsUnicode == column.IsUnicode
                    && operation.MaxLength == column.MaxLength
                    && operation.Precision == column.Precision
                    && operation.Scale == column.Scale
                    && operation.IsFixedLength == column.IsFixedLength
                    && operation.IsRowVersion == column.IsRowVersion)
                {
                    return column.StoreType;
                }

                keyOrIndex = table!.UniqueConstraints.Any(u => u.Columns.Contains(column))
                    || table.ForeignKeyConstraints.Any(u => u.Columns.Contains(column))
                    || table.Indexes.Any(u => u.Columns.Contains(column));
            }

            return Dependencies.TypeMappingSource.FindMapping(
                    operation.ClrType,
                    null,
                    keyOrIndex,
                    operation.IsUnicode,
                    operation.MaxLength,
                    operation.IsRowVersion,
                    operation.IsFixedLength,
                    operation.Precision,
                    operation.Scale)
                ?.StoreType;
        }

        protected override void DefaultValue(
            object? defaultValue,
            string? defaultValueSql,
            string? columnType,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (defaultValueSql != null)
            {
                builder
                    .Append(" WITH DEFAULT ")
                    .Append(defaultValueSql);
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
                    .Append(" WITH DEFAULT ")
                    .Append(typeMapping.GenerateSqlLiteral(defaultValue));
            }
        }

        private void ModifyToReconstruct(
            MigrationCommandListBuilder builder,
            string? schema,
            [NotNull] string tableName
            )
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotEmpty(tableName, nameof(tableName));

            SetCurrentUser(schema!, builder);

            builder
                .Append("MODIFY ")
                .Append(DelimitIdentifier(tableName, schema!))
                .Append(" TO RECONSTRUCT")
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        private static bool IsIdentity(ColumnOperation operation)
            => operation[ActianAnnotationNames.Identity] != null
                || operation[ActianAnnotationNames.ValueGenerationStrategy] as ActianValueGenerationStrategy?
                == ActianValueGenerationStrategy.IdentityColumn
                || operation[ActianAnnotationNames.ValueGenerationStrategy] as ActianValueGenerationStrategy?
                == ActianValueGenerationStrategy.IdentityByDefaultColumn;

        private TColumnOperation ClearDefaultValueIfIdentity<TColumnOperation>(TColumnOperation operation)
            where TColumnOperation : ColumnOperation
        {
            if (operation[ActianAnnotationNames.ValueGenerationStrategy] is ActianValueGenerationStrategy strategy)
            {
                switch (strategy)
                {
                    case ActianValueGenerationStrategy.IdentityAlwaysColumn:
                    case ActianValueGenerationStrategy.IdentityByDefaultColumn:
                                                        operation.DefaultValue = null;
                        break;
                }
            }
            return operation;
        }

        #endregion Columns

        #region Primary Keys

        protected override void Generate(
            AddPrimaryKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, model!, builder);

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                        .Append(" DROP CONSTRAINT ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT");

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void CreateTablePrimaryKeyConstraint(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.PrimaryKey != null)
            {
                builder.AppendLine(",");
                PrimaryKeyConstraint(operation.PrimaryKey, model, builder);
            }
        }

        protected override void PrimaryKeyConstraint(
            AddPrimaryKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("PRIMARY KEY ");

            IndexTraits(operation, model!, builder);

            builder.Append("(")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        #endregion Primary Keys

        #region Foreign Keys

        protected override void Generate(
            AddForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" ADD ");

            ForeignKeyConstraint(operation, model!, builder);

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(
            DropForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" DROP CONSTRAINT ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT");

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void CreateTableForeignKeys(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            foreach (var foreignKey in operation.ForeignKeys)
            {
                builder.AppendLine(",");
                ForeignKeyConstraint(foreignKey, model, builder);
            }
        }

        protected override void ForeignKeyConstraint(
            AddForeignKeyOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("FOREIGN KEY (")
                .Append(ColumnList(operation.Columns))
                .Append(") REFERENCES ")
                .Append(DelimitIdentifier(operation.PrincipalTable, operation.PrincipalSchema!));

            if (operation.PrincipalColumns != null)
            {
                builder
                    .Append(" (")
                    .Append(ColumnList(operation.PrincipalColumns))
                    .Append(")");
            }

            if (operation.OnUpdate != ReferentialAction.NoAction)
            {
                builder.Append(" ON UPDATE ");
                ForeignKeyAction(operation.OnUpdate, builder);
            }

            if (operation.OnDelete != ReferentialAction.NoAction)
            {
                builder.Append(" ON DELETE ");
                ForeignKeyAction(operation.OnDelete, builder);
            }
        }

        protected override void ForeignKeyAction(
            ReferentialAction referentialAction,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            switch (referentialAction)
            {
                case ReferentialAction.Restrict:
                    builder.Append("RESTRICT");
                    break;
                case ReferentialAction.Cascade:
                    builder.Append("CASCADE");
                    break;
                case ReferentialAction.SetNull:
                    builder.Append("SET NULL");
                    break;
                case ReferentialAction.SetDefault:
                    builder.Append("SET DEFAULT");
                    break;
                default:
                    Debug.Assert(
                        referentialAction == ReferentialAction.NoAction,
                        "Unexpected value: " + referentialAction);
                    break;
            }
        }

        #endregion Foreign Keys

        #region Unique Constraints

        protected override void Generate(
            AddUniqueConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" ADD ");
            UniqueConstraint(operation, model!, builder);
            builder.AppendLine(StatementTerminator);
            EndStatement(builder);
        }

        protected override void Generate(
            DropUniqueConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" DROP CONSTRAINT ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" RESTRICT")
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void CreateTableUniqueConstraints(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            foreach (var uniqueConstraint in operation.UniqueConstraints)
            {
                builder.AppendLine(",");
                UniqueConstraint(uniqueConstraint, model, builder);
            }
        }

        protected override void UniqueConstraint(
            AddUniqueConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("UNIQUE ");

            IndexTraits(operation, model, builder);

            builder.Append("(")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        #endregion Unique Constraints

        #region Check Constraints

        protected override void Generate(
            AddCheckConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" ADD ");
            CheckConstraint(operation, model, builder);
            builder.AppendLine(StatementTerminator);
            EndStatement(builder);
        }

        protected override void Generate(
            DropCheckConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" DROP CONSTRAINT ")
                .Append(DelimitIdentifier(operation.Name))
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void CreateTableCheckConstraints(
            CreateTableOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            foreach (var checkConstraint in operation.CheckConstraints)
            {
                builder.AppendLine(",");
                CheckConstraint(checkConstraint, model, builder);
            }
        }

        protected override void CheckConstraint(
            AddCheckConstraintOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("CHECK ");

            builder.Append("(")
                .Append(operation.Sql)
                .Append(")");
        }

        #endregion Check Constraints

        #region Indexes

        protected override void Generate(
            CreateIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            var nullableColumns = operation.Columns
                .Where(column =>
                {
                    var property = model?.GetRelationalModel().FindTable(operation.Table, operation.Schema!)!.FindColumn(column);
                    return property?.IsNullable != false;
                })
                .ToList();

            builder.Append("CREATE ");

            if (operation.IsUnique && !nullableColumns.Any())
            {
                builder.Append("UNIQUE ");
            }

            IndexTraits(operation, model, builder);

            builder
                .Append("INDEX ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema!))
                .Append(" (")
                .Append(ColumnList(operation.Columns))
                .Append(")");

            IndexOptions(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(
            DropIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                        .Append("DROP INDEX ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!));

            if (terminate)
            {
                builder.AppendLine(StatementTerminator);
                EndStatement(builder);
            }
        }

        protected override void Generate(
            RenameIndexOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
                throw new NotImplementedException();
        }

        protected override void IndexTraits(
            MigrationOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        ///     Generates a SQL fragment for extras (filter, included columns, options) of an index from a <see cref="CreateIndexOperation" />.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="model">The target model which may be <see langword="null" /> if the operations exist without a model.</param>
        /// <param name="builder">The command builder to use to add the SQL fragment.</param>
        protected override void IndexOptions(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            if (operation[ActianAnnotationNames.Include] is IReadOnlyList<string> includeColumns
                && includeColumns.Count > 0)
            {
                builder.Append(" INCLUDE (");
                for (var i = 0; i < includeColumns.Count; i++)
                {
                    builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(includeColumns[i]));

                    if (i != includeColumns.Count - 1)
                    {
                        builder.Append(", ");
                    }
                }

                builder.Append(")");
            }

            if (operation is CreateIndexOperation createIndexOperation)
            {
                if (!string.IsNullOrEmpty(createIndexOperation.Filter))
                {
                    builder
                        .Append(" WHERE ")
                        .Append(createIndexOperation.Filter);
                }
                else if (UseLegacyIndexFilters(createIndexOperation, model))
                {
                    var table = model?.GetRelationalModel().FindTable(createIndexOperation.Table, createIndexOperation.Schema);
                    var nullableColumns = createIndexOperation.Columns
                        .Where(c => table?.FindColumn(c)?.IsNullable != false)
                        .ToList();

                    builder.Append(" WHERE ");
                    for (var i = 0; i < nullableColumns.Count; i++)
                    {
                        if (i != 0)
                        {
                            builder.Append(" AND ");
                        }

                        builder
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(nullableColumns[i]))
                            .Append(" IS NOT NULL");
                    }
                }
            }

            IndexWithOptions(operation, builder);
        }

        private static void IndexWithOptions(MigrationOperation operation, MigrationCommandListBuilder builder)
        {
            var options = new List<string>();

            if (operation[ActianAnnotationNames.FillFactor] is int fillFactor)
            {
                options.Add("FILLFACTOR = " + fillFactor);
            }

            if (operation[ActianAnnotationNames.CreatedOnline] is bool isOnline && isOnline)
            {
                options.Add("ONLINE = ON");
            }

            if (operation[ActianAnnotationNames.SortInTempDb] is bool sortInTempDb && sortInTempDb)
            {
                options.Add("SORT_IN_TEMPDB = ON");
            }

            if (operation[ActianAnnotationNames.DataCompression] is DataCompressionType dataCompressionType)
            {
                switch (dataCompressionType)
                {
                    case DataCompressionType.None:
                        options.Add("DATA_COMPRESSION = NONE");
                        break;
                    case DataCompressionType.Row:
                        options.Add("DATA_COMPRESSION = ROW");
                        break;
                    case DataCompressionType.Page:
                        options.Add("DATA_COMPRESSION = PAGE");
                        break;
                }
            }

            if (options.Count > 0)
            {
                builder
                    .Append(" WITH (")
                    .Append(string.Join(", ", options))
                    .Append(")");
            }
        }

        /// <summary>
        ///     Checks whether or not <see cref="CreateIndexOperation" /> should have a filter generated for it by
        ///     Migrations.
        /// </summary>
        /// <param name="operation">The index creation operation.</param>
        /// <param name="model">The target model.</param>
        /// <returns><see langword="true" /> if a filter should be generated.</returns>
        protected virtual bool UseLegacyIndexFilters(CreateIndexOperation operation, IModel? model)
            => (!TryGetVersion(model, out var version) || VersionComparer.Compare(version, "2.0.0") < 0)
                && operation.Filter is null
                && operation.IsUnique
                && operation[ActianAnnotationNames.Clustered] is null or false
                && model?.GetRelationalModel().FindTable(operation.Table, operation.Schema) is var table
                && operation.Columns.Any(c => table?.FindColumn(c)?.IsNullable != false);

        /// <summary>
        ///     Gets the list of indexes that need to be rebuilt when the given column is changing.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="currentOperation">The operation which may require a rebuild.</param>
        /// <returns>The list of indexes affected.</returns>
        protected virtual IEnumerable<ITableIndex> GetIndexesToRebuild(
            IColumn? column,
            MigrationOperation currentOperation)
        {
            {
                if (column == null)
                {
                    yield break;
                }

                var table = column.Table;
                var createIndexOperations = _operations.SkipWhile(o => o != currentOperation).Skip(1)
                    .OfType<CreateIndexOperation>().Where(o => o.Table == table.Name && o.Schema == table.Schema).ToList();
                foreach (var index in table.Indexes)
                {
                    var indexName = index.Name;
                    if (createIndexOperations.Any(o => o.Name == indexName))
                    {
                        continue;
                    }

                    if (index.Columns.Any(c => c == column))
                    {
                        yield return index;
                    }
                    else if (index[ActianAnnotationNames.Include] is IReadOnlyList<string> includeColumns
                             && includeColumns.Contains(column.Name))
                    {
                        yield return index;
                    }
                }
            }
        }

        /// <summary>
        ///     Generates SQL to drop the given indexes.
        /// </summary>
        /// <param name="indexes">The indexes to drop.</param>
        /// <param name="builder">The command builder to use to build the commands.</param>
        protected virtual void DropIndexes(
            IEnumerable<ITableIndex> indexes,
            MigrationCommandListBuilder builder)
        {
            foreach (var index in indexes)
            {
                var table = index.Table;
                var operation = new DropIndexOperation
                {
                    Schema = table.Schema,
                    Table = table.Name,
                    Name = index.Name
                };
                operation.AddAnnotations(index.GetAnnotations());

                Generate(operation, table.Model.Model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            }
        }

        /// <summary>
        ///     Generates SQL to create the given indexes.
        /// </summary>
        /// <param name="indexes">The indexes to create.</param>
        /// <param name="builder">The command builder to use to build the commands.</param>
        protected virtual void CreateIndexes(
            IEnumerable<ITableIndex> indexes,
            MigrationCommandListBuilder builder)
        {
            foreach (var index in indexes)
            {
                Generate(CreateIndexOperation.CreateFrom(index), index.Table.Model.Model, builder, terminate: false);
                builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            }
        }

        #endregion Indexes

        #region Sequences

        protected override void Generate(
            CreateSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("CREATE SEQUENCE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!));

            var typeMapping = Dependencies.TypeMappingSource.GetMapping(operation.ClrType);

            if (operation.ClrType != typeof(long))
            {
                builder
                    .Append(" AS ")
                    .Append(typeMapping.StoreType);

                        typeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(long));
            }

            builder
                .Append(" START WITH ")
                .Append(typeMapping.GenerateSqlLiteral(operation.StartValue));

            SequenceOptions(operation, model, builder);

            builder.AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate(
            RenameSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
                throw new NotImplementedException();
        }

        protected override void Generate(
            AlterSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);
            builder
                .Append("ALTER SEQUENCE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!));

            SequenceOptions(operation, model, builder);

            builder.AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate(
            DropSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("DROP SEQUENCE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!))
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void Generate(
            RestartSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            SetCurrentUser(operation.Schema!, builder);

            builder
                .Append("ALTER SEQUENCE ")
                .Append(DelimitIdentifier(operation.Name, operation.Schema!))
                .Append(" RESTART WITH ")
                .Append(SqlLiteral(operation.StartValue))
                .AppendLine(StatementTerminator);

            EndStatement(builder);
        }

        protected override void SequenceOptions(
            AlterSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
            => SequenceOptions(
                operation.Schema,
                operation.Name,
                operation,
                model,
                builder);


        protected override void SequenceOptions(
            CreateSequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
            => SequenceOptions(
                operation.Schema,
                operation.Name,
                operation,
                model,
                builder);


        protected override void SequenceOptions(
            string? schema,
            string name,
            SequenceOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append(" INCREMENT BY ")
                .Append(SqlLiteral(operation.IncrementBy));

            if (operation.MinValue != null)
            {
                builder
                    .Append(" MINVALUE ")
                    .Append(SqlLiteral(operation.MinValue));
            }
            else
            {
                builder.Append(" NO MINVALUE");
            }

            if (operation.MaxValue != null)
            {
                builder
                    .Append(" MAXVALUE ")
                    .Append(SqlLiteral(operation.MaxValue));
            }
            else
            {
                builder.Append(" NO MAXVALUE");
            }

            builder.Append(operation.IsCyclic ? " CYCLE" : " NO CYCLE");
        }

        #endregion Sequences

        #region Data

        protected override void Generate(
            InsertDataOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var sqlBuilder = new StringBuilder();
            foreach (var modificationCommand in GenerateModificationCommands(operation, model))
            {
                SqlGenerator.AppendInsertOperation(
                    sqlBuilder,
                    modificationCommand,
                    0);
            }

            builder.Append(sqlBuilder.ToString());

            if (terminate)
            {
                EndStatement(builder);
            }
        }

        protected override void Generate(
            UpdateDataOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var sqlBuilder = new StringBuilder();
            foreach (var modificationCommand in GenerateModificationCommands(operation, model))
            {
                SqlGenerator.AppendUpdateOperation(
                    sqlBuilder,
                    modificationCommand,
                    0);
            }

            builder.Append(sqlBuilder.ToString());
            EndStatement(builder);
        }

        protected override void Generate(
            DeleteDataOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var sqlBuilder = new StringBuilder();
            foreach (var modificationCommand in GenerateModificationCommands(operation, model))
            {
                SqlGenerator.AppendDeleteOperation(
                    sqlBuilder,
                    modificationCommand,
                    0);
            }

            builder.Append(sqlBuilder.ToString());
            EndStatement(builder);
        }

        #endregion Data

        #region Comments

        private void CommentOnTable(
            [NotNull] MigrationCommandListBuilder builder,
            [CanBeNull] string schema,
            [NotNull] string table,
            [CanBeNull] string comment
        )
        {
            builder
                .Append("COMMENT ON TABLE ")
                .Append(DelimitIdentifier(table, schema))
                .Append(" IS ")
                .Append(SqlLiteral(comment ?? ""))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
        }

        private void CommentOnView(
            [NotNull] MigrationCommandListBuilder builder,
            [CanBeNull] string schema,
            [NotNull] string table,
            [CanBeNull] string comment
        )
        {
            builder
                .Append("COMMENT ON VIEW ")
                .Append(DelimitIdentifier(table, schema))
                .Append(" IS ")
                .Append(SqlLiteral(comment ?? ""))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
        }

        private void CommentOnColumn(
            [NotNull] MigrationCommandListBuilder builder,
            [CanBeNull] string schema,
            [NotNull] string table,
            [NotNull] string column,
            [CanBeNull] string comment
        )
        {
            builder
                .Append("COMMENT ON COLUMN ")
                .Append(DelimitIdentifier(table, schema))
                .Append(".")
                .Append(DelimitIdentifier(column))
                .Append(" IS ")
                .Append(SqlLiteral(comment ?? ""))
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
        }

        #endregion Comments

        #region Helpers

        protected override void EndStatement(
            [NotNull] MigrationCommandListBuilder builder,
            bool suppressTransaction = false
            )
        {
            base.EndStatement(builder, false);
        }

        private string SqlLiteral<T>(T value)
            => Dependencies.TypeMappingSource.GetMapping(typeof(T)).GenerateSqlLiteral(value);

        private string DelimitIdentifier([NotNull] string identifier)
            => Dependencies.SqlGenerationHelper.DelimitIdentifier(identifier);

        private string DelimitIdentifier([NotNull] string name, [CanBeNull] string schema)
            => Dependencies.SqlGenerationHelper.DelimitIdentifier(name, schema);

        private void SetCurrentUser(
            [CanBeNull] string? userName,
            [NotNull] MigrationCommandListBuilder builder
            )
        {
            Check.NotNull(builder, nameof(builder));

            if (userName == _currentUserName)
                return;

            builder.Append("set session authorization ");
            if (userName is null)
            {
                builder.Append("initial_user");
            }
            else
            {
                builder.Append(DelimitIdentifier(userName));
            }
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

            EndStatement(builder, true);

            _currentUserName = userName!;
        }

        private void ResetCurrentUser(
            [NotNull] MigrationCommandListBuilder builder
            )
        {
            SetCurrentUser(null, builder);
        }

        #endregion Helpers

        private IReadOnlyList<MigrationOperation> RewriteOperations(
            IReadOnlyList<MigrationOperation> migrationOperations,
            IModel? model,
            MigrationsSqlGenerationOptions options)
        {
            var operations = new List<MigrationOperation>();

            var versioningMap = new Dictionary<(string?, string?), (string, string?, bool)>();
            var periodMap = new Dictionary<(string?, string?), (string, string, bool)>();
            var availableSchemas = new List<string>();

            foreach (var operation in migrationOperations)
            {
                if (operation is EnsureSchemaOperation ensureSchemaOperation)
                {
                    availableSchemas.Add(ensureSchemaOperation.Name);
                }

                var isTemporal = operation[ActianAnnotationNames.IsTemporal] as bool? == true;
                if (isTemporal)
                {
                    string? table = null;
                    string? schema = null;

                    if (operation is ITableMigrationOperation tableMigrationOperation)
                    {
                        table = tableMigrationOperation.Table;
                        schema = tableMigrationOperation.Schema;
                    }

                    var suppressTransaction = table is not null && IsMemoryOptimized(operation, model, schema, table);

                    schema ??= model?.GetDefaultSchema();
                    var historyTableName = operation[ActianAnnotationNames.TemporalHistoryTableName] as string;
                    var historyTableSchema = operation[ActianAnnotationNames.TemporalHistoryTableSchema] as string
                        ?? schema;
                    var periodStartColumnName = operation[ActianAnnotationNames.TemporalPeriodStartColumnName] as string;
                    var periodEndColumnName = operation[ActianAnnotationNames.TemporalPeriodEndColumnName] as string;

                    switch (operation)
                    {
                        case CreateTableOperation createTableOperation:
                            if (historyTableSchema != createTableOperation.Schema
                                && historyTableSchema != null
                                && !availableSchemas.Contains(historyTableSchema))
                            {
                                operations.Add(new EnsureSchemaOperation { Name = historyTableSchema });
                                availableSchemas.Add(historyTableSchema);
                            }

                            operations.Add(operation);
                            break;

                        case DropTableOperation:
                            DisableVersioning(table!, schema, historyTableName!, historyTableSchema, suppressTransaction);
                            operations.Add(operation);

                            versioningMap.Remove((table, schema));
                            periodMap.Remove((table, schema));
                            break;

                        case RenameTableOperation renameTableOperation:
                            DisableVersioning(table!, schema, historyTableName!, historyTableSchema, suppressTransaction);
                            operations.Add(operation);

                            // since table was renamed, remove old entry and add new entry
                            // marked as versioning disabled, so we enable it in the end for the new table
                            versioningMap.Remove((table, schema));
                            versioningMap[(renameTableOperation.NewName, renameTableOperation.NewSchema)] =
                                (historyTableName!, historyTableSchema, suppressTransaction);

                            // same thing for disabled system period - remove one associated with old table and add one for the new table
                            if (periodMap.TryGetValue((table, schema), out var result))
                            {
                                periodMap.Remove((table, schema));
                                periodMap[(renameTableOperation.NewName, renameTableOperation.NewSchema)] = result;
                            }

                            break;

                        case AlterTableOperation alterTableOperation:
                            var oldIsTemporal = alterTableOperation.OldTable[ActianAnnotationNames.IsTemporal] as bool? == true;
                            if (!oldIsTemporal)
                            {
                                periodMap[(alterTableOperation.Name, alterTableOperation.Schema)] =
                                    (periodStartColumnName!, periodEndColumnName!, suppressTransaction);
                                versioningMap[(alterTableOperation.Name, alterTableOperation.Schema)] =
                                    (historyTableName!, historyTableSchema, suppressTransaction);
                            }
                            else
                            {
                                var oldHistoryTableName =
                                    alterTableOperation.OldTable[ActianAnnotationNames.TemporalHistoryTableName] as string;
                                var oldHistoryTableSchema =
                                    alterTableOperation.OldTable[ActianAnnotationNames.TemporalHistoryTableSchema] as string
                                    ?? alterTableOperation.OldTable.Schema
                                    ?? model?[RelationalAnnotationNames.DefaultSchema] as string;

                                if (oldHistoryTableName != historyTableName
                                    || oldHistoryTableSchema != historyTableSchema)
                                {
                                    if (historyTableSchema != null
                                        && !availableSchemas.Contains(historyTableSchema))
                                    {
                                        operations.Add(new EnsureSchemaOperation { Name = historyTableSchema });
                                        availableSchemas.Add(historyTableSchema);
                                    }

                                    operations.Add(
                                        new RenameTableOperation
                                        {
                                            Name = oldHistoryTableName!,
                                            Schema = oldHistoryTableSchema,
                                            NewName = historyTableName,
                                            NewSchema = historyTableSchema
                                        });

                                    if (versioningMap.ContainsKey((alterTableOperation.Name, alterTableOperation.Schema)))
                                    {
                                        versioningMap[(alterTableOperation.Name, alterTableOperation.Schema)] =
                                            (historyTableName!, historyTableSchema, suppressTransaction);
                                    }
                                }
                            }

                            operations.Add(operation);
                            break;

                        case AlterColumnOperation alterColumnOperation:
                            // if only difference is in temporal annotations being removed or history table changed etc - we can ignore this operation
                            if (!CanSkipAlterColumnOperation(alterColumnOperation.OldColumn, alterColumnOperation))
                            {
                                operations.Add(operation);

                                // when modifying a period column, we need to perform the operations as a normal column first, and only later enable period
                                // removing the period information now, so that when we generate SQL that modifies the column we won't be making them auto generated as period
                                // (making column auto generated is not allowed in ALTER COLUMN statement)
                                // in later operation we enable the period and the period columns get set to auto generated automatically
                                //
                                // if the column is not period we just remove temporal information - it's no longer needed and could affect the generated sql
                                // we will generate all the necessary operations involved with temporal tables here
                                alterColumnOperation.RemoveAnnotation(ActianAnnotationNames.IsTemporal);
                                alterColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodStartColumnName);
                                alterColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodEndColumnName);
                                alterColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableName);
                                alterColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableSchema);

                                // this is the case where we are not converting from normal table to temporal
                                // just a normal modification to a column on a temporal table
                                // in that case we need to double check if we need have disabled versioning earlier in this migration
                                // if so, we need to mirror the operation to the history table
                                if (alterColumnOperation.OldColumn[ActianAnnotationNames.IsTemporal] as bool? == true)
                                {
                                    alterColumnOperation.OldColumn.RemoveAnnotation(ActianAnnotationNames.IsTemporal);
                                    alterColumnOperation.OldColumn.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodStartColumnName);
                                    alterColumnOperation.OldColumn.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodEndColumnName);
                                    alterColumnOperation.OldColumn.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableName);
                                    alterColumnOperation.OldColumn.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableSchema);

                                    if (versioningMap.ContainsKey((table, schema)))
                                    {
                                        var alterHistoryTableColumn = CopyColumnOperation<AlterColumnOperation>(alterColumnOperation);
                                        alterHistoryTableColumn.Table = historyTableName!;
                                        alterHistoryTableColumn.Schema = historyTableSchema;
                                        alterHistoryTableColumn.OldColumn =
                                            CopyColumnOperation<AddColumnOperation>(alterColumnOperation.OldColumn);
                                        alterHistoryTableColumn.OldColumn.Table = historyTableName!;
                                        alterHistoryTableColumn.OldColumn.Schema = historyTableSchema;

                                        operations.Add(alterHistoryTableColumn);
                                    }

                                    // TODO: test what happens if default value just changes (from temporal to temporal)
                                }
                            }

                            break;

                        case DropPrimaryKeyOperation:
                        case AddPrimaryKeyOperation:
                            DisableVersioning(table!, schema, historyTableName!, historyTableSchema, suppressTransaction);
                            operations.Add(operation);
                            break;

                        case DropColumnOperation dropColumnOperation:
                            DisableVersioning(table!, schema, historyTableName!, historyTableSchema, suppressTransaction);
                            if (dropColumnOperation.Name == periodStartColumnName
                                || dropColumnOperation.Name == periodEndColumnName)
                            {
                                // period columns can be null here - it doesn't really matter since we are never enabling the period back
                                // if we remove the period columns, it means we will be dropping the table also or at least convert it back to
                                // regular which will clear the entry in the periodMap for this table
                                DisablePeriod(table!, schema, periodStartColumnName!, periodEndColumnName!, suppressTransaction);
                            }

                            operations.Add(operation);

                            break;

                        case AddColumnOperation addColumnOperation:
                            operations.Add(addColumnOperation);

                            // when adding a period column, we need to add it as a normal column first, and only later enable period
                            // removing the period information now, so that when we generate SQL that adds the column we won't be making them
                            // auto generated as period it won't work, unless period is enabled but we can't enable period without adding the
                            // columns first - chicken and egg
                            if (addColumnOperation[ActianAnnotationNames.IsTemporal] as bool? == true)
                            {
                                addColumnOperation.RemoveAnnotation(ActianAnnotationNames.IsTemporal);
                                addColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableName);
                                addColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalHistoryTableSchema);
                                addColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodStartColumnName);
                                addColumnOperation.RemoveAnnotation(ActianAnnotationNames.TemporalPeriodEndColumnName);

                                // model differ adds default value, but for period end we need to replace it with the correct one -
                                // DateTime.MaxValue
                                if (addColumnOperation.Name == periodEndColumnName)
                                {
                                    addColumnOperation.DefaultValue = DateTime.MaxValue;
                                }

                                // when adding (non-period) column to an exisiting temporal table we need to check if we have disabled the period
                                // due to some other operations in the same migration (e.g. delete column)
                                // if so, we need to also add the same column to history table
                                if (addColumnOperation.Name != periodStartColumnName
                                    && addColumnOperation.Name != periodEndColumnName)
                                {
                                    if (versioningMap.ContainsKey((table, schema)))
                                    {
                                        var addHistoryTableColumnOperation = CopyColumnOperation<AddColumnOperation>(addColumnOperation);
                                        addHistoryTableColumnOperation.Table = historyTableName!;
                                        addHistoryTableColumnOperation.Schema = historyTableSchema;

                                        operations.Add(addHistoryTableColumnOperation);
                                    }
                                }
                            }

                            break;

                        case RenameColumnOperation renameColumnOperation:
                            operations.Add(renameColumnOperation);

                            // if we disabled period for the temporal table and now we are renaming the column,
                            // we need to also rename this same column in history table
                            if (versioningMap.ContainsKey((table, schema)))
                            {
                                var renameHistoryTableColumnOperation = new RenameColumnOperation
                                {
                                    IsDestructiveChange = renameColumnOperation.IsDestructiveChange,
                                    Name = renameColumnOperation.Name,
                                    NewName = renameColumnOperation.NewName,
                                    Table = historyTableName!,
                                    Schema = historyTableSchema
                                };

                                operations.Add(renameHistoryTableColumnOperation);
                            }

                            break;

                        default:
                            operations.Add(operation);
                            break;
                    }
                }
                else
                {
                    if (operation is AlterTableOperation alterTableOperation
                        && alterTableOperation.OldTable[ActianAnnotationNames.IsTemporal] as bool? == true)
                    {
                        var historyTableName = alterTableOperation.OldTable[ActianAnnotationNames.TemporalHistoryTableName] as string;
                        var historyTableSchema = alterTableOperation.OldTable[ActianAnnotationNames.TemporalHistoryTableSchema] as string
                            ?? alterTableOperation.OldTable.Schema
                            ?? model?[RelationalAnnotationNames.DefaultSchema] as string;

                        var periodStartColumnName =
                            alterTableOperation.OldTable[ActianAnnotationNames.TemporalPeriodStartColumnName] as string;
                        var periodEndColumnName =
                            alterTableOperation.OldTable[ActianAnnotationNames.TemporalPeriodEndColumnName] as string;
                        var suppressTransaction = IsMemoryOptimized(operation, model, alterTableOperation.Schema, alterTableOperation.Name);

                        DisableVersioning(
                            alterTableOperation.Name, alterTableOperation.Schema, historyTableName!, historyTableSchema, suppressTransaction);
                        DisablePeriod(
                            alterTableOperation.Name, alterTableOperation.Schema, periodStartColumnName!, periodEndColumnName!,
                            suppressTransaction);

                        if (historyTableName != null)
                        {
                            operations.Add(
                                new DropTableOperation { Name = historyTableName, Schema = historyTableSchema });
                        }

                        operations.Add(operation);

                        // when we disable versioning and period earlier, we marked it to be re-enabled
                        // since table is no longer temporal we don't need to do that anymore
                        versioningMap.Remove((alterTableOperation.Name, alterTableOperation.Schema));
                        periodMap.Remove((alterTableOperation.Name, alterTableOperation.Schema));
                    }
                    else if (operation is AlterColumnOperation alterColumnOperation)
                    {
                        // if only difference is in temporal annotations being removed or history table changed etc - we can ignore this operation
                        if (alterColumnOperation.OldColumn?[ActianAnnotationNames.IsTemporal] as bool? != true
                            || !CanSkipAlterColumnOperation(alterColumnOperation.OldColumn, alterColumnOperation))
                        {
                            operations.Add(operation);
                        }
                    }
                    else
                    {
                        operations.Add(operation);
                    }
                }
            }

            foreach (var ((table, schema), (periodStartColumnName, periodEndColumnName, suppressTransaction)) in periodMap)
            {
                EnablePeriod(table!, schema, periodStartColumnName, periodEndColumnName, suppressTransaction);
            }

            foreach (var ((table, schema), (historyTableName, historyTableSchema, suppressTransaction)) in versioningMap)
            {
                EnableVersioning(table!, schema, historyTableName, historyTableSchema, suppressTransaction);
            }

            return operations;

            void DisableVersioning(string table, string? schema, string historyTableName, string? historyTableSchema, bool suppressTransaction)
            {
                if (!versioningMap.TryGetValue((table, schema), out _))
                {
                    versioningMap[(table, schema)] = (historyTableName, historyTableSchema, suppressTransaction);

                    operations.Add(
                        new SqlOperation
                        {
                            Sql = new StringBuilder()
                                .Append("ALTER TABLE ")
                                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema))
                                .AppendLine(" SET (SYSTEM_VERSIONING = OFF)")
                                .ToString(),
                            SuppressTransaction = suppressTransaction
                        });
                }
            }

            void EnableVersioning(string table, string? schema, string historyTableName, string? historyTableSchema, bool suppressTransaction)
            {
                var stringBuilder = new StringBuilder();

                if (historyTableSchema == null)
                {
                    // need to run command using EXEC to inject default schema
                    stringBuilder.AppendLine("DECLARE @historyTableSchema sysname = SCHEMA_NAME()");
                    stringBuilder.Append("EXEC(N'");
                }

                var historyTable = historyTableSchema != null
                    ? Dependencies.SqlGenerationHelper.DelimitIdentifier(historyTableName, historyTableSchema)
                    : Dependencies.SqlGenerationHelper.DelimitIdentifier(historyTableName);

                stringBuilder
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema));

                if (historyTableSchema != null)
                {
                    stringBuilder.AppendLine($" SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {historyTable}))");
                }
                else
                {
                    stringBuilder.AppendLine(
                        $" SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + '].{historyTable}))')");
                }

                operations.Add(
                    new SqlOperation { Sql = stringBuilder.ToString(), SuppressTransaction = suppressTransaction });
            }

            void DisablePeriod(string table, string? schema, string periodStartColumnName, string periodEndColumnName, bool suppressTransaction)
            {
                if (!periodMap.TryGetValue((table, schema), out _))
                {
                    periodMap[(table, schema)] = (periodStartColumnName, periodEndColumnName, suppressTransaction);

                    operations.Add(
                        new SqlOperation
                        {
                            Sql = new StringBuilder()
                                .Append("ALTER TABLE ")
                                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema))
                                .AppendLine(" DROP PERIOD FOR SYSTEM_TIME")
                                .ToString(),
                            SuppressTransaction = suppressTransaction
                        });
                }
            }

            void EnablePeriod(string table, string? schema, string periodStartColumnName, string periodEndColumnName, bool suppressTransaction)
            {
                var addPeriodSql = new StringBuilder()
                    .Append("ALTER TABLE ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema))
                    .Append(" ADD PERIOD FOR SYSTEM_TIME (")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(periodStartColumnName))
                    .Append(", ")
                    .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(periodEndColumnName))
                    .Append(')')
                    .ToString();

                if (options.HasFlag(MigrationsSqlGenerationOptions.Idempotent))
                {
                    addPeriodSql = new StringBuilder()
                        .Append("EXEC(N'")
                        .Append(addPeriodSql.Replace("'", "''"))
                        .Append("')")
                        .ToString();
                }

                operations.Add(
                    new SqlOperation { Sql = addPeriodSql, SuppressTransaction = suppressTransaction });

                operations.Add(
                    new SqlOperation
                    {
                        Sql = new StringBuilder()
                            .Append("ALTER TABLE ")
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema))
                            .Append(" ALTER COLUMN ")
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(periodStartColumnName))
                            .Append(" ADD HIDDEN")
                            .ToString(),
                        SuppressTransaction = suppressTransaction
                    });

                operations.Add(
                    new SqlOperation
                    {
                        Sql = new StringBuilder()
                            .Append("ALTER TABLE ")
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(table, schema))
                            .Append(" ALTER COLUMN ")
                            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(periodEndColumnName))
                            .Append(" ADD HIDDEN")
                            .ToString(),
                        SuppressTransaction = suppressTransaction
                    });
            }

            static bool CanSkipAlterColumnOperation(ColumnOperation first, ColumnOperation second)
                => ColumnPropertiesAreTheSame(first, second)
                    && ColumnOperationsOnlyDifferByTemporalTableAnnotation(first, second)
                    && ColumnOperationsOnlyDifferByTemporalTableAnnotation(second, first);

            // don't compare name, table or schema - they are not being set in the model differ (since they should always be the same)
            static bool ColumnPropertiesAreTheSame(ColumnOperation first, ColumnOperation second)
                => first.ClrType == second.ClrType
                    && first.Collation == second.Collation
                    && first.ColumnType == second.ColumnType
                    && first.Comment == second.Comment
                    && first.ComputedColumnSql == second.ComputedColumnSql
                    && Equals(first.DefaultValue, second.DefaultValue)
                    && first.DefaultValueSql == second.DefaultValueSql
                    && first.IsDestructiveChange == second.IsDestructiveChange
                    && first.IsFixedLength == second.IsFixedLength
                    && first.IsNullable == second.IsNullable
                    && first.IsReadOnly == second.IsReadOnly
                    && first.IsRowVersion == second.IsRowVersion
                    && first.IsStored == second.IsStored
                    && first.IsUnicode == second.IsUnicode
                    && first.MaxLength == second.MaxLength
                    && first.Precision == second.Precision
                    && first.Scale == second.Scale;

            static bool ColumnOperationsOnlyDifferByTemporalTableAnnotation(ColumnOperation first, ColumnOperation second)
            {
                var unmatched = first.GetAnnotations().ToList();
                foreach (var annotation in second.GetAnnotations())
                {
                    var index = unmatched.FindIndex(
                        a => a.Name == annotation.Name
                            && StructuralComparisons.StructuralEqualityComparer.Equals(a.Value, annotation.Value));
                    if (index == -1)
                    {
                        continue;
                    }

                    unmatched.RemoveAt(index);
                }

                return unmatched.All(
                    a => a.Name is ActianAnnotationNames.IsTemporal
                        or ActianAnnotationNames.TemporalHistoryTableName
                        or ActianAnnotationNames.TemporalHistoryTableSchema
                        or ActianAnnotationNames.TemporalPeriodStartPropertyName
                        or ActianAnnotationNames.TemporalPeriodEndPropertyName
                        or ActianAnnotationNames.TemporalPeriodStartColumnName
                        or ActianAnnotationNames.TemporalPeriodEndColumnName);
            }

            static TOperation CopyColumnOperation<TOperation>(ColumnOperation source)
                where TOperation : ColumnOperation, new()
            {
                var result = new TOperation
                {
                    ClrType = source.ClrType,
                    Collation = source.Collation,
                    ColumnType = source.ColumnType,
                    Comment = source.Comment,
                    ComputedColumnSql = source.ComputedColumnSql,
                    DefaultValue = source.DefaultValue,
                    DefaultValueSql = source.DefaultValueSql,
                    IsDestructiveChange = source.IsDestructiveChange,
                    IsFixedLength = source.IsFixedLength,
                    IsNullable = source.IsNullable,
                    IsRowVersion = source.IsRowVersion,
                    IsStored = source.IsStored,
                    IsUnicode = source.IsUnicode,
                    MaxLength = source.MaxLength,
                    Name = source.Name,
                    Precision = source.Precision,
                    Scale = source.Scale,
                    Table = source.Table,
                    Schema = source.Schema
                };

                foreach (var annotation in source.GetAnnotations())
                {
                    result.AddAnnotation(annotation.Name, annotation.Value);
                }

                return result;
            }
        }

        private static bool IsMemoryOptimized(Annotatable annotatable, IModel? model, string? schema, string tableName)
            => annotatable[ActianAnnotationNames.MemoryOptimized] as bool?
            ?? model?.GetRelationalModel().FindTable(tableName, schema)?[ActianAnnotationNames.MemoryOptimized] as bool? == true;
    }
}
