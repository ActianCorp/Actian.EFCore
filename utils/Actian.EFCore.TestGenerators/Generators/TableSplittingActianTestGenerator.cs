// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class TableSplittingActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new TableSplittingActianTestGenerator().GenerateFile();
        }

        private TableSplittingActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "TableSplittingTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "TableSplittingSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "TableSplittingActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
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
                public class TableSplittingActianTest : TableSplittingTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public TableSplittingActianTest(ITestOutputHelper testOutputHelper)
                    : base(testOutputHelper)
                {{
                    TestEnvironment.Log(this, testOutputHelper);
                    Helpers = new ActianSqlFixtureHelpers(TestSqlLoggerFactory, testOutputHelper);
                }}

                protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                public ActianSqlFixtureHelpers Helpers {{ get; }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected new void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);
            ");
        }
    }
}
