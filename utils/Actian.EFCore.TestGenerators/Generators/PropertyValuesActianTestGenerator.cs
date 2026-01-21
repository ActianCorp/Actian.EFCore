// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class PropertyValuesActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new PropertyValuesActianTestGenerator().GenerateFile();
        }

        private PropertyValuesActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "PropertyValuesTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "PropertyValuesSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "PropertyValuesActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Threading.Tasks;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                public class PropertyValuesActianTest : PropertyValuesTestBase<PropertyValuesActianTest.PropertyValuesActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public PropertyValuesActianTest(PropertyValuesActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public class PropertyValuesActianFixture : PropertyValuesFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                    {
                        base.OnModelCreating(modelBuilder, context);

                        modelBuilder.Entity<Building>()
                            .Property(b => b.Value).HasColumnType(""decimal(18,2)"");

                        modelBuilder.Entity<CurrentEmployee>()
                            .Property(ce => ce.LeaveBalance).HasColumnType(""decimal(18,2)"");
                    }
                }
            ");
        }
    }
}
