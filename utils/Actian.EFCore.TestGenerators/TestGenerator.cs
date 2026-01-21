// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Actian.EFCore.TestGenerators
{
    public abstract class TestGenerator
    {
        private readonly Dictionary<string, string> _skips = new Dictionary<string, string>();
        private readonly Dictionary<string, int> _timeouts = new Dictionary<string, int>();

        protected TestGenerator()
        {
        }

        public abstract string[] EFPaths { get; }
        public abstract string SqlServerPath { get; }
        public abstract string ActianPath { get; }
        public virtual int DefaultTimeout => 10 * 60 * 1000; // 10 minutes

        protected void Skip(string methodName, string reason)
        {
            _skips[methodName] = reason;
        }

        protected void SetTimeout(string methodName, int milliseconds)
        {
            _timeouts[methodName] = milliseconds;
        }

        public virtual bool IncludeMethod(IEnumerable<MethodDeclarationSyntax> methods, MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(m => m.ToString() == "virtual");
        }

        public static IEnumerable<MethodDeclarationSyntax> GetMethods(Func<IEnumerable<MethodDeclarationSyntax>, MethodDeclarationSyntax, bool> predicate, params string[] paths)
        {
            var methods = new List<MethodDeclarationSyntax>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    continue;

                var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));

                methods.AddRange(tree.GetRoot().DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => predicate(methods, method))
                );
            }

            return methods;
        }

        public static void EnsureDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public virtual void GenerateFile()
        {
            Console.WriteLine(Path.GetFileName(ActianPath));

            EnsureDirectory(ActianPath);
            using var fileWriter = new StreamWriter(ActianPath, false, Encoding.UTF8) { NewLine = "\n" };
            using var writer = new IndentedTextWriter(fileWriter);

            WriteUsings(writer);
            WriteNamespace(writer);
        }

        protected virtual void WriteUsings(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteNamespace(IndentedTextWriter writer)
        {
            WriteNamespaceDeclaration(writer);
            writer.WriteLine("{");
            writer.Indent += 1;
            WriteNamespaceInit(writer);
            WriteClass(writer);
            WriteNamespaceFinit(writer);
            writer.Indent -= 1;
            writer.WriteLine("}");
        }

        protected virtual void WriteNamespaceDeclaration(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteNamespaceInit(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteNamespaceFinit(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteClass(IndentedTextWriter writer)
        {
            WriteClassDeclaration(writer);
            writer.WriteLine("{");
            writer.Indent += 1;
            WriteClassInit(writer);
            WriteTestMethods(writer);
            WriteClassFinit(writer);
            writer.Indent -= 1;
            writer.WriteLine("}");
        }

        protected virtual void WriteClassDeclaration(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteClassInit(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteClassFinit(IndentedTextWriter writer)
        {
        }

        protected virtual void WriteTestMethods(IndentedTextWriter writer)
        {
            var methods = GetMethods(IncludeMethod, EFPaths);

            var sqlServerMethods = GetMethods(
                (methods, method) => method.Modifiers.Any(m => m.ToString() == "override"),
                SqlServerPath
            );

            var first = true;
            foreach (var method in methods)
            {
                if (WriteTestMethod(method, sqlServerMethods, writer, first))
                    first = false;
            }
        }

        protected virtual bool WriteTestMethod(MethodDeclarationSyntax method, IEnumerable<MethodDeclarationSyntax> sqlServerMethods, IndentedTextWriter writer, bool first)
        {
            if (!method.IsTestMethod())
                return false;

            var modifiers = method.GetModifiers().Where(m => m != "async");

            if (!modifiers.Any(m => m == "virtual" || m == "override"))
                return false;

            method = method.WithAttributeLists(new SyntaxList<AttributeListSyntax>(method.AttributeLists.Where(l => !l.Attributes.Any(a => a.Name.ToString() == "MemberData"))));

            var sqlServerMethod = sqlServerMethods.FirstOrDefault(m => m.Identifier.ToString() == method.Identifier.ToString());

            if (!first)
                writer.WriteLine();

            if (method.HasConditionalFact())
            {
                writer.Write($"[ConditionalFact");
            }
            else if (method.HasConditionalTheory())
            {
                writer.Write($"[ConditionalTheory");
            }

            WriteTestProperties(writer, method.Identifier.ToString(), method, method.IsAsync());

            writer.WriteLine($"]");

            if (sqlServerMethod?.Body is null)
            {
                writer.Write(string.Join(" ", modifiers));
                writer.Write(" ");
                writer.Write(method.ReturnType);
                writer.Write(" ");
                writer.Write(method.Identifier);
                writer.Write(method.ParameterList);
            }
            else
            {
                writer.Write(string.Join(" ", sqlServerMethod.GetModifiers()));
                writer.Write(" ");
                writer.Write(sqlServerMethod.ReturnType);
                writer.Write(" ");
                writer.Write(sqlServerMethod.Identifier);
                writer.Write(sqlServerMethod.ParameterList);
            }
            writer.WriteLine();
            writer.WriteLine("{");
            writer.Indent += 1;
            WriteMethodBody(method, writer, modifiers, sqlServerMethod);
            writer.Indent -= 1;
            writer.WriteLine("}");

            return true;
        }

        protected virtual void WriteMethodBody(MethodDeclarationSyntax method, IndentedTextWriter writer, IEnumerable<string> modifiers, MethodDeclarationSyntax sqlServerMethod)
        {
            if (sqlServerMethod?.Body is null)
            {
                if (method.ReturnType.ToString() != "void")
                {
                    writer.Write("return ");
                }
                writer.WriteLine($"base.{method.Identifier}({string.Join(", ", method.ParameterList.Parameters.Select(p => p.Identifier.ToString()))});");
            }
            else
            {
                foreach (var statement in sqlServerMethod.Body.Statements)
                {
                    WriteStatement(writer, statement);
                }
            }
        }

        protected virtual void WriteTestProperties(IndentedTextWriter writer, string methodName, MethodDeclarationSyntax method, bool isAsync)
        {
            var properties = GetTestProperties(methodName, method, isAsync).ToList();
            if (!properties.Any())
                return;
            writer.Write($"({string.Join(", ", properties)})");
        }

        protected virtual IEnumerable<string> GetTestProperties(string methodName, MethodDeclarationSyntax method, bool isAsync)
        {
            if (_skips.TryGetValue(methodName, out var reason))
            {
                yield return $"Skip = \"{reason.Replace("\"", "\\\"")}\"";
            }

            if (_timeouts.TryGetValue(methodName, out var timeout))
            {
                yield return $"Timeout = {timeout}";
            }
            else if (isAsync)
            {
                yield return $"Timeout = {DefaultTimeout}";
            }
        }

        protected virtual void WriteStatement(IndentedTextWriter writer, StatementSyntax statement)
        {
            if (statement is BlockSyntax blockSyntax)
            {
                writer.WriteLine("{");
                writer.Indent += 1;
                foreach (var blockStatement in blockSyntax.Statements)
                {
                    WriteStatement(writer, blockStatement);
                }
                writer.Indent -= 1;
                writer.WriteLine("}");
            }
            else if (statement is IfStatementSyntax ifStatementSyntax)
            {
                WriteIfStatement(writer, ifStatementSyntax);
            }
            else if (statement.IsAssertSql(out var isAssertSql))
            {
                writer.Write("AssertSql(");

                if (isAssertSql.ArgumentList.Arguments.Count > 1)
                {
                    writer.WriteLine();
                    writer.Indent += 1;
                }

                var firstArgument = true;
                foreach (var argument in isAssertSql.ArgumentList.Arguments)
                {
                    if (!firstArgument)
                        writer.WriteLine(",");

                    if (argument.Expression is LiteralExpressionSyntax literalExpression)
                    {
                        WriteIsAssertSqlArgument(writer, literalExpression.ToString().Substring(2).RemoveLast(1));
                    }
                    else
                    {
                        writer.Write(argument.ToString());
                    }

                    firstArgument = false;
                }

                if (isAssertSql.ArgumentList.Arguments.Count > 1)
                {
                    writer.Indent -= 1;
                    writer.WriteLine();
                }
                writer.WriteLine(");");
            }
            else
            {
                writer.WriteLine($"{statement}".Replace("SqlServer", "Actian"));
            }
        }

        protected virtual void WriteIfStatement(IndentedTextWriter writer, IfStatementSyntax ifStatementSyntax)
        {
            writer.WriteLine($"if ({ifStatementSyntax.Condition})");
            WriteStatement(writer, ifStatementSyntax.Statement);
            if (ifStatementSyntax.Else is not null)
            {
                writer.WriteLine($"else");
                WriteStatement(writer, ifStatementSyntax.Else.Statement);
            }
        }

        protected void WriteIsAssertSqlArgument(IndentedTextWriter writer, string argument)
        {
            var lines = argument.NormalizeSql().ToList();
            switch (lines.Count)
            {
                case 0:
                    writer.Write("@\"\"");
                    break;
                case 1:
                    writer.Write($"@\"{lines[0]}\"");
                    break;
                default:
                    writer.WriteLine("@\"");
                    writer.Indent += 1;
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Indent -= 1;
                    writer.Write("\"");
                    break;
            }
        }
    }
}
