// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract class QueryActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new AsNoTracking().GenerateFile();
            new AsTracking().GenerateFile();
            new AsyncFromSqlQuery().GenerateFile();
            new AsyncGearsOfWarQuery().GenerateFile();
            new AsyncSimpleQuery().GenerateFile();
            new ChangeTracking().GenerateFile();
            new CompiledQuery().GenerateFile();
            new ComplexNavigationsQuery().GenerateFile();
            new ComplexNavigationsWeakQuery().GenerateFile();
            new DbFunctions().GenerateFile();
            new FiltersInheritance().GenerateFile();
            new Filters().GenerateFile();
            new FromSqlQuery().GenerateFile();
            new FromSqlSprocQuery().GenerateFile();
            new FunkyDataQuery().GenerateFile();
            new GearsOfWarFromSqlQuery().GenerateFile();
            new GearsOfWarQuery().GenerateFile();
            new GroupByQuery().GenerateFile();
            new IncludeAsync().GenerateFile();
            new IncludeOneToOne().GenerateFile();
            new Include().GenerateFile();
            new Inheritance().GenerateFile();
            new InheritanceRelationshipsQuery().GenerateFile();
            new MappingQuery().GenerateFile();
            new NullKeys().GenerateFile();
            new NullSemanticsQuery().GenerateFile();
            new OwnedQuery().GenerateFile();
            new QueryFilterFuncletization().GenerateFile();
            new QueryNavigations().GenerateFile();
            new QueryNoClientEval().GenerateFile();
            new QueryTagging().GenerateFile();
            new SpatialQueryGeography().GenerateFile();
            new SpatialQueryGeometry().GenerateFile();
            new SqlExecutor().GenerateFile();
            new UdfDbFunction().GenerateFile();
            new Warnings().GenerateFile();
        }

        private QueryActianTestGenerator() : base()
        {
        }

        public string Name => GetType().Name;

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"{Name}TestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs"),
            Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"Relational{Name}TestBase.cs")
        };
        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "Query", $"{Name}SqlServerTest.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "Query", $"{Name}ActianTest.cs");

        protected override void WriteNamespaceDeclaration(IndentedTextWriter writer)
        {
            writer.WriteText(@"
                namespace Actian.EFCore.Query
            ");
        }

        protected override void WriteClassDeclaration(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public class {Name}ActianTest : {Name}TestBase<NorthwindQueryActianFixture<NoopModelCustomizer>>
            ");
        }

        protected override void WriteClassInit(IndentedTextWriter writer)
        {
            writer.WriteText($@"
                public {Name}ActianTest(NorthwindQueryActianFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
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

                public void AssertSql(params string[] expected)
                    => Helpers.AssertSql(expected);
            ");
        }

        private class AsNoTracking : QueryActianTestGenerator
        {
            public AsNoTracking() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }
        }

        private class AsTracking : QueryActianTestGenerator
        {
            public AsTracking() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }
        }

        private class AsyncFromSqlQuery : QueryActianTestGenerator
        {
            public AsyncFromSqlQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }
        }

        private class AsyncGearsOfWarQuery : QueryActianTestGenerator
        {
            public AsyncGearsOfWarQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class AsyncGearsOfWarQueryActianTest : AsyncGearsOfWarQueryTestBase<GearsOfWarQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public AsyncGearsOfWarQueryActianTest(GearsOfWarQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class AsyncSimpleQuery : QueryActianTestGenerator
        {
            public AsyncSimpleQuery() : base()
            {
                Skip("ToListAsync_can_be_canceled", "The test hangs");
            }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }
        }

        private class ChangeTracking : QueryActianTestGenerator
        {
            public ChangeTracking() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class CompiledQuery : QueryActianTestGenerator
        {
            public CompiledQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class ComplexNavigationsQuery : QueryActianTestGenerator
        {
            public ComplexNavigationsQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class ComplexNavigationsQueryActianTest : ComplexNavigationsQueryTestBase<ComplexNavigationsQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public ComplexNavigationsQueryActianTest(ComplexNavigationsQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                    private bool SupportsOffset => true;

                ");
            }
        }

        private class ComplexNavigationsWeakQuery : QueryActianTestGenerator
        {
            public ComplexNavigationsWeakQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"ComplexNavigationsQueryTestBase.cs"),
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"ComplexNavigationsWeakQueryTestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class ComplexNavigationsWeakQueryActianTest : ComplexNavigationsWeakQueryTestBase<ComplexNavigationsWeakQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public ComplexNavigationsWeakQueryActianTest(ComplexNavigationsWeakQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class DbFunctions : QueryActianTestGenerator
        {
            public DbFunctions() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class FiltersInheritance : QueryActianTestGenerator
        {
            public FiltersInheritance() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class FiltersInheritanceActianTest : FiltersInheritanceTestBase<FiltersInheritanceActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public FiltersInheritanceActianTest(FiltersInheritanceActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class Filters : QueryActianTestGenerator
        {
            public Filters() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class {Name}ActianTest : {Name}TestBase<NorthwindQueryActianFixture<NorthwindFiltersCustomizer>>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public {Name}ActianTest(NorthwindQueryActianFixture<NorthwindFiltersCustomizer> fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class FromSqlQuery : QueryActianTestGenerator
        {
            public FromSqlQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Data.Common;
                    using Actian.EFCore.TestUtilities;
                    using Ingres.Client;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    protected override DbParameter CreateDbParameter(string name, object value)
                        => new IngresParameter { ParameterName = name, Value = value };
                ");
            }
        }

        private class FromSqlSprocQuery : QueryActianTestGenerator
        {
            public FromSqlSprocQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    protected override string TenMostExpensiveProductsSproc => @""""""dbo"""".""""Ten Most Expensive Products"""""";

                    protected override string CustomerOrderHistorySproc => @""""""dbo"""".""""CustOrderHist"""" @CustomerID = {0}"";
                ");
            }
        }

        private class FunkyDataQuery : QueryActianTestGenerator
        {
            public FunkyDataQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class FunkyDataQueryActianTest : FunkyDataQueryTestBase<FunkyDataQueryActianTest.FunkyDataQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public FunkyDataQueryActianTest(FunkyDataQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"
                    protected override void ClearLog()
                        => Fixture.TestSqlLoggerFactory.Clear();

                    public class FunkyDataQueryActianFixture : FunkyDataQueryFixtureBase, IActianSqlFixture
                    {
                        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    }
                ");
            }
        }

        private class GearsOfWarFromSqlQuery : QueryActianTestGenerator
        {
            public GearsOfWarFromSqlQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class GearsOfWarFromSqlQueryActianTest : GearsOfWarFromSqlQueryTestBase<GearsOfWarQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public GearsOfWarFromSqlQueryActianTest(GearsOfWarQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    protected override void ClearLog() => Fixture.TestSqlLoggerFactory.Clear();

                    private string Sql => Fixture.TestSqlLoggerFactory.Sql;
                ");
            }
        }

        private class GearsOfWarQuery : QueryActianTestGenerator
        {
            public GearsOfWarQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class GearsOfWarQueryActianTest : GearsOfWarQueryTestBase<GearsOfWarQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public GearsOfWarQueryActianTest(GearsOfWarQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class GroupByQuery : QueryActianTestGenerator
        {
            public GroupByQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System;
                    using System.Linq;
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class IncludeAsync : QueryActianTestGenerator
        {
            public IncludeAsync() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class IncludeOneToOne : QueryActianTestGenerator
        {
            public IncludeOneToOne() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class IncludeOneToOneActianTest : IncludeOneToOneTestBase<IncludeOneToOneActianTest.OneToOneQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public IncludeOneToOneActianTest(OneToOneQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    public class OneToOneQueryActianFixture : OneToOneQueryFixtureBase, IActianSqlFixture
                    {
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
                    }
                ");
            }
        }

        private class Include : QueryActianTestGenerator
        {
            public Include() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                base.WriteClassInit(writer);
                writer.WriteText($@"
                    private bool SupportsOffset => true;

                ");
            }
        }

        private class Inheritance : QueryActianTestGenerator
        {
            public Inheritance() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"InheritanceTestBase.cs"),
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"InheritanceRelationalTestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class InheritanceActianTest : InheritanceRelationalTestBase<InheritanceActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public InheritanceActianTest(InheritanceActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class InheritanceRelationshipsQuery : QueryActianTestGenerator
        {
            public InheritanceRelationshipsQuery() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class InheritanceRelationshipsQueryActianTest : InheritanceRelationshipsQueryTestBase<InheritanceRelationshipsQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public InheritanceRelationshipsQueryActianTest(InheritanceRelationshipsQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class MappingQuery : QueryActianTestGenerator
        {
            public MappingQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class MappingQueryActianTest : MappingQueryTestBase<MappingQueryActianTest.MappingQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public MappingQueryActianTest(MappingQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    public class MappingQueryActianFixture : MappingQueryFixtureBase, IActianSqlFixture
                    {
                        protected override ITestStoreFactory TestStoreFactory => ActianNorthwindTestStoreFactory.Instance;

                        protected override string DatabaseSchema { get; } = ""dbo"";

                        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                        {
                            base.OnModelCreating(modelBuilder, context);

                            modelBuilder.Entity<MappedCustomer>(
                                e =>
                                {
                                    e.Property(c => c.CompanyName2).Metadata.SetColumnName(""CompanyName"");
                                    e.Metadata.SetTableName(""Customers"");
                                    e.Metadata.SetSchema(""dbo"");
                                });

                            modelBuilder.Entity<MappedEmployee>()
                                .Property(c => c.EmployeeID)
                                .HasColumnType(""int"");
                        }
                    }
                ");
            }
        }

        private class NullKeys : QueryActianTestGenerator
        {
            public NullKeys() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class NullKeysActianTest : NullKeysTestBase<NullKeysActianTest.NullKeysActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public NullKeysActianTest(NullKeysActianFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(testOutputHelper);
                        TestEnvironment.LogTestClassImplements(testOutputHelper, typeof(NullKeysTestBase<NullKeysActianTest.NullKeysActianFixture>));
                    }}

                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                writer.WriteText(@"

                    public class NullKeysActianFixture : NullKeysFixtureBase
                    {
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    }
                ");
            }
        }

        private class NullSemanticsQuery : QueryActianTestGenerator
        {
            public NullSemanticsQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.Infrastructure;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestModels.NullSemanticsModel;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class NullSemanticsQueryActianTest : NullSemanticsQueryTestBase<NullSemanticsQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public NullSemanticsQueryActianTest(NullSemanticsQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    protected override void ClearLog()
                        => Fixture.TestSqlLoggerFactory.Clear();

                    protected override NullSemanticsContext CreateContext(bool useRelationalNulls = false)
                    {
                        var options = new DbContextOptionsBuilder(Fixture.CreateOptions());
                        if (useRelationalNulls)
                        {
                            new ActianDbContextOptionsBuilder(options).UseRelationalNulls();
                        }

                        var context = new NullSemanticsContext(options.Options);

                        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                        return context;
                    }
                ");
            }
        }

        private class OwnedQuery : QueryActianTestGenerator
        {
            public OwnedQuery() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"{Name}TestBase.cs"),
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"Relational{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class OwnedQueryActianTest : RelationalOwnedQueryTestBase<OwnedQueryActianTest.OwnedQueryActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public OwnedQueryActianTest(OwnedQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    public class OwnedQueryActianFixture : RelationalOwnedQueryFixture, IActianSqlFixture
                    {
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    }
                ");
            }
        }

        private class QueryFilterFuncletization : QueryActianTestGenerator
        {
            public QueryFilterFuncletization() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"{Name}TestBase.cs"),
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"Relational{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class QueryFilterFuncletizationActianTest : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationActianTest.QueryFilterFuncletizationActianFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public QueryFilterFuncletizationActianTest(QueryFilterFuncletizationActianFixture fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    public class QueryFilterFuncletizationActianFixture : QueryFilterFuncletizationRelationalFixture, IActianSqlFixture
                    {
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
                    }
                ");
            }
        }

        private class QueryNavigations : QueryActianTestGenerator
        {
            public QueryNavigations() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                base.WriteClassInit(writer);
                writer.WriteText($@"
                    private bool SupportsOffset => true;

                ");
            }

            protected override void WriteIfStatement(IndentedTextWriter writer, IfStatementSyntax ifStatementSyntax)
            {
                writer.WriteLine($"if ({ifStatementSyntax.Condition})".Replace("TestEnvironment.GetFlag(nameof(SqlServerCondition.SupportsOffset)) ?? true", "SupportsOffset"));
                WriteStatement(writer, ifStatementSyntax.Statement);
                if (ifStatementSyntax.Else is not null)
                {
                    writer.WriteLine($"else");
                    WriteStatement(writer, ifStatementSyntax.Else.Statement);
                }
            }
        }

        private class QueryNoClientEval : QueryActianTestGenerator
        {
            public QueryNoClientEval() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"{Name}TestBase.cs")
            };

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class QueryTagging : QueryActianTestGenerator
        {
            public QueryTagging() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }

        private class SpatialQueryGeography : QueryActianTestGenerator
        {
            public SpatialQueryGeography() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"SpatialQueryTestBase.cs"),
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"SpatialQueryTestBase.cs")
            };
            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "Query", $"SpatialQuerySqlServerGeographyTest.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "Query", $"SpatialQueryActianGeographyTest.cs");

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class SpatialQueryActianGeographyTest : SpatialQueryTestBase<SpatialQueryActianGeographyFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public SpatialQueryActianGeographyTest(SpatialQueryActianGeographyFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class SpatialQueryGeometry : QueryActianTestGenerator
        {
            public SpatialQueryGeometry() : base() { }

            public override string[] EFPaths => new[]
            {
                Path.Combine(Paths.EFCoreSpecificationTests, "Query", $"SpatialQueryTestBase.cs"),
                Path.Combine(Paths.EFCoreRelationalSpecificationTests, "Query", $"SpatialQueryTestBase.cs")
            };
            public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "Query", $"SpatialQuerySqlServerGeometryTest.cs");
            public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "Query", $"SpatialQueryActianGeometryTest.cs");

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Linq;
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
                    using NetTopologySuite.Geometries;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class SpatialQueryActianGeometryTest : SpatialQueryTestBase<SpatialQueryActianGeometryFixture>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public SpatialQueryActianGeometryTest(SpatialQueryActianGeometryFixture fixture, ITestOutputHelper testOutputHelper)
                        : base(fixture)
                    {{
                        TestEnvironment.Log(this, testOutputHelper);
                        Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
                    }}

                    public ActianSqlFixtureHelpers Helpers {{ get; }}

                ");
            }
        }

        private class SqlExecutor : QueryActianTestGenerator
        {
            public SqlExecutor() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Data.Common;
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Ingres.Client;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }

            protected override void WriteClassFinit(IndentedTextWriter writer)
            {
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    protected override DbParameter CreateDbParameter(string name, object value)
                        => new IngresParameter { ParameterName = name, Value = value };

                    protected override string TenMostExpensiveProductsSproc => @""""""dbo"""".""""Ten Most Expensive Products"""""";

                    protected override string CustomerOrderHistorySproc => @""""""dbo"""".""""CustOrderHist"""" @CustomerID"";

                    protected override string CustomerOrderHistoryWithGeneratedParameterSproc => @""""""dbo"""".""""CustOrderHist"""" @CustomerID = {0}"";
                ");
            }
        }

        private class UdfDbFunction : QueryActianTestGenerator
        {
            public UdfDbFunction() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;

                ");
            }

            protected override void WriteClassDeclaration(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public class UdfDbFunctionActianTests : UdfDbFunctionTestBase<UdfDbFunctionActianTests.Actian>
                ");
            }

            protected override void WriteClassInit(IndentedTextWriter writer)
            {
                writer.WriteText($@"
                    public UdfDbFunctionActianTests(Actian fixture, ITestOutputHelper testOutputHelper)
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
                base.WriteClassFinit(writer);

                writer.WriteText(@"

                    public class Actian : UdfFixtureBase, IActianSqlFixture
                    {
                        protected override string StoreName { get; } = ""UDFDbFunctionActianTests"";
                        protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

                        protected override void Seed(DbContext context)
                        {
                            base.Seed(context);

                            context.Database.ExecuteSqlRaw(
                                @""create function [dbo].[CustomerOrderCount] (@customerId int)
                                                                returns int
                                                                as
                                                                begin
                                                                    return (select count(id) from orders where customerId = @customerId);
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function[dbo].[StarValue] (@starCount int, @value nvarchar(max))
                                                                returns nvarchar(max)
                                                                    as
                                                                    begin
                                                                return replicate('*', @starCount) + @value
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function[dbo].[DollarValue] (@starCount int, @value nvarchar(max))
                                                                returns nvarchar(max)
                                                                    as
                                                                    begin
                                                                return replicate('$', @starCount) + @value
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function [dbo].[GetReportingPeriodStartDate] (@period int)
                                                                returns DateTime
                                                                as
                                                                begin
                                                                    return '1998-01-01'
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function [dbo].[GetCustomerWithMostOrdersAfterDate] (@searchDate Date)
                                                                returns int
                                                                as
                                                                begin
                                                                    return (select top 1 customerId
                                                                            from orders
                                                                            where orderDate > @searchDate
                                                                            group by CustomerId
                                                                            order by count(id) desc)
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function [dbo].[IsTopCustomer] (@customerId int)
                                                                returns bit
                                                                as
                                                                begin
                                                                    if(@customerId = 1)
                                                                        return 1

                                                                    return 0
                                                                end"");

                            context.Database.ExecuteSqlRaw(
                                @""create function [dbo].[IdentityString] (@customerName nvarchar(max))
                                                                returns nvarchar(max)
                                                                as
                                                                begin
                                                                    return @customerName;
                                                                end"");

                            context.SaveChanges();
                        }
                    }
                ");
            }
        }

        private class Warnings : QueryActianTestGenerator
        {
            public Warnings() : base() { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteText(@"
                    using System.Threading.Tasks;
                    using Actian.EFCore.TestUtilities;
                    using Microsoft.EntityFrameworkCore.Query;
                    using Microsoft.EntityFrameworkCore.TestUtilities;
                    using Xunit;
                    using Xunit.Abstractions;
        
                ");
            }
        }
    }
}
