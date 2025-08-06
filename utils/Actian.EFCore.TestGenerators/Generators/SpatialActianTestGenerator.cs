// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class SpatialActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new SpatialActianTestGenerator().GenerateFile();
        }

        private SpatialActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "SpatialTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "SpatialSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "SpatialActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Storage;
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
                public class SpatialActianTest : SpatialTestBase<SpatialActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public SpatialActianTest(SpatialActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {{
                    TestEnvironment.Log(testOutputHelper);
                    TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(SpatialTestBase<SpatialActianFixture>));
                }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());
            ");
        }
    }
}
