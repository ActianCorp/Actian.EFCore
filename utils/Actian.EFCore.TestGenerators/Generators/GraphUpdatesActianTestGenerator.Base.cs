// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract partial class GraphUpdatesActianTestGenerator
    {
        private class Base : GraphUpdatesActianTestGenerator
        {
            public Base() : base()
            {
            }

            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "GraphUpdatesSqlServerTestBase.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "GraphUpdatesActianTestBase.cs");

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.Infrastructure;
                    using Microsoft.EntityFrameworkCore.Storage;
                    using Microsoft.EntityFrameworkCore.TestUtilities;

                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    public abstract class GraphUpdatesActianTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
                        where TFixture : GraphUpdatesActianTestBase<TFixture>.GraphUpdatesActianFixtureBase, new()
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    protected GraphUpdatesActianTestBase(TFixture fixture)
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

                    public abstract class GraphUpdatesActianFixtureBase : GraphUpdatesFixtureBase
                    {
                        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    }
                ");
            }

            protected override void WriteTestMethods(IndentedTextWriter writer)
            {
            }
        }
    }
}
