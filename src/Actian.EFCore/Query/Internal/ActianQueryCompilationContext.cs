// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.EntityFrameworkCore.Query;

namespace Actian.EFCore.Query.Internal
{
    public class ActianQueryCompilationContext : RelationalQueryCompilationContext
    {
        private readonly bool _multipleActiveResultSetsEnabled;

        public ActianQueryCompilationContext(
            QueryCompilationContextDependencies dependencies,
            RelationalQueryCompilationContextDependencies relationalDependencies,
            bool async,
            bool multipleActiveResultSetsEnabled)
            : base(dependencies, relationalDependencies, async)
        {
            _multipleActiveResultSetsEnabled = multipleActiveResultSetsEnabled;
        }

        public override bool IsBuffering
            => base.IsBuffering
                || (QuerySplittingBehavior == Microsoft.EntityFrameworkCore.QuerySplittingBehavior.SplitQuery
                    && !_multipleActiveResultSetsEnabled);

        /// <summary>
        ///     Tracks whether translation is currently within the argument of an aggregate method (e.g. MAX, COUNT); 
        /// </summary>
        /// <remarks>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </remarks>
        public virtual bool InAggregateFunction { get; set; }
    }
}
