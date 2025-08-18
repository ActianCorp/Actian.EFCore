// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Actian.EFCore.TestUtilities
{
    public class TestDatabases
    {
        public static TestDatabases Load(string path)
        {
            return new TestDatabases(JsonConvert.DeserializeObject<IEnumerable<TestDatabase>>(File.ReadAllText(path)));
        }

        private TestDatabases(IEnumerable<TestDatabase> databases)
        {
            Databases = databases;
            foreach (var db in Databases)
            {
                var messages = new List<string>();
                db.Valid = true;
                db.Messages = messages;
                if (db.Name.Length > 24)
                {
                    db.Valid = false;
                    messages.Add("Database name too long");
                }
                if (Databases.Any(db2 => db2 != db && db2.Name == db.Name))
                {
                    db.Valid = false;
                    messages.Add("Duplicate database name");
                }
                foreach (var alias in db.Aliases)
                {
                    if (Databases.Any(db2 => db2 != db && db2.Aliases.Contains(alias)))
                    {
                        db.Valid = false;
                        messages.Add($"Duplicate alias {alias}");
                    }
                }
            }
        }

        public IEnumerable<TestDatabase> Databases { get; set; } = Enumerable.Empty<TestDatabase>();

        public bool Valid => Databases
            .All(db => db.Valid);

        public IEnumerable<string> Messages => Databases
            .OrderBy(db => db.Name)
            .SelectMany(db => db.Messages.Select(message => $"{db.Name}: {message}"));

        public TestDatabase GetTestDatabase(string name, bool strict = true)
        {
            if (name is null)
                return strict ? throw new ArgumentNullException(nameof(name)) : (TestDatabase)null;

            if (string.IsNullOrWhiteSpace(name))
                return strict ? throw new Exception($"Argument {nameof(name)} must have a value") : (TestDatabase)null;

            var testDatabases = Databases.Where(db => db.Matches(name)).ToList();
            return testDatabases.Count switch
            {
                0 => throw new Exception($"No test database found matching {name}"),
                1 => testDatabases.Single(),
                _ => throw new Exception($"{testDatabases.Count} test databases found matching {name}: {string.Join(", ", testDatabases.Select(db => db.Name))}"),
            };
        }

        public string GetConnectionString(string name)
            => GetTestDatabase(name).ConnectionString;

        public string GetDatabaseName(string name)
            => GetTestDatabase(name).Name;
    }
}
