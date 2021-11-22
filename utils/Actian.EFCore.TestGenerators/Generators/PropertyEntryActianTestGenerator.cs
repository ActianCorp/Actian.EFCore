﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class PropertyEntryActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new PropertyEntryActianTestGenerator().GenerateFile();
        }

        private PropertyEntryActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "PropertyEntryTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "PropertyEntrySqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "PropertyEntryActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Actian.EFCore.TestUtilities;
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
                public class PropertyEntryActianTest : PropertyEntryTestBase<F1ActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public PropertyEntryActianTest(F1ActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {
                    TestEnvironment.Log(this, testOutputHelper);
                    Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                }

                public ActianSqlFixtureHelpers Helpers { get; }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                private void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);
            ");
        }
    }
}
