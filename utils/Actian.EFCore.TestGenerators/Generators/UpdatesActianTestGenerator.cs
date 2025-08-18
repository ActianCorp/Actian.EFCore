// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;


namespace Actian.EFCore.TestGenerators.Generators
{
    public class UpdatesActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new UpdatesActianTestGenerator().GenerateFile();
        }

        private UpdatesActianTestGenerator() : base()
        {
        }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "UpdatesTestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "UpdatesRelationalTestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "UpdatesSqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "UpdatesActianTest.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                using System.Linq;
                using Actian.EFCore.TestUtilities;
                using Microsoft.EntityFrameworkCore;
                using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
                using Xunit;
                using Xunit.Abstractions;

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
                public class UpdatesActianTest : UpdatesRelationalTestBase<UpdatesActianFixture>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public UpdatesActianTest(UpdatesActianFixture fixture, ITestOutputHelper testOutputHelper)
                    : base(fixture)
                {{
                    TestEnvironment.Log(this, testOutputHelper);
                    Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                }}

                public ActianSqlFixtureHelpers Helpers {{ get; }}

            ");
        }

        protected override void WriteClassFinit(IndentedTextWriter writer)
        {
            writer.WriteText(@"

                public override void Identifiers_are_generated_correctly()
                {
                    using (var context = CreateContext())
                    {
                        var entityType = context.Model.FindEntityType(
                            typeof(
                                LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly
                            ));
                        Assert.Equal(
                            ""LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorking~"",
                            entityType.GetTableName());
                        Assert.Equal(
                            ""PK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWork~"",
                            entityType.GetKeys().Single().GetName());
                        Assert.Equal(
                            ""FK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWork~"",
                            entityType.GetForeignKeys().Single().GetConstraintName());
                        Assert.Equal(
                            ""IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWork~"",
                            entityType.GetIndexes().Single().GetName());

                        var entityType2 = context.Model.FindEntityType(
                            typeof(
                                LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectlyDetails
                            ));

                        Assert.Equal(
                            ""LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkin~1"",
                            entityType2.GetTableName());
                        Assert.Equal(
                            ""PK_LoginDetails"",
                            entityType2.GetKeys().Single().GetName());
                        Assert.Equal(
                            ""ExtraPropertyWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCo~"",
                            entityType2.GetProperties().ElementAt(1).GetColumnName());
                        Assert.Equal(
                            ""ExtraPropertyWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingC~1"",
                            entityType2.GetProperties().ElementAt(2).GetColumnName());
                        Assert.Equal(
                            ""IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWor~1"",
                            entityType2.GetIndexes().Single().GetName());
                    }
                }

                public void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);
            ");
        }
    }
}
