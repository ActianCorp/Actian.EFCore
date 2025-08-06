// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Actian specific extension methods for <see cref="IndexBuilder" />.
    /// </summary>
    public static class ActianIndexBuilderExtensions
    {
        #region Persistent

        /// <summary>
        /// Configures whether the index is persistent.
        /// </summary>
        /// <param name="indexBuilder">The builder for the index being configured.</param>
        /// <param name="isPersistent">A value indicating whether the index is persistent</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IndexBuilder WithPersistence(
            [NotNull] this IndexBuilder indexBuilder,
            bool isPersistent = true)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));

            indexBuilder.Metadata.SetPersistence(isPersistent);

            return indexBuilder;
        }

        /// <summary>
        /// Configures whether the index is persistent.
        /// </summary>
        /// <typeparam name="TEntity">The index being configured.</typeparam>
        /// <param name="indexBuilder">The builder for the index being configured.</param>
        /// <param name="isPersistent">A value indicating whether the index is persistent</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IndexBuilder<TEntity> WithPersistence<TEntity>(
            [NotNull] this IndexBuilder<TEntity> indexBuilder,
            bool isPersistent = true)
            where TEntity : class
        {
            return (IndexBuilder<TEntity>)WithPersistence((IndexBuilder)indexBuilder, isPersistent);
        }

        /// <summary>
        /// Configures whether the index is persistent.
        /// </summary>
        /// <param name="indexBuilder">The builder for the index being configured.</param>
        /// <param name="isPersistent">A value indicating whether the index is persistent</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IConventionIndexBuilder WithPersistence(
            [NotNull] this IConventionIndexBuilder indexBuilder,
            bool isPersistent = true,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));

            indexBuilder.Metadata.SetPersistence(isPersistent, fromDataAnnotation);

            return indexBuilder;
        }

        #endregion
    }
}
