// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Actian.EFCore.Migrations.Internal
{
    public static class ActianMigrationCommandListBuilderExtensions
    {
        /// <summary>
        /// Returns a new <see cref="ActianWithClauseBuilder"/> to build a with clause.
        /// </summary>
        /// <param name="builder">The command builder to use to add the with clause.</param>
        /// <returns>A new <see cref="ActianWithClauseBuilder"/></returns>
        public static ActianWithClauseBuilder WithClause([NotNull] this MigrationCommandListBuilder builder)
        {
            return new ActianWithClauseBuilder(builder);
        }
    }
}
