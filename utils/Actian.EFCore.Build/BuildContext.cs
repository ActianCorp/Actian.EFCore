// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Actian.EFCore.Build.Commands;
using Actian.EFCore.TestUtilities;

namespace Actian.EFCore.Build
{
    public class BuildContext
    {
        private readonly List<BuildCommand> _commands = new List<BuildCommand>();

        public BuildContext()
        {
            RootCommand.SetHandler(HandleRootCommand);
            AddCommand(new CreateDatabaseUsers(this));
            AddCommand(new CreateMissingTestDatabases(this));
            AddCommand(new CreateNorthwind(this));
            AddCommand(new CreateTestDatabases(this));
            AddCommand(new DropTestDatabases(this));
            AddCommand(new PopulateNorthwind(this));
            AddCommand(new SetupTestDatabases(this));
        }

        public RootCommand RootCommand { get; } = new RootCommand();
        public IEnumerable<BuildCommand> Commands => _commands;

        public async Task<int> Run(string[] args)
        {
            using var console = new LogConsole("", buffer: false);
            var exitCode = await RootCommand.InvokeAsync(args);
            console.WriteLine($"Exit code: {exitCode}");
            return exitCode;
        }

        public async Task RunCommand<TCommand>() where TCommand : BuildCommand
        {
            if (!TryGetCommand<TCommand>(out var command))
                throw new Exception("");

            var result = await command.Run();
            if (result != 0)
                throw new Exception("");
        }

        public bool TryGetCommand<TCommand>(out TCommand command)
            where TCommand : BuildCommand
        {
            command = null;
            foreach (var cmd in Commands)
            {
                if (cmd is TCommand result)
                {
                    command = result;
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<TestDatabase> TestDatabases { get; } = TestUtilities.TestDatabases
            .Load(Config.TestDatabasesPath).Databases
            .Where(db => (db.Create || db.Drop) && db.Name != "iidbdb")
            .ToList();

        private List<(string name, string owner)> _existingDatabases = null;
        public List<(string name, string owner)> ExistingDatabases => _existingDatabases ?? throw new Exception("Existing databases have not been retrieved");
        public async Task<List<(string name, string owner)>> GetExistingDatabases(LogConsole console, bool force = false)
        {
            if (_existingDatabases != null && !force)
                return _existingDatabases;

            console.WriteCaption("Get existing databases");
            using (console.Indent())
            {
                using var session = new IngresSession(Config.GetConnectionString("iidbdb"), console);

                _existingDatabases = await session.SelectAsync<(string name, string owner)>($@"
                    select name,own
                      from iidatabase
                ", reader => (reader.GetString(0), reader.GetString(1)));

                foreach (var (name, owner) in _existingDatabases)
                {
                    console.WriteLine($"Database: {name} ({owner})");
                }

                return _existingDatabases;
            }
        }

        public Task<bool> DatabaseExists(TestDatabase db, LogConsole console)
            => DatabaseExists(db.Name, console);

        public async Task<bool> DatabaseExists(string name, LogConsole console)
        {
            var existingDatabases = await GetExistingDatabases(console);
            return existingDatabases
                .Any(edb => edb.name.ToLowerInvariant() == name.ToLowerInvariant());
        }

        public Task<(string name, string owner)> GetExistingDatabase(TestDatabase db, LogConsole console)
            => GetExistingDatabase(db.Name, console);

        public async Task<(string name, string owner)> GetExistingDatabase(string name, LogConsole console)
        {
            var existingDatabases = await GetExistingDatabases(console);
            return existingDatabases
                .Where(edb => edb.name.ToLowerInvariant() == name.ToLowerInvariant())
                .DefaultIfEmpty((null, null))
                .First();
        }

        public Task AddExistingDatabase(TestDatabase db, LogConsole console)
            => AddExistingDatabase(db.Name, db.DbmsUser, console);

        public async Task AddExistingDatabase(string name, string owner, LogConsole console)
        {
            var existingDatabases = await GetExistingDatabases(console);
            _existingDatabases = existingDatabases
                .Where(edb => edb.name.ToLowerInvariant() != name.ToLowerInvariant())
                .Concat(new[] { (name, owner) })
                .ToList();
        }

        public Task RemoveExistingDatabase(TestDatabase db, LogConsole console)
            => RemoveExistingDatabase(db.Name, console);

        public async Task RemoveExistingDatabase(string name, LogConsole console)
        {
            var existingDatabases = await GetExistingDatabases(console);
            _existingDatabases = existingDatabases
                .Where(edb => edb.name.ToLowerInvariant() != name.ToLowerInvariant())
                .ToList();
        }

        public async Task EnsureTestDatabases(LogConsole console)
        {
            await GetExistingDatabases(console);
            foreach (var db in TestDatabases.Where(d => d.Create && !d.IsTemplateDatabase))
            {
                await EnsureTestDatabase(db, console);
            }
        }

        public async Task EnsureTestDatabase(TestDatabase db, LogConsole console)
        {
            var templateDb = db.IsTemplateDatabase ? null : TestDatabases.FirstOrDefault(d => d.IsTemplateDatabase && d.DbmsUser == db.DbmsUser);

            if (templateDb != null)
            {
                await CopyTestDatabase(db, templateDb, console);
            }
            else
            {
                await CreateTestDatabase(db, console);
            }
        }

        public async Task CreateTestDatabase(TestDatabase db, LogConsole console)
        {
            if (await DatabaseExists(db.Name, console))
                return;

            console.WriteCaption($"Create database {db.Name}");

            using var cli = new CliSession(console);
            await cli.ExecuteAsync("createdb", $@"-i -u\""{db.DbmsUser}\"" -no_x100 {db.Name}");

            using var session = new IngresSession(Config.GetConnectionString("iidbdb"), console);

            await session.ExecuteAsync($@"
                grant access, db_admin on database {db.Name} to public
            ", ignoreErrors: true);

            await AddExistingDatabase(db, console);
        }

        public async Task CopyTestDatabase(TestDatabase db, TestDatabase templateDb, LogConsole console)
        {
            if (await DatabaseExists(db.Name, console))
                return;

            await CreateTestDatabase(templateDb, console);

            console.WriteCaption($"Copy database {db.Name} from {templateDb.Name}");

            using var cli = new CliSession(console);
            await cli.ExecuteAsync("relocatedb", $@"-new_database={db.Name} {templateDb.Name}");

            await AddExistingDatabase(db, console);
        }

        public async Task DropDatabases(LogConsole console)
        {
            foreach (var db in TestDatabases.Where(d => d.Drop || d.Create).OrderBy(db => db.Name))
            {
                await DropDatabase(db, console);
            }
        }

        public async Task DropDatabase(TestDatabase db, LogConsole console)
        {
            var (name, owner) = await GetExistingDatabase(db.Name, console);

            if (name is null)
                return;

            console.WriteCaption($"Drop database {db.Name}");
            using var cli = new CliSession(console);
            await cli.ExecuteAsync("destroydb", $@"-u\""{owner}\"" {name}");
            await RemoveExistingDatabase(db.Name, console);
        }


        private void HandleRootCommand()
        {
            Console.WriteLine($"InstallationPath:     {Config.InstallationPath}");
            Console.WriteLine($"InstallationCode:     {Config.InstallationCode}");
            Console.WriteLine($"BaseConnectionString: {Config.EFCoreTestConnectionString}");
        }

        private void AddCommand(BuildCommand command)
        {
            _commands.Add(command);
            RootCommand.Add(command);
        }
    }
}
