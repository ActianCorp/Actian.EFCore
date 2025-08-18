// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;

namespace Actian.EFCore.Build
{
    public class CliSession : IDisposable
    {
        private readonly LogConsole _console;

        public CliSession(LogConsole console)
        {
            _console = console;
        }

        public async Task ExecuteAsync(string command, string args, Stream input = null, bool ignoreErrors = false)
        {
            var cmd = GetCommand(command, args, ignoreErrors);

            if (input != null)
            {
                cmd = cmd.WithStandardInputPipe(PipeSource.FromStream(input));
            }

            _console.WriteLine(cmd.ToString());
            _console.WriteLine();

            try
            {
                var result = await ExecuteInternalAsync(cmd, ignoreErrors);
                if (result.ExitCode != 0 && !ignoreErrors)
                    throw new Exception("");
            }
            catch
            {
                if (!ignoreErrors)
                    throw new Exception("");
            }
            finally
            {
                _console.WriteLine();
            }
        }

        private async Task<CommandResult> ExecuteInternalAsync(Command cmd, bool ignoreErrors)
        {
            using (_console.Indent())
            {
                try
                {
                    return await cmd.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"ERROR: {ex.Message}", stderr: !ignoreErrors);
                    throw;
                }
            }
        }

        private Command GetCommand(string command, string args, bool ignoreErrors)
        {
            return Cli.Wrap(command)
                .WithArguments(args)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => _console.WriteLine(line)))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(line => _console.WriteLine(line, stderr: !ignoreErrors)))
                .WithValidation(CommandResultValidation.None);
        }

        public void Dispose()
        {
        }
    }
}
