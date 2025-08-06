// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class TransactionInterceptionActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new TransactionInterceptionActianTestGenerator().GenerateFile();
        }

        private TransactionInterceptionActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "InterceptionTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "TransactionInterceptionTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "TransactionInterceptionSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "TransactionInterceptionActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Collections.Generic;
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
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
                public abstract class TransactionInterceptionActianTest : TransactionInterceptionTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected TransactionInterceptionActianTest(InterceptionActianFixtureBase fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
                {
                    protected override string StoreName => ""TransactionInterception"";
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override IServiceCollection InjectInterceptors(
                        IServiceCollection serviceCollection,
                        IEnumerable<IInterceptor> injectedInterceptors)
                        => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
                }

                public class WithoutDiagnostics : TransactionInterceptionActianTest, IClassFixture<WithoutDiagnostics.InterceptionActianFixture>
                {
                    public WithoutDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(TransactionInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => false;
                    }
                }

                public class WithDiagnostics : TransactionInterceptionActianTest, IClassFixture<WithDiagnostics.InterceptionActianFixture>
                {
                    public WithDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(TransactionInterceptionTestBase));
                    }

                    public class InterceptionActianFixture : InterceptionActianFixtureBase
                    {
                        protected override bool ShouldSubscribeToDiagnosticListener => true;
                    }
                }
            ");
        }
    }
}
