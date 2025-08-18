// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.Query;

namespace Actian.EFCore.Query.Internal
{
    public class ActianSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
    {
        public ActianSqlTranslatingExpressionVisitorFactory(
            RelationalSqlTranslatingExpressionVisitorDependencies dependencies)
        {
            Dependencies = dependencies;
        }
        protected virtual RelationalSqlTranslatingExpressionVisitorDependencies Dependencies { get; }

        public virtual RelationalSqlTranslatingExpressionVisitor Create(
            QueryCompilationContext queryCompilationContext,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            => new ActianSqlTranslatingExpressionVisitor(
                Dependencies,
                (ActianQueryCompilationContext)queryCompilationContext,
                queryableMethodTranslatingExpressionVisitor);
    }
}
