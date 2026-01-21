// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class ConnectionInterceptionActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new ConnectionInterceptionActianTestGenerator().GenerateFile();
        }

        private ConnectionInterceptionActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "InterceptionTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "ConnectionInterceptionTestBase.cs")
        };

        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "ConnectionInterceptionSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "ConnectionInterceptionActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System;
                using System.Collections.Generic;
                using System.Data;
                using System.Data.Common;
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Diagnostics;
                using Microsoft.EntityFrameworkCore.TestUtilities;
                using Microsoft.Extensions.DependencyInjection;
                using Xunit;
                using Xunit.Abstractions;

            ");
        }

        protected override void WriteNamespaceDeclaration(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                namespace Actian.EFCore
            ");
        }

        protected override void WriteClassDeclaration(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public abstract class ConnectionInterceptionActianTest : ConnectionInterceptionTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected ConnectionInterceptionActianTest(InterceptionActianFixtureBase fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
                {
                    protected override string StoreName => ""ConnectionInterception"";
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override IServiceCollection InjectInterceptors(
                        IServiceCollection serviceCollection,
                        IEnumerable<IInterceptor> injectedInterceptors)
                        => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
                }

                protected override BadUniverseContext CreateBadUniverse(DbContextOptionsBuilder optionsBuilder)
                    => new BadUniverseContext(optionsBuilder.UseActian(new FakeDbConnection()).Options);

                public class FakeDbConnection : DbConnection
                {
                    public override string ConnectionString { get; set; }
                    public override string Database => ""Database"";
                    public override string DataSource => ""DataSource"";
                    public override string ServerVersion => throw new NotImplementedException();
                    public override ConnectionState State => ConnectionState.Closed;
                    public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();
                    public override void Close() => throw new NotImplementedException();
                    public override void Open() => throw new NotImplementedException();
                    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotImplementedException();
                    protected override DbCommand CreateDbCommand() => throw new NotImplementedException();
                }

                public class WithoutDiagnostics : ConnectionInterceptionActianTest, IClassFixture<WithoutDiagnostics.InterceptionActianFixture>
                {
                    public WithoutDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(ConnectionInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => false;
                    }
                }

                public class WithDiagnostics : ConnectionInterceptionActianTest, IClassFixture<WithDiagnostics.InterceptionActianFixture>
                {
                    public WithDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(ConnectionInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => true;
                    }
                }
            ");
        }
    }
}
