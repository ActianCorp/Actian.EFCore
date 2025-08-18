// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class StoreGeneratedFixupActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new StoreGeneratedFixupActianTestGenerator().GenerateFile();
        }

        private StoreGeneratedFixupActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "StoreGeneratedFixupTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "StoreGeneratedFixupRelationalTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "StoreGeneratedFixupSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "StoreGeneratedFixupActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
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
                public class StoreGeneratedFixupActianTest : StoreGeneratedFixupRelationalTestBase<StoreGeneratedFixupActianTest.StoreGeneratedFixupActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public StoreGeneratedFixupActianTest(StoreGeneratedFixupActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {{
                    TestEnvironment.Log(testOutputHelper);
                    TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(StoreGeneratedFixupRelationalTestBase<StoreGeneratedFixupActianFixture>));
                }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void MarkIdsTemporary(DbContext context, object game, object level, object item)
                {
                    var entry = context.Entry(game);
                    entry.Property(""Id"").IsTemporary = true;

                    entry = context.Entry(item);
                    entry.Property(""Id"").IsTemporary = true;
                }

                protected override bool EnforcesFKs => true;

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());

                public class StoreGeneratedFixupActianFixture : StoreGeneratedFixupRelationalFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                    {
                        base.OnModelCreating(modelBuilder, context);

                        modelBuilder.Entity<Parent>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<Child>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ParentPN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ChildPN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ParentDN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ChildDN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ParentNN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ChildNN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<CategoryDN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ProductDN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<CategoryPN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ProductPN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<CategoryNN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<ProductNN>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<Category>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<Product>(
                            b =>
                            {
                                b.Property(e => e.Id1).ValueGeneratedOnAdd();
                                b.Property(e => e.Id2).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()"");
                            });

                        modelBuilder.Entity<Item>(b => b.Property(e => e.Id).ValueGeneratedOnAdd());

                        modelBuilder.Entity<Game>(b => b.Property(e => e.Id).ValueGeneratedOnAdd().HasDefaultValueSql(""newid()""));
                    }
                }
            ");
        }
    }
}
