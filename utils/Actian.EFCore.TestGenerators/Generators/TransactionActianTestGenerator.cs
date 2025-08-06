// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class TransactionActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new TransactionActianTestGenerator().GenerateFile();
        }

        private TransactionActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "TransactionTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "TransactionSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "TransactionActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.Infrastructure;
                using Actian.EFCore.Storage.Internal;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.TestUtilities;
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
                public class TransactionActianTest : TransactionTestBase<TransactionActianTest.TransactionActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public TransactionActianTest(TransactionActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {{
                    TestEnvironment.Log(testOutputHelper);
                    TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(TransactionTestBase<TransactionActianFixture>));
                }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override bool SnapshotSupported => true;

                protected override bool AmbientTransactionsSupported => true;

                protected override DbContext CreateContextWithConnectionString()
                {
                    var options = Fixture.AddOptions(
                            new DbContextOptionsBuilder()
                                .UseActian(
                                    TestStore.ConnectionString,
                                    b => b.ApplyConfiguration().ExecutionStrategy(c => new ActianExecutionStrategy(c))))
                        .UseInternalServiceProvider(Fixture.ServiceProvider);

                    return new DbContext(options.Options);
                }

                public class TransactionActianFixture : TransactionFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override void Seed(PoolableDbContext context)
                    {
                        base.Seed(context);

                        context.Database.ExecuteSqlRaw(""ALTER DATABASE ["" + StoreName + ""] SET ALLOW_SNAPSHOT_ISOLATION ON"");
                        context.Database.ExecuteSqlRaw(""ALTER DATABASE ["" + StoreName + ""] SET READ_COMMITTED_SNAPSHOT ON"");
                    }

                    public override void Reseed()
                    {
                        using (var context = CreateContext())
                        {
                            context.Set<TransactionCustomer>().RemoveRange(context.Set<TransactionCustomer>());
                            context.SaveChanges();

                            base.Seed(context);
                        }
                    }

                    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    {
                        new ActianDbContextOptionsBuilder(
                                base.AddOptions(builder))
                            .ExecutionStrategy(c => new ActianExecutionStrategy(c));
                        return builder;
                    }
                }
            ");
        }
    }
}
