// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Actian.EFCore
{
    public abstract class ConnectionInterceptionActianTestBase : ConnectionInterceptionTestBase
    {
        protected ConnectionInterceptionActianTestBase(InterceptionActianFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName
                => "ConnectionInterception";

            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
        }

        protected override BadUniverseContext CreateBadUniverse(DbContextOptionsBuilder optionsBuilder)
            => new(optionsBuilder.UseActian(new FakeDbConnection()).Options);

        public class FakeDbConnection : DbConnection
        {
            [AllowNull]
            public override string ConnectionString { get; set; }

            public override string Database
                => "Database";

            public override string DataSource
                => "DataSource";

            public override string ServerVersion
                => throw new NotImplementedException();

            public override ConnectionState State
                => ConnectionState.Closed;

            public override void ChangeDatabase(string databaseName)
                => throw new NotImplementedException();

            public override void Close()
                => throw new NotImplementedException();

            public override void Open()
                => throw new NotImplementedException();

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
                => throw new NotImplementedException();

            protected override DbCommand CreateDbCommand()
                => throw new NotImplementedException();
        }

        public class ConnectionInterceptionActianTest
            : ConnectionInterceptionActianTestBase, IClassFixture<ConnectionInterceptionActianTest.InterceptionActianFixture>
        {
            public ConnectionInterceptionActianTest(InterceptionActianFixture fixture)
                : base(fixture)
            {
            }

            protected override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseActian("Database=Dummy");

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener
                    => false;
            }
        }

        public class ConnectionInterceptionWithConnectionStringActianTest
            : ConnectionInterceptionActianTestBase,
                IClassFixture<ConnectionInterceptionWithConnectionStringActianTest.InterceptionActianFixture>
        {
            public ConnectionInterceptionWithConnectionStringActianTest(InterceptionActianFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener
                    => false;
            }

            protected override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseActian("Database=Dummy");
        }

        public class ConnectionInterceptionWithDiagnosticsActianTest
            : ConnectionInterceptionActianTestBase,
                IClassFixture<ConnectionInterceptionWithDiagnosticsActianTest.InterceptionActianFixture>
        {
            public ConnectionInterceptionWithDiagnosticsActianTest(InterceptionActianFixture fixture)
                : base(fixture)
            {
            }

            protected override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseActian("Database=Dummy");

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener
                    => true;
            }
        }
    }
}
