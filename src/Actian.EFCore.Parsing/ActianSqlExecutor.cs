// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Actian.EFCore.Parsing.Internal;

namespace Actian.EFCore.Parsing
{
    public delegate Task ExecuteStatementAsync(string statement, bool ignoreErrors);

    public class ActianSqlExecutor
    {
        public static async Task ExecuteFileAsync(string path, ExecuteStatementAsync executeStatementAsync)
        {
            await new ActianSqlExecutor(executeStatementAsync)
                .ExecuteFileAsync(path);
        }

        private readonly ExecuteStatementAsync _executeStatementAsync;
        private readonly List<ActianSqlStatement> _buffer = new List<ActianSqlStatement>();
        private bool _ignoreErrors = false;
        private string _currentDirectory = Environment.CurrentDirectory;

        public ActianSqlExecutor(ExecuteStatementAsync executeStatementAsync)
        {
            _executeStatementAsync = executeStatementAsync;
        }

        public ActianSqlExecutor WithCurrentDirectory(string currentDirectory)
        {
            _currentDirectory = currentDirectory;
            return this;
        }

        public async Task ExecuteFileAsync(string path, bool isIncluded = false)
        {
            var oldCurrentDirectory = _currentDirectory;
            try
            {
                _currentDirectory = Path.GetDirectoryName(path);
                using var reader = new StreamReader(path);
                await ExecuteAsync(ActianSqlParser.Parse(reader), isIncluded: isIncluded);
            }
            finally
            {
                _currentDirectory = oldCurrentDirectory;
            }
        }

        private async Task ExecuteAsync(IEnumerator<ActianSqlStatement> statements, bool isIncluded = false)
        {
            if (!isIncluded)
            {
                _buffer.Clear();
            }

            while (statements.MoveNext())
            {
                switch (statements.Current)
                {
                    case ActianSqlCommand.Continue _:
                        _ignoreErrors = true;
                        break;
                    case ActianSqlCommand.NoContinue _:
                        _ignoreErrors = false;
                        break;
                    case ActianSqlCommand.Include include:
                        await ExecuteFileAsync(Path.GetFullPath(Path.Combine(_currentDirectory, include.FileName)), isIncluded: true);
                        break;
                    case ActianSqlCommand.Go _:
                        await Go();
                        break;
                    case ActianSqlCommand _:
                        // Do nothing
                        break;
                    case ActianSqlStatement statement when statement.HasStatement:
                        _buffer.Add(statement);
                        break;
                }
            }

            if (!isIncluded)
            {
                await Go();
            }
        }

        private async Task Go()
        {
            try
            {
                if (_executeStatementAsync is null)
                    return;

                foreach (var statement in _buffer)
                {
                    await _executeStatementAsync(statement.CommandText, _ignoreErrors);
                }
            }
            finally
            {
                _buffer.Clear();
            }
        }
    }
}
