// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public class NotificationEntitiesActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new NotificationEntitiesActianTestGenerator().GenerateFile();
        }

        private NotificationEntitiesActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "NotificationEntitiesTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "NotificationEntitiesSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "NotificationEntitiesActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
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
                public class NotificationEntitiesActianTest : NotificationEntitiesTestBase<NotificationEntitiesActianTest.NotificationEntitiesActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                public NotificationEntitiesActianTest(NotificationEntitiesActianFixture fixture)
                    : base(fixture)
                {
                }

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public class NotificationEntitiesActianFixture : NotificationEntitiesFixtureBase
                {
                    protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                }
            ");
        }
    }
}
