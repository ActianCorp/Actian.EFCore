// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using Actian.EFCore.Metadata.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for <see cref="IEntityType" /> for Actian-specific metadata.
    /// </summary>
    public static class ActianIndexExtensions
    {
        #region Persistent

        /// <summary>
        /// Returns a value indicating whether the index is persistent.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns><c>true</c> if the index is persistent</returns>
        public static bool IsPersistent([NotNull] this IIndex index)
            => index[ActianAnnotationNames.Persistence] as bool? ?? false;

        /// <summary>
        /// Sets whether the index is persistent.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="isPersistent">A value indicating whether the index is persistent</param>
        public static void SetPersistence([NotNull] this IMutableIndex index, bool isPersistent)
            => index.SetOrRemoveAnnotation(ActianAnnotationNames.Persistence, isPersistent);

        /// <summary>
        /// Sets whether the index is persistent.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="isPersistent">A value indicating whether the index is persistent</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetPersistence([NotNull] this IConventionIndex index, bool isPersistent, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(ActianAnnotationNames.Persistence, isPersistent, fromDataAnnotation);

        /// <summary>
        /// Gets the configuration source for the persistence setting.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The configuration source for the persistence setting</returns>
        public static ConfigurationSource? GetPersistenceConfigurationSource([NotNull] this IConventionIndex index)
            => index.FindAnnotation(ActianAnnotationNames.Persistence)?.GetConfigurationSource();

        #endregion

        #region IncludeProperties

        /// <summary>
        ///     Returns included property names, or <c>null</c> if they have not been specified.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The included property names, or <c>null</c> if they have not been specified. </returns>
        public static IReadOnlyList<string> GetIncludeProperties([NotNull] this IIndex index)
            => (string[])index[ActianAnnotationNames.Include];

        /// <summary>
        ///     Sets included property names.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="properties"> The value to set. </param>
        public static void SetIncludeProperties([NotNull] this IMutableIndex index, [NotNull] IReadOnlyList<string> properties)
            => index.SetOrRemoveAnnotation(ActianAnnotationNames.Include, properties);

        /// <summary>
        ///     Sets included property names.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <param name="properties"> The value to set. </param>
        public static void SetIncludeProperties([NotNull] this IConventionIndex index, [NotNull] IReadOnlyList<string> properties, bool fromDataAnnotation = false)
            => index.SetOrRemoveAnnotation(ActianAnnotationNames.Include, properties, fromDataAnnotation);

        #endregion
    }
}
