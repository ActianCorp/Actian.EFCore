// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator
    {
        private class Identity : GraphUpdatesActianTestGenerator
        {
            public Identity() : base()
            {
            }

            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "GraphUpdatesSqlServerTestIdentity.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "GraphUpdatesActianTestIdentity.cs");

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
                    public class GraphUpdatesActianTestIdentity : GraphUpdatesActianTestBase<GraphUpdatesActianTestIdentity.GraphUpdatesWithIdentityActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public GraphUpdatesActianTestIdentity(GraphUpdatesWithIdentityActianFixture fixture)
                        : base(fixture)
                    {
                    }

                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                writer.WriteText(@"

                    public class GraphUpdatesWithIdentityActianFixture : GraphUpdatesActianFixtureBase
                    {
                        protected override string StoreName { get; } = ""GraphIdentityUpdatesTest"";

                        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                        {
                            modelBuilder.UseIdentityColumns();

                            base.OnModelCreating(modelBuilder, context);
                        }
                    }
                ");
            }
        }
    }
}
