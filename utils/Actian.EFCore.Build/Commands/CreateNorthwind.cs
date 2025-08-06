// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace Actian.EFCore.Build.Commands
{
    public class CreateNorthwind : BuildCommand
    {
        public CreateNorthwind(BuildContext context)
            : base(context, "create-northwind")
        {
        }

        public override async Task RunInternal()
        {
            var northwindDb = Context.TestDatabases.FirstOrDefault(db => db.Aliases.Contains("Northwind"));

            if (northwindDb is null)
                throw new Exception("Northwind test database not found");

            using var console = new LogConsole($"Create northwind database ({northwindDb.Name})", buffer: false);

            await Context.DropDatabase(northwindDb, console);
            await Context.CreateTestDatabase(northwindDb, console);
            await Context.RunCommand<PopulateNorthwind>();
        }
    }
}
