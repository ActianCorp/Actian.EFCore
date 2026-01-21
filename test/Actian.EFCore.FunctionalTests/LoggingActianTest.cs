// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Reflection;
using Actian.EFCore.Diagnostics.Internal;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Infrastructure.Internal;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using TestEnvironment = Actian.EFCore.TestUtilities.TestEnvironment;

namespace Actian.EFCore
{
    public class LoggingActianTest : LoggingRelationalTestBase<ActianDbContextOptionsBuilder, ActianOptionsExtension>
    {
        public LoggingActianTest(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            IServiceCollection services,
            Action<RelationalDbContextOptionsBuilder<ActianDbContextOptionsBuilder, ActianOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder()
                .UseInternalServiceProvider(services.AddEntityFrameworkActian().BuildServiceProvider())
                .UseActian(TestEnvironment.GetConnectionString("LoggingTest"), relationalAction);

        protected override string ProviderName => "Actian.EFCore";

        protected override TestLogger CreateTestLogger()
            => new TestLogger<ActianLoggingDefinitions>();

        protected override string ProviderVersion
            => typeof(ActianOptionsExtension).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        [ActianTodo]
        public override void ForeignKeyPropertiesMappedToUnrelatedTables_throws_by_default()
        {
            base.ForeignKeyPropertiesMappedToUnrelatedTables_throws_by_default();
        }

        [ActianTodo]
        public override void IndexPropertiesBothMappedAndNotMappedToTable_throws_by_default()
        {
            base.IndexPropertiesBothMappedAndNotMappedToTable_throws_by_default();
        }

        [ActianTodo]
        public override void UnnamedIndexPropertiesMappedToNonOverlappingTables_throws_by_default()
        {
            base.UnnamedIndexPropertiesMappedToNonOverlappingTables_throws_by_default();
        }
    }
}
