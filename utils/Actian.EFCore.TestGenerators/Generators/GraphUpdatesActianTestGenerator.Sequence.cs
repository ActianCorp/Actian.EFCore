// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator
    {
        private class Sequence : GraphUpdatesActianTestGenerator
        {
            public Sequence() : base()
            {
            }

            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "GraphUpdatesSqlServerTestSequence.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "GraphUpdatesActianTestSequence.cs");

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.ChangeTracking;
                    using Xunit;

                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public class GraphUpdatesActianTestSequence : GraphUpdatesActianTestBase<GraphUpdatesActianTestSequence.GraphUpdatesWithSequenceActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public GraphUpdatesActianTestSequence(GraphUpdatesWithSequenceActianFixture fixture)
                        : base(fixture)
                    {
                    }

                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                writer.WriteText(@"

                    public class GraphUpdatesWithSequenceActianFixture : GraphUpdatesActianFixtureBase
                    {
                        protected override string StoreName { get; } = ""GraphSequenceUpdatesTest"";

                        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                        {
                            modelBuilder.UseHiLo(); // ensure model uses sequences
                            base.OnModelCreating(modelBuilder, context);
                        }
                    }
                ");
            }
        }
    }
}
