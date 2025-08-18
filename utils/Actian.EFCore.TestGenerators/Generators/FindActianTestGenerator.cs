// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class FindActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new FindActianTestGenerator().GenerateFile();
        }

        private FindActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "FindTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "FindSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "FindActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
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
                public abstract class FindActianTest : FindTestBase<FindActianTest.FindActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected FindActianTest(FindActianFixture fixture, ITestOutputHelper testOutputHelper)
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

                public class FindActianTestSet : FindActianTest
                {
                    public FindActianTestSet(FindActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture, testOutputHelper)
                    {
                    }

                    protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                        => context.Set<TEntity>().Find(keyValues);

                    protected override ValueTask<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                        => context.Set<TEntity>().FindAsync(keyValues);
                }

                public class FindActianTestContext : FindActianTest
                {
                    public FindActianTestContext(FindActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture, testOutputHelper)
                    {
                    }

                    protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                        => context.Find<TEntity>(keyValues);

                    protected override ValueTask<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                        => context.FindAsync<TEntity>(keyValues);
                }

                public class FindActianTestNonGeneric : FindActianTest
                {
                    public FindActianTestNonGeneric(FindActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture, testOutputHelper)
                    {
                    }

                    protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                        => (TEntity)context.Find(typeof(TEntity), keyValues);

                    protected override async ValueTask<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                        => (TEntity)await context.FindAsync(typeof(TEntity), keyValues);
                }

                private string Sql => Fixture.TestSqlLoggerFactory.Sql;

                private void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);

                public class FindActianFixture : FindFixtureBase, IActianSqlFixture
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                }
            ");
        }
    }
}
