// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Actian.EFCore.TestGenerators
{
    public static class CSharpSyntaxExtensions
    {
        public static IEnumerable<string> GetModifiers(this MethodDeclarationSyntax method)
        {
            foreach (var modifier in method.Modifiers.Select(m => m.ToString()))
            {
                if (modifier == "virtual")
                    yield return "override";
                else
                    yield return modifier;
            }
        }

        public static bool HasConditionalTheory(this MethodDeclarationSyntax method)
        {
            return method.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == "ConditionalTheory"));
        }

        public static bool HasConditionalFact(this MethodDeclarationSyntax method)
        {
            return method.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == "ConditionalFact"));
        }

        public static bool IsTestMethod(this MethodDeclarationSyntax method)
        {
            return method.HasConditionalFact() || method.HasConditionalTheory();
        }

        public static bool IsAsync(this MethodDeclarationSyntax method)
        {
            return method.ReturnType.ToString() == "Task"
                || method.ReturnType.ToString().StartsWith("Task<")
                || method.ReturnType.ToString() == "ValueTask"
                || method.ReturnType.ToString().StartsWith("ValueTask<");
        }

        public static bool IsAssertSql(this StatementSyntax statement, out InvocationExpressionSyntax isAssertSql)
        {
            isAssertSql = null;

            if (statement is not ExpressionStatementSyntax expressionStatement)
                return false;

            if (expressionStatement.Expression is not InvocationExpressionSyntax invocationExpression)
                return false;

            if (!invocationExpression.IsAssertSql())
                return false;

            isAssertSql = invocationExpression;
            return true;
        }

        public static bool IsAssertSql(this InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.Expression is IdentifierNameSyntax identifierName
                && identifierName.Identifier.Text == "AssertSql";
        }

        public static IEnumerable<object> GetIsAssertSqlArgs(this InvocationExpressionSyntax invocationExpression)
        {
            foreach (var argument in invocationExpression.ArgumentList.Arguments)
            {
                if (argument.Expression is LiteralExpressionSyntax literalExpression)
                {
                    yield return literalExpression.ToString().Substring(2).RemoveLast(1);
                }
                else
                {
                    yield return argument;
                }
            }
        }
    }
}
