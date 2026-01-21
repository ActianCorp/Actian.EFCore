// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class LoadActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new LoadActianTestGenerator().GenerateFile();
        }

        private LoadActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "LoadTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "LoadSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "LoadActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using FluentAssertions;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                public class LoadActianTest : LoadTestBase<LoadActianTest.LoadActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected LoadActianTest(LoadActianFixture fixture)
                    : base(fixture)
                {
                    fixture.TestSqlLoggerFactory.Clear();
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                private string Sql { get; set; }

                protected override void ClearLog() => Fixture.TestSqlLoggerFactory.Clear();

                protected override void RecordLog() => Sql = Fixture.TestSqlLoggerFactory.Sql;

                private void AssertSql(string expected) => Sql.Should().NotDifferFrom(expected);

                public class LoadActianFixture : LoadFixtureBase
                {
                    public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                }
            ");
        }
    }
}
