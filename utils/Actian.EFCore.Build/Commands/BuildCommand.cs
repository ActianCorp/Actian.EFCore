// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace Actian.EFCore.Build.Commands
{
    public abstract class BuildCommand : Command
    {
        protected BuildCommand(BuildContext context, string name, string description = null)
            : base(name, description)
        {
            Context = context;
            this.SetHandler(async () => await Run());
        }

        public int? ExitCode { get; private set; } = null;
        public BuildContext Context { get; }

        public async Task<int> Run()
        {
            if (ExitCode.HasValue)
                return ExitCode.Value;

            try
            {
                await RunInternal();
                ExitCode = 0;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    Console.Error.WriteLine($"ERROR: {ex.Message}");
                }
                ExitCode = 1;
            }
            return ExitCode.Value;
        }

        public abstract Task RunInternal();
    }
}
