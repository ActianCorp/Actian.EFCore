// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator
    {
        private class Cascade : GraphUpdatesActianTestGenerator
        {
            public Cascade() : base()
            {
            }

            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "GraphUpdatesSqlServerTestClientCascade.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "GraphUpdatesActianTestClientCascade.cs");

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Linq;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.ChangeTracking;
                    using Xunit;

                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public class GraphUpdatesActianTestClientCascade : GraphUpdatesActianTestBase<GraphUpdatesActianTestClientCascade.GraphUpdatesWithClientCascadeActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public GraphUpdatesActianTestClientCascade(GraphUpdatesWithClientCascadeActianFixture fixture)
                        : base(fixture)
                    {
                    }

                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                writer.WriteText(@"

                    public class GraphUpdatesWithClientCascadeActianFixture : GraphUpdatesActianFixtureBase
                    {
                        protected override string StoreName { get; } = ""GraphClientCascadeUpdatesTest"";
                        public override bool NoStoreCascades => true;

                        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                        {
                            base.OnModelCreating(modelBuilder, context);

                            foreach (var foreignKey in modelBuilder.Model
                                .GetEntityTypes()
                                .SelectMany(e => e.GetDeclaredForeignKeys())
                                .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade))
                            {
                                foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                            }
                        }
                    }
                ");
            }
        }
    }
}
