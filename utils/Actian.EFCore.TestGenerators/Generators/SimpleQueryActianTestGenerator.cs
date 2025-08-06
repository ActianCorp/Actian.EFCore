// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.CodeDom.Compiler;
using System.IO;

namespace Actian.EFCore.TestGenerators.Generators
{
    public abstract class SimpleQueryActianTestGenerator : TestGenerator
    {
        public static void Generate()
        {
            new General().GenerateFile();
            new Functions().GenerateFile();
            new JoinGroupJoin().GenerateFile();
            new KeylessEntities().GenerateFile();
            new ResultOperators().GenerateFile();
            new Select().GenerateFile();
            new SetOperations().GenerateFile();
            new Where().GenerateFile();
        }

        private SimpleQueryActianTestGenerator(string name)
            : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public override string[] EFPaths => new[]
        {
            Path.Combine(Paths.EFCoreSpecificationTests, "Query", string.IsNullOrWhiteSpace(Name) ? "SimpleQueryTestBase.cs" : $"SimpleQueryTestBase.{Name}.cs")
        };

        public override string SqlServerPath => Path.Combine(Paths.EFCoreSqlServerFunctionalTests, "Query", string.IsNullOrWhiteSpace(Name) ? "SimpleQuerySqlServerTest.cs" : $"SimpleQuerySqlServerTest.{Name}.cs");
        public override string ActianPath => Path.Combine(Paths.ActianEFCoreFunctionalTests, "Query", string.IsNullOrWhiteSpace(Name) ? "SimpleQueryActianTest.cs" : $"SimpleQueryActianTest.{Name}.cs");

        protected override void WriteUsings(IndentedTextWriter writer)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Threading.Tasks;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore.TestModels.Northwind;");
            writer.WriteLine("using Xunit;");
            writer.WriteLine();
        }

        protected override void WriteNamespaceDeclaration(IndentedTextWriter writer)
        {
            writer.WriteLine($"namespace Actian.EFCore.Query");
        }

        protected override void WriteClassDeclaration(IndentedTextWriter writer)
        {
            writer.WriteLine($"partial class SimpleQueryActianTest");
        }

        private class General : SimpleQueryActianTestGenerator
        {
            public General() : base("") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class Functions : SimpleQueryActianTestGenerator
        {
            public Functions() : base("Functions") { }
        }

        private class JoinGroupJoin : SimpleQueryActianTestGenerator
        {
            public JoinGroupJoin() : base("JoinGroupJoin") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class KeylessEntities : SimpleQueryActianTestGenerator
        {
            public KeylessEntities() : base("KeylessEntities") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class ResultOperators : SimpleQueryActianTestGenerator
        {
            public ResultOperators() : base("ResultOperators") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class Select : SimpleQueryActianTestGenerator
        {
            public Select() : base("Select") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class SetOperations : SimpleQueryActianTestGenerator
        {
            public SetOperations() : base("SetOperations") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }

        private class Where : SimpleQueryActianTestGenerator
        {
            public Where() : base("Where") { }

            protected override void WriteUsings(IndentedTextWriter writer)
            {
                writer.WriteLine("using System.Threading.Tasks;");
                writer.WriteLine("using Xunit;");
                writer.WriteLine();
            }
        }
    }
}
