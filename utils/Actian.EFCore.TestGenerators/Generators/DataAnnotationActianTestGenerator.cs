﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class DataAnnotationActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new DataAnnotationActianTestGenerator().GenerateFile();
        }

        private DataAnnotationActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "DataAnnotationTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "DataAnnotationSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "DataAnnotationActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Metadata;
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
                public class DataAnnotationActianTest : DataAnnotationTestBase<DataAnnotationActianTest.DataAnnotationActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected DataAnnotationActianTest(DataAnnotationActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {
                    TestEnvironment.Log(this, testOutputHelper);
                    Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                }

                public ActianSqlFixtureHelpers Helpers { get; }

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                private static readonly string _eol = Environment.NewLine;

                private void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);

                public class DataAnnotationActianFixture : DataAnnotationFixtureBase, IActianSqlFixture
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                }
            ");
        }
    }
}
