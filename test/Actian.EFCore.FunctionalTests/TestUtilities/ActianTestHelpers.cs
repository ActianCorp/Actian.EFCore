// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Diagnostics.Internal;
using Ingres.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Actian.EFCore.TestUtilities
{
    public class ActianTestHelpers : RelationalTestHelpers
    {
        protected ActianTestHelpers()
        {
        }

        public static ActianTestHelpers Instance { get; } = new ();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
                => services.AddEntityFrameworkActian();

        public override DbContextOptionsBuilder UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseActian(new IngresConnection("Database=DummyDatabase"));

        public override LoggingDefinitions LoggingDefinitions { get; } = new ActianLoggingDefinitions();
    }
}
