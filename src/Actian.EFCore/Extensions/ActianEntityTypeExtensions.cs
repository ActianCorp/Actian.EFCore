// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Actian.EFCore.Metadata.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;


#nullable enable
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for <see cref="IEntityType" /> for Actian-specific metadata.
    /// </summary>
    public static class ActianEntityTypeExtensions
    {
        #region Locations

        private static readonly IEnumerable<string> NoLocations = Enumerable.Empty<string>();

        /// <summary>
        /// Returns the locations for the table the entity type maps to, if any
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns>The locations for the table the entity type maps to, if any</returns>
        public static IEnumerable<string> GetLocations([NotNull] this IEntityType entityType)
            => entityType[ActianAnnotationNames.Locations] as IEnumerable<string> ?? NoLocations;

        /// <summary>
        /// Sets the locations for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        public static void SetLocations(
            [NotNull] this IMutableEntityType entityType,
            [CanBeNull] IEnumerable<string> locations)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Locations, locations?.ToArray() ?? NoLocations);

        /// <summary>
        /// Sets the locations for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetLocations(
            [NotNull] this IConventionEntityType entityType,
            [CanBeNull] IEnumerable<string> locations,
            bool fromDataAnnotation = false)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Locations, locations?.ToArray() ?? NoLocations, fromDataAnnotation);

        /// <summary>
        /// Sets locations for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="locations"> The value to set. </param>
        public static void SetLocations([NotNull] this IMutableEntityType entityType, params string[] locations)
            => entityType.SetLocations(locations?.AsEnumerable()!);

        /// <summary>
        /// Gets the configuration source for the locations setting.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns>The configuration source for the locations setting</returns>
        public static ConfigurationSource? GetLocationsConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(ActianAnnotationNames.Locations)?.GetConfigurationSource();

        #endregion

        #region Journaling

        /// <summary>
        /// Returns a value indicating whether journaling is enabled for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns><c>true</c> if journaling is enabled for the table the entity type maps to</returns>
        public static bool IsJournalingEnabled([NotNull] this IEntityType entityType)
            => entityType[ActianAnnotationNames.Journaling] as bool? ?? false;

        /// <summary>
        /// Sets whether journaling is enabled for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        public static void SetJournaling([NotNull] this IMutableEntityType entityType, bool journalingEnabled)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Journaling, journalingEnabled);

        /// <summary>
        /// Sets whether journaling is enabled for the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetJournaling([NotNull] this IConventionEntityType entityType, bool journalingEnabled, bool fromDataAnnotation = false)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Journaling, journalingEnabled, fromDataAnnotation);

        /// <summary>
        /// Gets the configuration source for the journaling setting.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns>The configuration source for the journaling setting</returns>
        public static ConfigurationSource? GetJournalingConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(ActianAnnotationNames.Journaling)?.GetConfigurationSource();

        #endregion

        #region Duplicates

        /// <summary>
        /// Returns a value indicating whether duplicates are allowed in the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns><c>true</c> if duplicates are allowed in the table the entity type maps to</returns>
        public static bool AreDuplicatesAllowed([NotNull] this IEntityType entityType)
            => entityType[ActianAnnotationNames.Duplicates] as bool? ?? false;

        /// <summary>
        /// Sets whether duplicates are allowed in the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        public static void SetDuplicates([NotNull] this IMutableEntityType entityType, bool duplicates)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Duplicates, duplicates);

        /// <summary>
        /// Sets whether duplicates are allowed in the table the entity type maps to.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetDuplicates([NotNull] this IConventionEntityType entityType, bool duplicates, bool fromDataAnnotation = false)
            => entityType.SetOrRemoveAnnotation(ActianAnnotationNames.Duplicates, duplicates, fromDataAnnotation);

        /// <summary>
        /// Gets the configuration source for the duplicates setting.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns>The configuration source for the duplicates setting</returns>
        public static ConfigurationSource? GetDuplicatesConfigurationSource([NotNull] this IConventionEntityType entityType)
            => entityType.FindAnnotation(ActianAnnotationNames.Duplicates)?.GetConfigurationSource();

        #endregion
    }
}
