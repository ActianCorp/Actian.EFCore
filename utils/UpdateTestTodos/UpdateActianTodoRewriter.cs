// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Actian.TestLoggers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UpdateTestTodos
{
    internal class UpdateActianTodoRewriter : CSharpSyntaxRewriter
    {
        public static void Rewrite(string path, IEnumerable<ActianTestResult> testResults, ref int changeCount)
        {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path, Encoding.UTF8));
            var rewriter = new UpdateActianTodoRewriter(testResults, changeCount);
            var newRoot = rewriter.Visit(tree.GetRoot());
            if (rewriter.ChangeCount > 0)
            {
                changeCount += rewriter.ChangeCount;
                var text = newRoot.GetText().ToString();
                File.WriteAllText(path, text, Encoding.UTF8);
            }
        }

        private UpdateActianTodoRewriter(IEnumerable<ActianTestResult> testResults, int previousChangeCount)
        {
            TestResults = testResults;
            PreviousChangeCount = previousChangeCount;
        }

        public IEnumerable<ActianTestResult> TestResults { get; }
        public int PreviousChangeCount { get; }
        public int ChangeCount { get; private set; } = 0;

        public string Namespace { get; set; }
        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            Namespace = node.Name.ToString();
            try
            {
                return base.VisitNamespaceDeclaration(node);
            }
            finally
            {
                Namespace = null;
            }
        }

        public string Class { get; set; }
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Class = $"{Namespace}.{node.Identifier.Text}";
            try
            {
                return base.VisitClassDeclaration(node);
            }
            finally
            {
                Class = null;
            }
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodName = $"{Class}.{node.Identifier.Text}";

            if (!HasAttribute(node, "ActianTodo") || !AllPassed(methodName))
                return base.VisitMethodDeclaration(node);

            ChangeCount += 1;

            Console.WriteLine($"{PreviousChangeCount + ChangeCount,5}: {methodName}");

            return RemoveAttribute(node, "ActianTodo");
        }

        private bool AllPassed(string methodName)
        {
            var hasTestresult = false;
            foreach (var testResult in TestResults.Where(r => r.FullyQualifiedName == methodName))
            {
                hasTestresult = true;
                if (testResult.Outcome != ActianTestOutcome.Passed)
                    return false;
            }
            return hasTestresult;
        }

        private bool HasAttribute(MethodDeclarationSyntax node, string name)
        {
            return node.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attr => attr.Name.ToString() == name);
        }

        private MethodDeclarationSyntax RemoveAttribute(MethodDeclarationSyntax node, string name)
        {
            var newAttributes = new SyntaxList<AttributeListSyntax>();

            foreach (var attributeList in node.AttributeLists)
            {
                var nodesToRemove = attributeList.Attributes
                    .Where(att => att.Name.ToString() == name)
                    .ToArray();

                if (nodesToRemove.Length == attributeList.Attributes.Count)
                {
                    //Do not add the attribute to the list. It's being removed completely.
                }
                else
                {
                    //We want to remove only some of the attributes
                    //var newAttribute = (AttributeListSyntax)VisitAttributeList(attributeList.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia));
                    //newAttributes = newAttributes.Add(newAttribute);
                    newAttributes = newAttributes.Add(attributeList.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia));
                }
            }

            //Get the leading trivia (the newlines and comments)
            var leadTriv = node.GetLeadingTrivia();
            node = node.WithAttributeLists(newAttributes);

            //Append the leading trivia to the method
            node = node.WithLeadingTrivia(leadTriv);
            return node;
        }
    }
}
