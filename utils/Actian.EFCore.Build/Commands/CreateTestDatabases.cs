// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Threading.Tasks;

namespace Actian.EFCore.Build.Commands
{
    public class CreateTestDatabases : BuildCommand
    {
        public CreateTestDatabases(BuildContext context)
            : base(context, "create-test-databases")
        {
        }

        public override async Task RunInternal()
        {
            using var console = new LogConsole($"Creating test databases", buffer: false);
            await Context.EnsureTestDatabases(console);
        }
    }
}
