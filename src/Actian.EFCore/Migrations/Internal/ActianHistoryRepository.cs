using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Actian.EFCore.Scaffolding.Internal;
using Actian.EFCore.Utilities;
using Ingres.Client;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Migrations.Internal
{
    public class ActianHistoryRepository : HistoryRepository
    {
        public ActianHistoryRepository([NotNull] HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override string ExistsSql
        {
            get
            {
                if (!(Dependencies.Connection.DbConnection is IngresConnection ingresConnection))
                    throw new Exception("Connection must be an IngresConnection");

                var (_, dbDelimitedCase) = ingresConnection.GetDbCasing();

                return TableSchema is null
                    ? $@"
                        select distinct 1
                            from $ingres.iitables
                            where table_name = '{dbDelimitedCase.Normalize(TableName)}'
                    "
                    : $@"
                        select distinct 1
                            from $ingres.iitables
                            where table_owner = '{dbDelimitedCase.Normalize(TableSchema)}'
                            and table_name = '{dbDelimitedCase.Normalize(TableName)}'
                    ";
            }
        }

        protected override bool InterpretExistsResult([NotNull] object value)
        {
            return value is int intValue && intValue == 1
                || value is short shortValue && shortValue == 1;
        }

        public override LockReleaseBehavior LockReleaseBehavior => LockReleaseBehavior.Connection;

        public override string GetCreateIfNotExistsScript()
            => Exists() ? "" : GetCreateScript();

        public override string GetBeginIfExistsScript(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

            return new StringBuilder()
                .Append("IF EXISTS (SELECT 1 FROM ")
                .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
                .Append(" WHERE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
                .Append(" = ")
                .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
                .AppendLine(")")
                .Append("BEGIN")
                .ToString();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

            return new StringBuilder()
                .Append("IF NOT EXISTS (SELECT 1 FROM ")
                .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
                .Append(" WHERE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
                .Append(" = ")
                .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
                .AppendLine(")")
                .Append("BEGIN")
                .ToString();
        }

        public override string GetEndIfScript() => new StringBuilder()
            .Append("END")
            .AppendLine(SqlGenerationHelper.StatementTerminator)
            .ToString();

        public override IMigrationsDatabaseLock AcquireDatabaseLock()
        {
            Dependencies.MigrationsLogger.AcquiringMigrationLock();

            var dbLock = CreateMigrationDatabaseLock();
            int result;
            try
            {
                result = (int)CreateGetLockCommand().ExecuteScalar(CreateRelationalCommandParameters())!;
            }
            catch
            {
                try
                {
                    dbLock.Dispose();
                }
                catch
                {
                }

                throw;
            }

            return result < 0
                ? throw new TimeoutException()
                : dbLock;
        }

        public override async Task<IMigrationsDatabaseLock> AcquireDatabaseLockAsync(CancellationToken cancellationToken = default)
        {
            Dependencies.MigrationsLogger.AcquiringMigrationLock();

            var dbLock = CreateMigrationDatabaseLock();
            int result;
            try
            {
                result = (int)(await CreateGetLockCommand().ExecuteScalarAsync(CreateRelationalCommandParameters(), cancellationToken)
                    .ConfigureAwait(false))!;
            }
            catch
            {
                try
                {
                    await dbLock.DisposeAsync().ConfigureAwait(false);
                }
                catch
                {
                }

                throw;
            }

            return result < 0
                ? throw new TimeoutException()
                : dbLock;
        }

        private ActianMigrationDatabaseLock CreateMigrationDatabaseLock()
    => new(
        Dependencies.RawSqlCommandBuilder.Build(
            """
DECLARE @result int;
EXEC @result = sp_releaseapplock @Resource = '__EFMigrationsLock', @LockOwner = 'Session';
SELECT @result
"""),
        CreateRelationalCommandParameters(),
        this);

        private RelationalCommandParameterObject CreateRelationalCommandParameters()
            => new(
                Dependencies.Connection,
                null,
                null,
                Dependencies.CurrentContext.Context,
                Dependencies.CommandLogger, CommandSource.Migrations);

        private IRelationalCommand CreateGetLockCommand()
    => Dependencies.RawSqlCommandBuilder.Build(
        """
DECLARE @result int;
EXEC @result = sp_getapplock @Resource = '__EFMigrationsLock', @LockOwner = 'Session', @LockMode = 'Exclusive';
SELECT @result
""",
        []).RelationalCommand;
    }
}
