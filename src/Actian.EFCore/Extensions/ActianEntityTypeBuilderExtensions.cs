// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Actian specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class ActianEntityTypeBuilderExtensions
    {
        #region Locations

        /// <summary>
        /// Configures the table that the entity maps to to use the specified locations.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder WithLocations(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [CanBeNull] IEnumerable<string> locations)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetLocations(locations);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Configures the table that the entity maps to to use the specified locations.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder<TEntity> WithLocations<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [CanBeNull] IEnumerable<string> locations)
            where TEntity : class
        {
            return (EntityTypeBuilder<TEntity>)WithLocations((EntityTypeBuilder)entityTypeBuilder, locations);
        }

        /// <summary>
        /// Configures the table that the entity maps to to use the specified locations.
        /// </summary>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder WithLocations(
            [NotNull] this OwnedNavigationBuilder collectionOwnershipBuilder,
            [CanBeNull] IEnumerable<string> locations)
        {
            Check.NotNull(collectionOwnershipBuilder, nameof(collectionOwnershipBuilder));

            collectionOwnershipBuilder.OwnedEntityType.SetLocations(locations);

            return collectionOwnershipBuilder;
        }

        /// <summary>
        /// Configures the table that the entity maps to to use the specified locations.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <typeparam name="TRelatedEntity">The entity type that this relationship targets.</typeparam>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder<TEntity, TRelatedEntity> WithLocations<TEntity, TRelatedEntity>(
            [NotNull] this OwnedNavigationBuilder<TEntity, TRelatedEntity> collectionOwnershipBuilder,
            [CanBeNull] IEnumerable<string> locations)
            where TEntity : class
            where TRelatedEntity : class
        {
            return (OwnedNavigationBuilder<TEntity, TRelatedEntity>)WithLocations((OwnedNavigationBuilder)collectionOwnershipBuilder, locations);
        }

        /// <summary>
        /// Configures the table that the entity maps to to use the specified locations.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="locations">The locations for the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IConventionEntityTypeBuilder WithLocations(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            [CanBeNull] IEnumerable<string> locations,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetLocations(locations, fromDataAnnotation);

            return entityTypeBuilder;
        }

        #endregion

        #region Journaling

        /// <summary>
        /// Configures whether journaling is enabled for the table that the entity type maps to.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder WithJournaling(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            bool journalingEnabled = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetJournaling(journalingEnabled);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Configures whether journaling is enabled for the table that the entity type maps to.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder<TEntity> WithJournaling<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            bool journalingEnabled = true)
            where TEntity : class
        {
            return (EntityTypeBuilder<TEntity>)WithJournaling((EntityTypeBuilder)entityTypeBuilder, journalingEnabled);
        }

        /// <summary>
        /// Configures whether journaling is enabled for the table that the entity type maps to.
        /// </summary>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder WithJournaling(
            [NotNull] this OwnedNavigationBuilder collectionOwnershipBuilder,
            bool journalingEnabled = true)
        {
            Check.NotNull(collectionOwnershipBuilder, nameof(collectionOwnershipBuilder));

            collectionOwnershipBuilder.OwnedEntityType.SetJournaling(journalingEnabled);

            return collectionOwnershipBuilder;
        }

        /// <summary>
        /// Configures whether journaling is enabled for the table that the entity type maps to.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <typeparam name="TRelatedEntity">The entity type that this relationship targets.</typeparam>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder<TEntity, TRelatedEntity> WithJournaling<TEntity, TRelatedEntity>(
            [NotNull] this OwnedNavigationBuilder<TEntity, TRelatedEntity> collectionOwnershipBuilder,
            bool journalingEnabled = true)
            where TEntity : class
            where TRelatedEntity : class
        {
            return (OwnedNavigationBuilder<TEntity, TRelatedEntity>)WithJournaling((OwnedNavigationBuilder)collectionOwnershipBuilder, journalingEnabled);
        }

        /// <summary>
        /// Configures whether journaling is enabled for the table that the entity type maps to.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="journalingEnabled">A value indicating whether journaling is enabled for the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IConventionEntityTypeBuilder WithJournaling(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            bool journalingEnabled = true,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetJournaling(journalingEnabled, fromDataAnnotation);

            return entityTypeBuilder;
        }

        #endregion

        #region Duplicates

        /// <summary>
        /// Configures whether duplicates are allowed in the table that the entity type maps to.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder WithDuplicates(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            bool duplicates = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetDuplicates(duplicates);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Configures whether duplicates are allowed in the table that the entity type maps to.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static EntityTypeBuilder<TEntity> WithDuplicates<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            bool duplicates = true)
            where TEntity : class
        {
            return (EntityTypeBuilder<TEntity>)WithDuplicates((EntityTypeBuilder)entityTypeBuilder, duplicates);
        }

        /// <summary>
        /// Configures whether duplicates are allowed in the table that the entity type maps to.
        /// </summary>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder WithDuplicates(
            [NotNull] this OwnedNavigationBuilder collectionOwnershipBuilder,
            bool duplicates = true)
        {
            Check.NotNull(collectionOwnershipBuilder, nameof(collectionOwnershipBuilder));

            collectionOwnershipBuilder.OwnedEntityType.SetDuplicates(duplicates);

            return collectionOwnershipBuilder;
        }

        /// <summary>
        /// Configures whether duplicates are allowed in the table that the entity type maps to.
        /// </summary>
        /// <typeparam name="TEntity">The entity type being configured.</typeparam>
        /// <typeparam name="TRelatedEntity">The entity type that this relationship targets.</typeparam>
        /// <param name="collectionOwnershipBuilder">The builder for the entity type being configured.</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static OwnedNavigationBuilder<TEntity, TRelatedEntity> WithDuplicates<TEntity, TRelatedEntity>(
            [NotNull] this OwnedNavigationBuilder<TEntity, TRelatedEntity> collectionOwnershipBuilder,
            bool duplicates = true)
            where TEntity : class
            where TRelatedEntity : class
        {
            return (OwnedNavigationBuilder<TEntity, TRelatedEntity>)WithDuplicates((OwnedNavigationBuilder)collectionOwnershipBuilder, duplicates);
        }

        /// <summary>
        /// Configures whether duplicates are allowed in the table that the entity type maps to.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
        /// <param name="duplicates">A value indicating whether duplicates are allowed in the table the entity type maps to</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static IConventionEntityTypeBuilder WithDuplicates(
            [NotNull] this IConventionEntityTypeBuilder entityTypeBuilder,
            bool duplicates = true,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetDuplicates(duplicates, fromDataAnnotation);

            return entityTypeBuilder;
        }

        #endregion
    }
}
