// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class LazyLoadProxyActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new LazyLoadProxyActianTestGenerator().GenerateFile();
        }

        private LazyLoadProxyActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "LazyLoadProxyTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "LazyLoadProxySqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "LazyLoadProxyActianTest.cs");

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
                public class LazyLoadProxyActianTest : LazyLoadProxyTestBase<LazyLoadProxyActianTest.LoadActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected LazyLoadProxyActianTest(LoadActianFixture fixture)
                    : base(fixture)
                {
                    fixture.TestSqlLoggerFactory.Clear();
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void ClearLog() => Fixture.TestSqlLoggerFactory.Clear();

                protected override void RecordLog() => Sql = Fixture.TestSqlLoggerFactory.Sql;

                private string Sql { get; set; }

                private void AssertSql(string expected) => Sql.Should().NotDifferFrom(expected);

                public class LoadActianFixture : LoadFixtureBase
                {
                    public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                }
            ");
        }

        protected override void WriteMethodBody(MethodDeclarationSyntax method, IndentedTextWriter writer, IEnumerable<string> modifiers, MethodDeclarationSyntax sqlServerMethod)
        {
            switch (method.Identifier.ToString())
            {
                case "Load_collection":
                    writer.WriteText(@"
                        await base.Load_collection(state, async);
                        if (!async)
                        {
                            // AssertSql(@""
                            //     @__p_0='707' (Nullable = true)
                            // 
                            //     SELECT [e].[Id], [e].[ParentId]
                            //     FROM [Child] AS [e]
                            //     WHERE [e].[ParentId] = @__p_0
                            // "");
                        }
                    ");
                    return;
                case "Top_level_projection_track_entities_before_passing_to_client_method":
                    writer.WriteText(@"
                        base.Top_level_projection_track_entities_before_passing_to_client_method();
                        AssertSql(@""
                            @__p_0='707' (Nullable = true)

                            SELECT [e].[Id], [e].[ParentId]
                            FROM [Child] AS [e]
                            WHERE [e].[ParentId] = @__p_0
                        "");
                    ");
                    return;
            }
            base.WriteMethodBody(method, writer, modifiers, sqlServerMethod);
        }
    }
}
