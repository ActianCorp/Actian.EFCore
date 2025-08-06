// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class WithConstructorsActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new WithConstructorsActianTestGenerator().GenerateFile();
        }

        private WithConstructorsActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "WithConstructorsTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "WithConstructorsSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "WithConstructorsActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Storage;
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
                public class WithConstructorsActianTest : WithConstructorsTestBase<WithConstructorsActianTest.WithConstructorsActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public WithConstructorsActianTest(WithConstructorsActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {{
                    TestEnvironment.Log(testOutputHelper);
                    TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(WithConstructorsTestBase<WithConstructorsActianFixture>));
                }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());

                public class WithConstructorsActianFixture : WithConstructorsFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                    {
                        base.OnModelCreating(modelBuilder, context);

                        modelBuilder.Entity<BlogQuery>().HasNoKey().ToQuery(
                            () => context.Set<BlogQuery>().FromSqlRaw(""SELECT * FROM Blog""));
                    }
                }
            ");
        }
    }
}
