// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class ActianMigrationSqlGeneratorTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new ActianMigrationSqlGeneratorTestGenerator().GenerateFile();
        }

        private ActianMigrationSqlGeneratorTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "MigrationSqlGeneratorTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "SqlServerMigrationSqlGeneratorTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "ActianMigrationSqlGeneratorTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Actian.EFCore.TestUtilities;
                using FluentAssertions;
                using Microsoft.EntityFrameworkCore;
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
                public class ActianMigrationSqlGeneratorTest : MigrationSqlGeneratorTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public ActianMigrationSqlGeneratorTest(ITestOutputHelper testOutputHelper)
                    : base(ActianTestHelpers.Instance)
                {
                    TestEnvironment.Log(testOutputHelper);
                    TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(MigrationSqlGeneratorTestBase));
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected new void AssertSql(string expected)
                    => Sql.Should().NotDifferFrom(expected);
            ");
        }
    }
}
