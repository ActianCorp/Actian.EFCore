// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class ActianServiceCollectionExtensionsTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new ActianServiceCollectionExtensionsTestGenerator().GenerateFile();
        }

        private ActianServiceCollectionExtensionsTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "EntityFrameworkServiceCollectionExtensionsTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "RelationalServiceCollectionExtensionsTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "SqlServerServiceCollectionExtensionsTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "ActianServiceCollectionExtensionsTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
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
                public class ActianServiceCollectionExtensionsTest : RelationalServiceCollectionExtensionsTestBase
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public ActianServiceCollectionExtensionsTest()
                    : base(ActianTestHelpers.Instance)
                {
                }

            ");
        }
    }
}
