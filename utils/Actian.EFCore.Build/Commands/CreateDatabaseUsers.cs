// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actian.EFCore.Build.Commands
{
    public class CreateDatabaseUsers : BuildCommand
    {
        public CreateDatabaseUsers(BuildContext context)
            : base(context, "create-db-users")
        {
        }

        public override async Task RunInternal()
        {
            var userIds = new[] { "dbo", "db2", "db.2" };

            using var console = new LogConsole($"Creating database users");
            var existingUsers = await ListUsers(console);

            foreach (var userId in userIds)
            {
                using var session = new IngresSession(Config.GetConnectionString("iidbdb"), console);
                console.WriteCaption($"Create user {userId}");

                if (existingUsers.Contains(userId))
                {
                    await session.ExecuteAsync($@"
                        drop user ""{userId}""
                    ", ignoreErrors: true);
                }

                await session.ExecuteAsync($@"
                    create user ""{userId}"" 
                      with privileges = (createdb, trace, security, operator, maintain_users),
                           nopassword
                ", ignoreErrors: true);

                await session.ExecuteAsync($@"
                    alter user ""{userId}"" 
                     with privileges = (createdb, trace, security, operator, maintain_users),
                          nopassword
                ");
            }

            await ListUsers(console);
        }

        private static async Task<List<string>> ListUsers(LogConsole console)
        {
            List<string> userIds;
            using (var session = new IngresSession(Config.GetConnectionString("iidbdb"), console))
            {
                userIds = await session.SelectAsync($@"
                    select name from iiuser
                ", reader => reader.GetString(0));
            }

            console.WriteCaption("Existing users");
            foreach (var user in userIds)
            {
                console.WriteLine($"    {user}");
            }
            return userIds;
        }
    }
}
