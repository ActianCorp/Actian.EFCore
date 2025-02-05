using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Actian.EFCore.Extensions;
using System.Transactions;
using System.ComponentModel;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IActianConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder) : RelationalDatabaseCreator(dependencies)
    {
        public readonly IActianConnection _connection = connection;
        private readonly IActianConnection IIDbDbConnection = connection.CreateIIDbDbConnection();
        private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder = rawSqlCommandBuilder;

        public virtual TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);
        public virtual TimeSpan RetryTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public override void Create()
        {
            throw new NotSupportedException();
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(100);
            throw new NotSupportedException();
        }

        public override bool HasTables()
        {
            return Dependencies.ExecutionStrategy.Execute<IActianConnection, bool>(_connection, ExecuteCommand, null);
        }

        private bool ExecuteCommand(IActianConnection connection)
        {
            return (Int16)CreateHasTablesCommand()
                .ExecuteScalar(new RelationalCommandParameterObject(
                    (IActianConnection)connection,
                    null,
                    null,
                    Dependencies.CurrentContext.Context,
                    Dependencies.CommandLogger,
                    CommandSource.Migrations))! != 0;
        }

        public override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
            => (int)(await Dependencies.ExecutionStrategy.ExecuteAsync(
                    _connection,
                    (connection, ct) => CreateHasTablesCommand()
                        .ExecuteScalarAsync<int>(
                            new RelationalCommandParameterObject(
                                connection,
                                null,
                                null,
                                Dependencies.CurrentContext.Context,
                                Dependencies.CommandLogger, CommandSource.Migrations),
                            cancellationToken: ct),
                    null,
                    cancellationToken).ConfigureAwait(false))!
                != 0;

        private IRelationalCommand CreateHasTablesCommand()
            => _rawSqlCommandBuilder
                .Build(
                    @"
                        select distinct 1
                          from $ingres.iitables
                         where table_type = 'T'
                           and system_use = 'U';
                    ");

        private IReadOnlyList<MigrationCommand> CreateCreateOperations()
        {
            throw new NotSupportedException();
        }

        public override bool Exists()
            => Exists(retryOnNotExists: false);

        private bool Exists(bool retryOnNotExists)
            => Dependencies.ExecutionStrategy.Execute(
                DateTime.UtcNow + RetryTimeout, giveUp =>
                {
                    while (true)
                    {
                        var opened = false;
                        try
                        {
                            using var _ = new TransactionScope(TransactionScopeOption.Suppress);
                            _connection.Open(errorsExpected: true);
                            opened = true;

                            _rawSqlCommandBuilder
                                .Build(@$"
                                    select 1
                                      from $ingres.iidatabase_info
                                     where database_name = 'iidbdb'
                                ")
                                .ExecuteNonQuery(
                                    new RelationalCommandParameterObject(
                                        IIDbDbConnection,
                                        null,
                                        null,
                                        Dependencies.CurrentContext.Context,
                                        Dependencies.CommandLogger, CommandSource.Migrations));

                            return true;
                        }
                        catch (Exception e)
                        {
                            if (DateTime.UtcNow > giveUp)
                            {
                                throw e.GetBaseException();
                            }

                            Thread.Sleep(RetryDelay);
                        }
                        finally
                        {
                            if (opened)
                            {
                                _connection.Close();
                            }
                        }
                    }
                },
                null);

        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
            => ExistsAsync(retryOnNotExists: false, cancellationToken: cancellationToken);

        private Task<bool> ExistsAsync(bool retryOnNotExists, CancellationToken cancellationToken)
            => Dependencies.ExecutionStrategy.ExecuteAsync(
                DateTime.UtcNow + RetryTimeout, async (giveUp, ct) =>
                {
                    while (true)
                    {
                        var opened = false;

                        try
                        {
                            using var _ = new TransactionScope(
                                TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
                            await _connection.OpenAsync(ct, errorsExpected: true).ConfigureAwait(false);
                            opened = true;

                            await _rawSqlCommandBuilder
                                .Build(@$"select 1
                                          from $ingres.iidatabase_info
                                          where database_name = 'iidbdb'")
                                .ExecuteNonQueryAsync(
                                    new RelationalCommandParameterObject(
                                        IIDbDbConnection,
                                        null,
                                        null,
                                        Dependencies.CurrentContext.Context,
                                        Dependencies.CommandLogger, CommandSource.Migrations),
                                    ct)
                                .ConfigureAwait(false);

                            return true;
                        }
                        catch (Exception e)
                        {
                            if (DateTime.UtcNow > giveUp)
                            {
                                throw e.GetBaseException();
                            }

                            await Task.Delay(RetryDelay, ct).ConfigureAwait(false);
                        }
                        finally
                        {
                            if (opened)
                            {
                                await _connection.CloseAsync().ConfigureAwait(false);
                            }
                        }
                    }
                }, null, cancellationToken);

        private bool RetryOnExistsFailure(Exception exception)
        {
            throw new NotSupportedException();
        }

        public override void Delete()
        {
            throw new NotSupportedException();
        }

        public override async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(100);
            throw new NotSupportedException();
        }

        private void ClearPool()
            => _connection.Close();
    }
}
