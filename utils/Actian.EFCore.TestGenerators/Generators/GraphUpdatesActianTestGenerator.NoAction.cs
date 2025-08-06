// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator
    {
        private class NoAction : GraphUpdatesActianTestGenerator
        {
            public NoAction() : base()
            {
            }

            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "GraphUpdatesSqlServerTestClientNoAction.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "GraphUpdatesActianTestClientNoAction.cs");

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
                    public class GraphUpdatesActianTestClientNoAction : GraphUpdatesActianTestBase<GraphUpdatesActianTestClientNoAction.GraphUpdatesWithClientNoActionActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public GraphUpdatesActianTestClientNoAction(GraphUpdatesWithClientNoActionActianFixture fixture)
                        : base(fixture)
                    {
                    }

                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                writer.WriteText(@"

                    public class GraphUpdatesWithClientNoActionActianFixture : GraphUpdatesActianFixtureBase
                    {
                        protected override string StoreName { get; } = ""GraphClientNoActionUpdatesTest"";
                        public override bool ForceClientNoAction => true;

                        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                        {
                            base.OnModelCreating(modelBuilder, context);

                            foreach (var foreignKey in modelBuilder.Model
                                .GetEntityTypes()
                                .SelectMany(e => e.GetDeclaredForeignKeys()))
                            {
                                foreignKey.DeleteBehavior = DeleteBehavior.ClientNoAction;
                            }
                        }
                    }
                ");
            }
        }
    }
}
