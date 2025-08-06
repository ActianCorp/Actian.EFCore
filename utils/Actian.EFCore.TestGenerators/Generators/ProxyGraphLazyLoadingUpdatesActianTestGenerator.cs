// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class ProxyGraphLazyLoadingUpdatesActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new ProxyGraphLazyLoadingUpdatesActianTestGenerator().GenerateFile();
        }

        private ProxyGraphLazyLoadingUpdatesActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "ProxyGraphUpdatesTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "ProxyGraphUpdatesSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "ProxyGraphLazyLoadingUpdatesActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.ChangeTracking;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Storage;
                using Microsoft.EntityFrameworkCore.TestUtilities;
                using Microsoft.Extensions.DependencyInjection;
                using Xunit;

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
                public class ProxyGraphLazyLoadingUpdatesActianTest : ProxyGraphUpdatesTestBase<ProxyGraphLazyLoadingUpdatesActianTest.ProxyGraphLazyLoadingUpdatesActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public ProxyGraphLazyLoadingUpdatesActianTest(ProxyGraphLazyLoadingUpdatesActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());

                public class ProxyGraphLazyLoadingUpdatesActianFixture : ProxyGraphUpdatesFixtureBase
                {
                    public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override string StoreName { get; } = ""ProxyGraphLazyLoadingUpdatesTest"";

                    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                        => base.AddOptions(builder.UseLazyLoadingProxies());

                    protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                        => base.AddServices(serviceCollection.AddEntityFrameworkProxies());

                    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                    {
                        modelBuilder.UseIdentityColumns();

                        base.OnModelCreating(modelBuilder, context);
                    }
                }
            ");
        }
    }
}
