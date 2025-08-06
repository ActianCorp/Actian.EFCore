// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using Actian.EFCore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Infrastructure
{
    /// <summary>
    /// <para>
    ///     Allows Actian specific configuration to be performed on <see cref="DbContextOptions" />.
    /// </para>
    /// </summary>
    public class ActianDbContextOptionsBuilder
        : RelationalDbContextOptionsBuilder<ActianDbContextOptionsBuilder, ActianOptionsExtension>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActianDbContextOptionsBuilder" /> class.
        /// </summary>
        /// <param name="optionsBuilder"> The options builder. </param>
        public ActianDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        /// <summary>
        /// Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        public virtual ActianDbContextOptionsBuilder EnableRetryOnFailure()
            => ExecutionStrategy(c => new ActianRetryingExecutionStrategy(c));

        /// <summary>
        /// Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        public virtual ActianDbContextOptionsBuilder EnableRetryOnFailure(int maxRetryCount)
            => ExecutionStrategy(c => new ActianRetryingExecutionStrategy(c, maxRetryCount));

        /// <summary>
        /// Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
        /// </summary>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxRetryDelay"> The maximum delay between retries. </param>
        /// <param name="errorNumbersToAdd"> Additional SQL error numbers that should be considered transient. </param>
        public virtual ActianDbContextOptionsBuilder EnableRetryOnFailure(
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            [CanBeNull] ICollection<int> errorNumbersToAdd)
            => ExecutionStrategy(c => new ActianRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorNumbersToAdd));

        /// <summary>
        ///     Sets the compatibility level that EF Core will use when interacting with the database.
        ///     Defaults to <c>160</c>
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="compatibilityLevel"><see langword="false" /> to have null resource</param>
        public virtual ActianDbContextOptionsBuilder UseCompatibilityLevel(int compatibilityLevel)
            => WithOption(e => e.WithCompatibilityLevel(compatibilityLevel));
    }
}
