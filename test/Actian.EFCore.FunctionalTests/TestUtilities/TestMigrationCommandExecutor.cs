using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Actian.EFCore.Utilities;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit.Abstractions;

namespace Actian.EFCore.TestUtilities
{
    internal class TestMigrationCommandExecutor : MigrationCommandExecutor
    {
        public TestMigrationCommandExecutor(IExecutionStrategy executionStrategy) : base(executionStrategy)
        {
        }

        public ITestOutputHelper Output { get; private set; }

        public override void ExecuteNonQuery(
            IEnumerable<MigrationCommand> migrationCommands,
            IRelationalConnection connection)
        {
            Check.NotNull(migrationCommands, nameof(migrationCommands));
            Check.NotNull(connection, nameof(connection));

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                connection.Open();

                try
                {
                    IDbContextTransaction transaction = null;

                    try
                    {
                        foreach (var command in migrationCommands)
                        {
                            if (transaction == null
                                && !command.TransactionSuppressed)
                            {
                                transaction = connection.BeginTransaction();
                            }

                            if (transaction != null
                                && command.TransactionSuppressed)
                            {
                                transaction.Commit();
                                transaction.Dispose();
                                transaction = null;
                            }

                            Output.WriteLine(command.CommandText);
                            command.ExecuteNonQuery(connection);
                        }

                        transaction?.Commit();
                    }
                    finally
                    {
                        transaction?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine("Error:");
                    Output.WriteLine(ex.Message);
                    Output.WriteLine(ex.StackTrace);
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public override async Task ExecuteNonQueryAsync(
            IEnumerable<MigrationCommand> migrationCommands,
            IRelationalConnection connection,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(migrationCommands, nameof(migrationCommands));
            Check.NotNull(connection, nameof(connection));

            var transactionScope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await connection.OpenAsync(cancellationToken);

                try
                {
                    IDbContextTransaction transaction = null;

                    try
                    {
                        foreach (var command in migrationCommands)
                        {
                            if (transaction == null
                                && !command.TransactionSuppressed)
                            {
                                transaction = await connection.BeginTransactionAsync(cancellationToken);
                            }

                            if (transaction != null
                                && command.TransactionSuppressed)
                            {
                                transaction.Commit();
                                await transaction.DisposeAsync();
                                transaction = null;
                            }

                            Output.WriteLine(command.CommandText);
                            await command.ExecuteNonQueryAsync(connection, cancellationToken: cancellationToken);
                        }

                        transaction?.Commit();
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            await transaction.DisposeAsync();
                        }
                    }
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Output.WriteLine("Error:");
                Output.WriteLine(ex.Message);
                Output.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                await transactionScope.DisposeAsyncIfAvailable();
            }
        }
    }
}
