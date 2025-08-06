// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class LoggingActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new LoggingActianTestGenerator().GenerateFile();
        }

        private LoggingActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "LoggingTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "LoggingRelationalTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "LoggingSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "LoggingActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System;
                using Actian.EFCore.Infrastructure;
                using Actian.EFCore.Infrastructure.Internal;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.Extensions.DependencyInjection;

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
                public class LoggingActianTest : LoggingRelationalTestBase<ActianDbContextOptionsBuilder, ActianOptionsExtension>
            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                protected override DbContextOptionsBuilder CreateOptionsBuilder(
                    IServiceCollection services,
                    Action<RelationalDbContextOptionsBuilder<ActianDbContextOptionsBuilder, ActianOptionsExtension>> relationalAction)
                    => new DbContextOptionsBuilder()
                        .UseInternalServiceProvider(services.AddEntityFrameworkActian().BuildServiceProvider())
                        .UseActian(TestEnvironment.GetConnectionString(""LoggingTest""), relationalAction);

                protected override string ProviderName => ""Actian.EFCore"";
            ");
        }
    }
}
