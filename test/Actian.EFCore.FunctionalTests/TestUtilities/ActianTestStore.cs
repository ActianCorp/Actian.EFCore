﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Actian.EFCore.Parsing.Internal;
using Actian.EFCore.Storage.Internal;
using Ingres.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

#pragma warning disable IDE0022 // Use block body for methods
// ReSharper disable SuggestBaseTypeForParameter
namespace Actian.EFCore.TestUtilities
{
    public class ActianTestStore : RelationalTestStore
    {
        private static readonly ActianSqlGenerationHelper SqlGenerationHelper = new ActianSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies());

        public const int CommandTimeout = 300;

        public static ActianTestStore GetIIDbDb()
            => new ActianTestStore("iidbdb", TestEnvironment.LoginUser);

        public static ActianTestStore GetNorthwindStore()
            => (ActianTestStore)ActianNorthwindTestStoreFactory.Instance
                .GetOrCreate(ActianNorthwindTestStoreFactory.Name).Initialize(null, (Func<DbContext>)null);

        public static ActianTestStore GetOrCreate(string name)
            => new ActianTestStore(name);

        public static ActianTestStore GetOrCreateInitialized(string name)
            => new ActianTestStore(name).InitializeActian(null, (Func<DbContext>)null, null);

        public static ActianTestStore GetOrCreate(string name, string scriptPath)
            => new ActianTestStore(name, scriptPath: scriptPath);

        public static ActianTestStore Create(string name)
            => new ActianTestStore(name, shared: false);

        public static ActianTestStore CreateInitialized(string name)
            => new ActianTestStore(name, shared: false)
                .InitializeActian(null, (Func<DbContext>)null, null);

        private readonly string _scriptPath;

        private ActianTestStore(
            string name,
            string scriptPath = null,
            bool shared = true)
            : base(name, shared)
        {
            SetOutput(new ActianTestStoreLogger(name));
            Logger.LogInformation($"Creating ActianTestStore {name}");
            Console.WriteLine($"Creating ActianTestStore {name}");
            if (scriptPath != null)
            {
                _scriptPath = Path.Combine(Path.GetDirectoryName(typeof(ActianTestStore).GetTypeInfo().Assembly.Location), scriptPath);
            }

            ConnectionString = TestEnvironment.GetConnectionString(Name);
            Connection = new IngresConnection(ConnectionString);

            Console.WriteLine($"Server version: {TestEnvironment.ActianServerVersion}");
            Logger.LogInformation($"Server version: {TestEnvironment.ActianServerVersion}");
        }

        public IngresConnection IngresConnection => (IngresConnection)Connection;
        //public ITestOutputHelper Output { get; private set; } = new ConsoleTestOutputHelper();
        private readonly ActianTestLogger _actianTestLogger = new ActianTestLogger();
        public ILogger Logger => _actianTestLogger;

        public void SetOutput(ITestOutputHelper output)
        {
            _actianTestLogger.SetOutput(output);
        }

        public ActianTestStore InitializeActian(IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (ActianTestStore)Initialize(serviceProvider, createContext, seed);

        public ActianTestStore InitializeActian(
            IServiceProvider serviceProvider, Func<ActianTestStore, DbContext> createContext, Action<DbContext> seed)
            => InitializeActian(serviceProvider, () => createContext(this), seed);

        public IngresConnection GetIIDbDbConnection()
        {
            return new IngresConnection(new IngresConnectionStringBuilder(Connection.ConnectionString) { Database = "iidbdb" }.ConnectionString);
        }

        private static readonly HashSet<(Type, string)> _scriptExecuted = new HashSet<(Type, string)>();
        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            if (loggerFactory is ActianTestSqlLoggerFactory actianTestSqlLoggerFactory)
            {
                actianTestSqlLoggerFactory.AddLogger(Logger);
            }
            Logger.LogInformation("Initializing");
            try
            {
                if (CreateDatabase(clean))
                {
                    if (_scriptPath != null)
                    {
                        _scriptExecuted.Add((GetType(), _scriptPath));
                        ExecuteScript(_scriptPath);
                    }
                    else
                    {
                        using var context = createContext();
                        context.Database.EnsureCreatedResiliently();
                        seed?.Invoke(context);
                    }
                }
                else if (_scriptPath != null && !_scriptExecuted.Contains((GetType(), _scriptPath)))
                {
                    _scriptExecuted.Add((GetType(), _scriptPath));
                    //ExecuteScript(_scriptPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => builder.UseActian(Connection, b => b.ApplyConfiguration());

        private bool CreateDatabase(Action<DbContext> clean)
        {
            if (!DatabaseExists())
                throw new NotSupportedException($"Can not create Actian database {Connection.Database}");

            if (_scriptPath != null)
                return false;

            using var context = new DbContext(
                AddProviderOptions(
                    new DbContextOptionsBuilder().EnableServiceProviderCaching(false)
                ).Options
            );
            clean?.Invoke(context);
            Clean(context);
            return true;
        }

        public void DeleteDatabase()
        {
            throw new NotSupportedException("Can not delete Actian database {Name}");
        }

        private bool DatabaseExists()
        {
            using var iiDbDbConnection = GetIIDbDbConnection();
            return ExecuteScalar<int>(iiDbDbConnection, @$"
                select count(*)
                  from $ingres.iidatabase_info
                 where database_name = '{Connection.Database}'
            ") == 1;
        }

        //private void Clean(string name)
        //{
        //    var options = new DbContextOptionsBuilder()
        //        .UseActian(CreateConnectionString(name), b => b.ApplyConfiguration())
        //        .UseInternalServiceProvider(
        //            new ServiceCollection()
        //                .AddEntityFrameworkActian()
        //                .BuildServiceProvider())
        //        .Options;

        //    using var context = new DbContext(options);
        //    context.Database.EnsureClean();
        //}

        public void Clean()
        {
            var options = new DbContextOptionsBuilder()
                .UseActian(ConnectionString, b => b.ApplyConfiguration())
                .UseInternalServiceProvider(
                    new ServiceCollection()
                        .AddEntityFrameworkActian()
                        .BuildServiceProvider())
                .Options;

            using var context = new DbContext(options);
            context.Database.EnsureClean();
        }

        public override void Clean(DbContext context)
            => context.Database.EnsureClean();

        public void CleanObjects()
        {
            Logger.LogInformation(new string('=', 80));
            Logger.LogInformation("Cleaning database objects");
            Logger.LogInformation(new string('-', 80));

            var statements = new List<string>();

            ForEach($@"
                select table_owner,
                       table_name
                  from $ingres.iitables
                 where table_type in ('T', 'V')
                   and system_use = 'U'
            ", reader =>
            {
                var schema = reader.GetFieldValue<string>(0)?.Trim();
                var name = reader.GetFieldValue<string>(1)?.Trim();

                statements.Add($@"SET SESSION AUTHORIZATION ""{schema}"";");
                statements.Add($@"DROP ""{name}"";");
            });


            ForEach($@"
                select seq_owner,
                       seq_name
                  from $ingres.iisequences
            ", reader =>
            {
                var schema = reader.GetFieldValue<string>(0)?.Trim();
                var name = reader.GetFieldValue<string>(1)?.Trim();

                statements.Add($@"SET SESSION AUTHORIZATION ""{schema}"";");
                statements.Add($@"DROP SEQUENCE ""{name}"";");
            });

            statements.Add($"SET SESSION AUTHORIZATION initial_user;");

            ExecuteStatementsIgnoreErrors(statements);

            Logger.LogInformation(new string('=', 80));
            Logger.LogInformation("");
            Logger.LogInformation("");
        }

        public void ExecuteScript(string scriptPath)
        {
            Logger.LogInformation($"Executing script {Path.GetFileName(scriptPath)}");

            var filename = Path.GetFileNameWithoutExtension(scriptPath);
            var extension = Path.GetExtension(scriptPath);
            var logfile = Path.Combine(TestEnvironment.LogDirectory, Name, $"{filename}.log{extension}");

            using var log = new StreamWriter(logfile, false, Encoding.UTF8) { NewLine = "\n" };
            using var statements = ActianSqlParser.Parse(File.ReadAllText(scriptPath));

            ExecuteStatements(statements, log);
        }

        public override void OpenConnection()
            => new TestActianRetryingExecutionStrategy(Name).Execute(Connection, connection => connection.Open());

        public override Task OpenConnectionAsync()
            => new TestActianRetryingExecutionStrategy(Name).ExecuteAsync(Connection, connection => connection.OpenAsync());

        public T ExecuteScalar<T>(string sql, params object[] parameters)
            => ExecuteScalar<T>(Connection, sql, parameters);

        private T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => command.ExecuteScalar<T>(), sql, false, parameters);

        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] parameters)
            => ExecuteScalarAsync<T>(Connection, sql, parameters);

        private Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, async command => await command.ExecuteScalarAsync<T>(), sql, false, parameters);

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(Connection, sql, parameters);

        public int ExecuteNonQuery(IEnumerable<string> statements, bool ignoreErrors = false)
        {
            Connection.Open();
            try
            {
                var rows = 0;
                if (statements != null)
                {
                    foreach (var statement in statements.Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        try
                        {
                            rows += ExecuteNonQuery(Connection, statement);
                        }
                        catch when (ignoreErrors)
                        {
                            // Ignore
                        }
                    }
                }
                return rows;
            }
            finally
            {
                Connection.Close();
            }
        }

        public string GetServerVersion()
        {
            Connection.Open();
            try
            {
                return Connection.ServerVersion;
            }
            finally
            {
                Connection.Close();
            }
        }

        public int ExecuteStatements(params string[] statements)
        {
            return ExecuteStatements(statements.AsEnumerable());
        }

        public int ExecuteStatements(IEnumerable<string> statements)
        {
            using var parsedStatements = ActianSqlParser.Parse(statements);
            return ExecuteStatements(parsedStatements);
        }

        public int ExecuteStatementsIgnoreErrors(params string[] statements)
        {
            return ExecuteStatementsIgnoreErrors(statements.AsEnumerable());
        }

        public int ExecuteStatementsIgnoreErrors(IEnumerable<string> statements)
        {
            using var parsedStatements = ActianSqlParser.Parse(statements);
            return ExecuteStatements(parsedStatements, ignoreErrors: true);
        }

        public int ExecuteStatements(IEnumerator<ActianSqlStatement> statements, TextWriter log = null, bool ignoreErrors = false)
        {
            try
            {
                var buffer = new List<ActianSqlStatement>();

                var rows = Execute(Connection, command =>
                {
                    while (statements.MoveNext())
                    {
                        if (statements.Current is ActianSqlCommand.Continue)
                        {
                            ignoreErrors = true;
                        }
                        else if (statements.Current is ActianSqlCommand.NoContinue)
                        {
                            ignoreErrors = false;
                        }
                        else if (statements.Current is ActianSqlCommand.Go)
                        {
                            try
                            {
                                foreach (var statement in buffer)
                                {
                                    try
                                    {
                                        log?.WriteLine(statement.ToString());
                                        Logger.LogInformation(statement.ToString());
                                        command.CommandText = statement.CommandText;
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex) when (ignoreErrors)
                                    {
                                        log?.WriteLine(ex.Message);
                                        Logger.LogWarning(ex, ex.Message);
                                        // Ignore error
                                    }
                                    catch (Exception ex) when (!ignoreErrors)
                                    {
                                        log?.WriteLine(ex.Message);
                                        Logger.LogError(ex, ex.Message);
                                        throw;
                                    }
                                }
                            }
                            finally
                            {
                                buffer = new List<ActianSqlStatement>();
                            }
                        }
                        else if (statements.Current is ActianSqlCommand)
                        {
                            // Do nothing
                        }
                        else if (statements.Current.HasStatement)
                        {
                            buffer.Add(statements.Current);
                        }
                    }
                    foreach (var statement in buffer)
                    {
                        try
                        {
                            log?.WriteLine(statement.ToString());
                            Logger.LogInformation(statement.ToString());
                            command.CommandText = statement.CommandText;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex) when (ignoreErrors)
                        {
                            log?.WriteLine(ex.Message);
                            Logger.LogWarning(ex, ex.Message);
                            // Ignore error
                        }
                        catch (Exception ex) when (!ignoreErrors)
                        {
                            log?.WriteLine(ex.Message);
                            Logger.LogError(ex, ex.Message);
                            throw;
                        }
                    }
                    return 0;
                }, "");

                log?.WriteLine("-- SCRIPT SUCCEEDED");
                log?.WriteLine();
                return rows;
            }
            catch
            {
                log?.WriteLine("-- SCRIPT FAILED");
                log?.WriteLine();
                throw;
            }
        }

        private int ExecuteNonQuery(DbConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => Log(sql, () => command.ExecuteNonQuery()), sql, false, parameters);

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
            => ExecuteNonQueryAsync(Connection, sql, parameters);

        private Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, command => LogAsync(sql, () => command.ExecuteNonQueryAsync()), sql, false, parameters);

        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
            => Query<T>(Connection, sql, parameters);

        public IEnumerable<T> Query<T>(string sql, Func<DbDataReader, T> read, params object[] parameters)
            => Query(Connection, sql, read, parameters);

        private IEnumerable<T> Query<T>(DbConnection connection, string sql, Func<DbDataReader, T> read, object[] parameters = null)
            => Execute(
                connection, command =>
                {
                    using var dataReader = Log(sql, () => command.ExecuteReader());
                    var results = new List<T>();
                    while (dataReader.Read())
                    {
                        results.Add(read(dataReader));
                    }
                    return results;
                }, sql, false, parameters);

        private IEnumerable<T> Query<T>(DbConnection connection, string sql, object[] parameters = null)
            => Execute(
                connection, command =>
                {
                    using var dataReader = Log(sql, () => command.ExecuteReader());
                    var results = Enumerable.Empty<T>();
                    while (dataReader.Read())
                    {
                        results = results.Concat(new[] { dataReader.GetFieldValue<T>(0) });
                    }
                    return results;
                }, sql, false, parameters);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
            => QueryAsync<T>(Connection, sql, parameters);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, Func<DbDataReader, T> read, params object[] parameters)
            => QueryAsync<T>(Connection, sql, read, parameters);

        private Task<IEnumerable<T>> QueryAsync<T>(DbConnection connection, string sql, Func<DbDataReader, T> read, object[] parameters = null)
            => ExecuteAsync(
                connection, async command =>
                {
                    using var dataReader = await LogAsync(sql, () => command.ExecuteReaderAsync());

                    var results = new List<T>();
                    while (await dataReader.ReadAsync())
                    {
                        results.Add(read(dataReader));
                    }
                    return results.AsEnumerable();
                }, sql, false, parameters);

        private Task<IEnumerable<T>> QueryAsync<T>(DbConnection connection, string sql, object[] parameters = null)
            => ExecuteAsync(
                connection, async command =>
                {
                    using var dataReader = await LogAsync(sql, () => command.ExecuteReaderAsync());

                    var results = Enumerable.Empty<T>();
                    while (await dataReader.ReadAsync())
                    {
                        results = results.Concat(new[] { await dataReader.GetFieldValueAsync<T>(0) });
                    }

                    return results;
                }, sql, false, parameters);

        public int ForEach(string sql, Action<DbDataReader> action, params object[] parameters)
            => ForEach(Connection, sql, action, parameters);

        private int ForEach(DbConnection connection, string sql, Action<DbDataReader> action, object[] parameters = null)
            => Execute(
                connection, command =>
                {
                    using var dataReader = Log(sql, () => command.ExecuteReader());
                    var rows = 0;
                    while (dataReader.Read())
                    {
                        action(dataReader);
                        rows += 1;
                    }
                    return rows;
                }, sql, false, parameters);

        private T Execute<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => new TestActianRetryingExecutionStrategy(Name).Execute(
                new
                {
                    connection,
                    execute,
                    sql,
                    useTransaction,
                    parameters
                },
                state => ExecuteCommand(state.connection, state.execute, state.sql, state.useTransaction, state.parameters));

        private T ExecuteCommand<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql, bool useTransaction, object[] parameters)
        {
            var wasOpen = connection.State == ConnectionState.Open;
            if (!wasOpen)
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                connection.Open();
            }
            try
            {
                using var transaction = useTransaction ? connection.BeginTransaction() : null;
                using var command = CreateCommand(connection, sql, parameters);
                command.Transaction = transaction;
                var result = execute(command);
                transaction?.Commit();
                return result;
            }
            finally
            {
                if (!wasOpen && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private Task<T> ExecuteAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql,
            bool useTransaction = false, IReadOnlyList<object> parameters = null)
            => new TestActianRetryingExecutionStrategy(Name).ExecuteAsync(
                new
                {
                    connection,
                    executeAsync,
                    sql,
                    useTransaction,
                    parameters
                },
                state => ExecuteCommandAsync(state.connection, state.executeAsync, state.sql, state.useTransaction, state.parameters));

        private async Task<T> ExecuteCommandAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql, bool useTransaction,
            IReadOnlyList<object> parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }

            await connection.OpenAsync();
            try
            {
                using (var transaction = useTransaction ? await connection.BeginTransactionAsync() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        result = await LogAsync(sql, () => executeAsync(command));
                    }

                    if (transaction != null)
                    {
                        await transaction.CommitAsync();
                    }

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private T Log<T>(string sql, Func<T> action)
        {
            try
            {
                Logger.LogInformation(sql);
                Logger.LogInformation("");
                return action();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "");
                throw;
            }
            finally
            {
                Logger.LogInformation("");
            }
        }

        private async Task<T> LogAsync<T>(string sql, Func<Task<T>> action)
        {
            try
            {
                Logger.LogInformation(sql);
                Logger.LogInformation("");
                return await action();
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"ERROR: {ex.Message}");
                throw;
            }
            finally
            {
                Logger.LogInformation("");
            }
        }

        private static DbCommand CreateCommand(
            DbConnection connection,
            string commandText,
            IReadOnlyList<object> parameters = null)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    command.Parameters.Add(new IngresParameter { ParameterName = "@p" + i, Value = parameters[i] });
                }
            }

            return command;
        }

        public override void Dispose()
        {
            Connection?.Dispose();
            Connection = null;
            base.Dispose();
        }
    }
}
