// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class SeedingActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new SeedingActianTestGenerator().GenerateFile();
        }

        private SeedingActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "SeedingTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "SeedingSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "SeedingActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
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
                public class SeedingActianTest : SeedingTestBase
            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
                {
                    var context = new SeedingSqlServerContext(testId);
                    context.Database.EnsureClean();
                    return context;
                }

                protected class SeedingSqlServerContext : SeedingContext
                {
                    public SeedingSqlServerContext(string testId) : base(testId) { }

                    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                        => optionsBuilder.UseActian(TestEnvironment.GetConnectionString($""Seeds{TestId}""));
                }
            ");
        }
    }
}
