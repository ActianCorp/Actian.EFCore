// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class ConcurrencyDetectorActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new ConcurrencyDetectorActianTestGenerator().GenerateFile();
        }

        private ConcurrencyDetectorActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "ConcurrencyDetectorTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "ConcurrencyDetectorRelationalTestBase.cs")
        };

        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "ConcurrencyDetectorSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "ConcurrencyDetectorActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System;
                using System.Threading.Tasks;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Query;
                using Microsoft.EntityFrameworkCore.TestModels.Northwind;
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
                public class ConcurrencyDetectorActianTest : ConcurrencyDetectorRelationalTestBase<NorthwindQueryActianFixture<NoopModelCustomizer>>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public ConcurrencyDetectorActianTest(NorthwindQueryActianFixture<NoopModelCustomizer> fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override async Task ConcurrencyDetectorTest(Func<NorthwindContext, Task> test)
                {
                    await base.ConcurrencyDetectorTest(test);

                    Assert.Empty(Fixture.TestSqlLoggerFactory.SqlStatements);
                }
            ");
        }
    }
}
