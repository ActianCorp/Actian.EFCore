// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class MonsterFixupChangedChangingActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new MonsterFixupChangedChangingActianTestGenerator().GenerateFile();
        }

        private MonsterFixupChangedChangingActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "MonsterFixupTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "MonsterFixupChangedChangingSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "MonsterFixupChangedChangingActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.TestUtilities;
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
                public class MonsterFixupChangedChangingActianTest : MonsterFixupTestBase<MonsterFixupChangedChangingActianTest.MonsterFixupChangedChangingActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public MonsterFixupChangedChangingActianTest(MonsterFixupChangedChangingActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public class MonsterFixupChangedChangingActianFixture : MonsterFixupChangedChangingFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override void OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(
                        ModelBuilder builder)
                    {
                        base.OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(builder);

                        //builder.Entity<TMessage>().Property(e => e.MessageId).UseIdentityColumn(); // Defined in SqlServerPropertyBuilderExtensions

                        builder.Entity<TProduct>()
                            .OwnsOne(
                                c => (TDimensions)c.Dimensions, db =>
                                {
                                    db.Property(d => d.Depth).HasColumnType(""decimal(18,2)"");
                                    db.Property(d => d.Width).HasColumnType(""decimal(18,2)"");
                                    db.Property(d => d.Height).HasColumnType(""decimal(18,2)"");
                                });

                        //builder.Entity<TProductPhoto>().Property(e => e.PhotoId).UseIdentityColumn(); // Defined in SqlServerPropertyBuilderExtensions
                        //builder.Entity<TProductReview>().Property(e => e.ReviewId).UseIdentityColumn(); // Defined in SqlServerPropertyBuilderExtensions

                        builder.Entity<TComputerDetail>()
                            .OwnsOne(
                                c => (TDimensions)c.Dimensions, db =>
                                {
                                    db.Property(d => d.Depth).HasColumnType(""decimal(18,2)"");
                                    db.Property(d => d.Width).HasColumnType(""decimal(18,2)"");
                                    db.Property(d => d.Height).HasColumnType(""decimal(18,2)"");
                                });
                    }
                }
            ");
        }
    }
}
