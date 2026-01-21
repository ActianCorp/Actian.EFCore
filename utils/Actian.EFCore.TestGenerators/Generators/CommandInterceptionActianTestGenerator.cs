// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class CommandInterceptionActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new CommandInterceptionActianTestGenerator().GenerateFile();
        }

        private CommandInterceptionActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "InterceptionTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "CommandInterceptionTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "CommandInterceptionSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "CommandInterceptionActianTest.cs");

        public override int DefaultTimeout => 10 * 1000; // 10 seconds

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Collections.Generic;
                using System.Threading.Tasks;
                using Actian.EFCore.Infrastructure;
                using Actian.EFCore.Storage.Internal;
                using Actian.EFCore.TestUtilities;
                using FluentAssertions;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Diagnostics;
                using Microsoft.EntityFrameworkCore.TestUtilities;
                using Microsoft.Extensions.DependencyInjection;
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
                public abstract class CommandInterceptionActianTest : CommandInterceptionTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected CommandInterceptionActianTest(InterceptionActianFixtureBase fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                private static new void AssertSql(string expected, string actual)
                {
                    actual.Should().NotDifferFrom(expected);
                }

                public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
                {
                    protected override string StoreName => ""CommandInterception"";
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override IServiceCollection InjectInterceptors(
                        IServiceCollection serviceCollection,
                        IEnumerable<IInterceptor> injectedInterceptors)
                        => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
                }

                public class WithoutDiagnostics : CommandInterceptionActianTest, IClassFixture<WithoutDiagnostics.InterceptionActianFixture>
                {
                    public WithoutDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(CommandInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => false;

                        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                        {
                            new ActianDbContextOptionsBuilder(base.AddOptions(builder))
                                .ExecutionStrategy(d => new ActianExecutionStrategy(d));
                            return builder;
                        }
                    }
                }

                public class WithDiagnostics : CommandInterceptionActianTest, IClassFixture<WithDiagnostics.InterceptionActianFixture>
                {
                    public WithDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(CommandInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => true;

                        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                        {
                            new ActianDbContextOptionsBuilder(base.AddOptions(builder))
                                .ExecutionStrategy(d => new ActianExecutionStrategy(d));
                            return builder;
                        }
                    }
                }
            ");
        }
    }
}
