// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new Base().GenerateFile();
            new Cascade().GenerateFile();
            new NoAction().GenerateFile();
            new Identity().GenerateFile();
            new Sequence().GenerateFile();
        }

        private GraphUpdatesActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "GraphUpdatesTestBase.cs")
        };

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using Microsoft.EntityFrameworkCore.Infrastructure;
                using Microsoft.EntityFrameworkCore.Storage;
                using Microsoft.EntityFrameworkCore.TestUtilities;
                using Actian.EFCore.TestUtilities;
                using Xunit;

            ");
        }

        protected override void WriteNamespaceDeclaration(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                namespace Actian.EFCore
            ");
        }
    }
}
