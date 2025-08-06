// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Threading.Tasks;

namespace Actian.EFCore.Build.Commands
{
    public class SetupTestDatabases : BuildCommand
    {
        public SetupTestDatabases(BuildContext context)
            : base(context, "setup-test-databases")
        {
        }

        public override async Task RunInternal()
        {
            await Context.RunCommand<DropTestDatabases>();
            await Context.RunCommand<CreateDatabaseUsers>();
            await Context.RunCommand<CreateTestDatabases>();
            await Context.RunCommand<PopulateNorthwind>();
        }
    }
}
