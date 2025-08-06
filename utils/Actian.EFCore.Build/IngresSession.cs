// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ingres.Client;

namespace Actian.EFCore.Build
{
    public class IngresSession : IDisposable
    {
        private readonly LogConsole _console;
        private readonly IngresConnectionStringBuilder _connectionStringBuilder;
        private IngresTransaction _transaction;

        public IngresSession(string connectionString, LogConsole console)
        {
            _connectionStringBuilder = new IngresConnectionStringBuilder(connectionString);
            _console = console;
            _console.WriteLine($"Execute SQL in database {Database}");
            _console.WriteLine();
        }

        public string Database => _connectionStringBuilder.Database;
        public string ConnectionString => _connectionStringBuilder.ConnectionString;

        public IngresSession WithDbmsUser(string user)
        {
            if (_connection != null)
                throw new Exception("Already connected");

            _connectionStringBuilder.DbmsUser = user;

            return this;
        }

        private IngresConnection _connection;
        public IngresConnection Connection
        {
            get
            {
                if (_connection is null)
                {
                    _connection = new IngresConnection(ConnectionString);
                    _connection.Open();
                }
                return _connection;
            }
        }

        private void EnsureTransaction()
        {
            _transaction ??= Connection.BeginTransaction();
        }

        public int Commit(bool beginNewTransaction = true)
        {
            if (_transaction is null)
                return -1;

            _console.WriteLine();
            _console.WriteLine("commit");
            _transaction?.Commit();
            _transaction = null;
            _console.WriteLine("ok");
            return -1;
        }

        public int Rollback(bool beginNewTransaction = true)
        {
            if (_transaction is null)
                return -1;

            _console.WriteLine();
            _console.WriteLine("rollback");
            _transaction?.Rollback();
            _transaction = null;
            _console.WriteLine("ok");
            return -1;
        }

        public async Task<int> ExecuteAsync(string sql, bool ignoreErrors = false)
        {
            sql = sql.NormalizeText();

            using (_console.Indent())
            {
                var connected = false;
                try
                {
                    connected = true;
                    return sql.Trim().ToLowerInvariant() switch
                    {
                        "commit" => Commit(),
                        "rollback" => Rollback(),
                        _ => await ExecuteAsyncInternal(sql),
                    };
                }
                catch (Exception ex) when (!connected)
                {
                    _console.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex) when (ignoreErrors)
                {
                    _console.WriteLine(ex.Message);
                    return 0;
                }
                catch (Exception ex)
                {
                    _console.WriteLine(ex.Message, stderr: true);
                    throw;
                }
            }
        }

        private async Task<int> ExecuteAsyncInternal(string sql)
        {
            EnsureTransaction();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            _console.WriteLine();
            _console.WriteLine(cmd.CommandText);
            var rows = await cmd.ExecuteNonQueryAsync();
            _console.WriteLine(rows == -1 ? "ok" : $"{rows} rows");
            return rows;
        }

        public async Task<List<T>> SelectAsync<T>(string sql, Func<IngresDataReader, T> getRow, bool ignoreErrors = false)
        {
            sql = sql.NormalizeText();

            using (_console.Indent())
            {
                try
                {
                    EnsureTransaction();
                    using var cmd = Connection.CreateCommand();
                    cmd.CommandText = sql;
                    var rows = new List<T>();
                    _console.WriteLine(cmd.CommandText);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        rows.Add(getRow(reader));
                    }
                    _console.WriteLine($"{rows.Count} rows");

                    return rows;
                }
                catch (Exception ex) when (ignoreErrors)
                {
                    _console.WriteLine(ex.Message);
                    return new List<T>();
                }
                catch (Exception ex)
                {
                    _console.WriteLine(ex.Message, stderr: true);
                    throw;
                }
            }
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    using (_console.Indent())
                    {
                        Commit(false);
                        _connection?.Dispose();
                    }
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
