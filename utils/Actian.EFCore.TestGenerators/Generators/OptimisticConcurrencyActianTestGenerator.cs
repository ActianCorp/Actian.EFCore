// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class OptimisticConcurrencyActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new OptimisticConcurrencyActianTestGenerator().GenerateFile();
        }

        private OptimisticConcurrencyActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "OptimisticConcurrencyTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "OptimisticConcurrencySqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "OptimisticConcurrencyActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Storage;
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
                public class OptimisticConcurrencyActianTest : OptimisticConcurrencyTestBase<F1ActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public OptimisticConcurrencyActianTest(F1ActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                    => facade.UseTransaction(transaction.GetDbTransaction());
            ");
        }
    }
}
