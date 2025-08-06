// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class CompositeKeyEndToEndActianTestTransplier : TestGenerator
    {
        public static void Generate()
        {
            new CompositeKeyEndToEndActianTestTransplier().GenerateFile();
        }

        private CompositeKeyEndToEndActianTestTransplier() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "CompositeKeyEndToEndTestBase.cs")
        };

        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "CompositeKeyEndToEndSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "CompositeKeyEndToEndActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
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
                public class CompositeKeyEndToEndActianTest : CompositeKeyEndToEndTestBase<CompositeKeyEndToEndActianTest.CompositeKeyEndToEndActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public CompositeKeyEndToEndActianTest(CompositeKeyEndToEndActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public class CompositeKeyEndToEndActianFixture : CompositeKeyEndToEndFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                }
            ");
        }
    }
}
