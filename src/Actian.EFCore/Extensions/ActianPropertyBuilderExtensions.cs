// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
#nullable enable
    /// <summary>
    /// Actian specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class ActianPropertyBuilderExtensions
    {
        #region HiLo

        /// <summary>
        ///     Configures the key property to use a sequence-based hi-lo pattern to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseHiLo(
            this PropertyBuilder propertyBuilder,
            string? name = null,
            string? schema = null)
        {
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var property = propertyBuilder.Metadata;

            name ??= ActianModelExtensions.DefaultHiLoSequenceName;

            var model = property.DeclaringType.Model;

            if (model.FindSequence(name, schema) == null)
            {
                model.AddSequence(name, schema).IncrementBy = 10;
            }

            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.SequenceHiLo);
            property.SetHiLoSequenceName(name);
            property.SetHiLoSequenceSchema(schema);
            property.SetIdentitySeed(null);
            property.SetIdentityIncrement(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the database sequence used for the hi-lo pattern to generate values for the key property.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>A builder to further configure the sequence.</returns>
        public static IConventionSequenceBuilder? HasHiLoSequence(
            this IConventionPropertyBuilder propertyBuilder,
            string? name,
            string? schema,
            bool fromDataAnnotation = false)
        {
            if (!propertyBuilder.CanSetHiLoSequence(name, schema, fromDataAnnotation))
            {
                return null;
            }

            propertyBuilder.Metadata.SetHiLoSequenceName(name, fromDataAnnotation);
            propertyBuilder.Metadata.SetHiLoSequenceSchema(schema, fromDataAnnotation);

            return name == null
                ? null
                : propertyBuilder.Metadata.DeclaringType.Model.Builder.HasSequence(name, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns a value indicating whether the given name and schema can be set for the hi-lo sequence.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given name and schema can be set for the hi-lo sequence.</returns>
        public static bool CanSetHiLoSequence(
            this IConventionPropertyBuilder propertyBuilder,
            string? name,
            string? schema,
            bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            return propertyBuilder.CanSetAnnotation(ActianAnnotationNames.HiLoSequenceName, name, fromDataAnnotation)
                && propertyBuilder.CanSetAnnotation(ActianAnnotationNames.HiLoSequenceSchema, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the key property to use a sequence-based key value generation pattern to generate values for new entities,
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseSequence(
            this PropertyBuilder propertyBuilder,
            string? name = null,
            string? schema = null)
        {
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var property = propertyBuilder.Metadata;

            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.Sequence);
            property.SetSequenceName(name);
            property.SetSequenceSchema(schema);
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);
            property.SetIdentitySeed(null);
            property.SetIdentityIncrement(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use a sequence-based key value generation pattern to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> UseSequence<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            string? name = null,
            string? schema = null)
            => (PropertyBuilder<TProperty>)UseSequence((PropertyBuilder)propertyBuilder, name, schema);

        /// <summary>
        ///     Configures the database sequence used for the key value generation pattern to generate values for the key property.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>A builder to further configure the sequence.</returns>
        public static IConventionSequenceBuilder? HasSequence(
            this IConventionPropertyBuilder propertyBuilder,
            string? name,
            string? schema,
            bool fromDataAnnotation = false)
        {
            if (!propertyBuilder.CanSetSequence(name, schema, fromDataAnnotation))
            {
                return null;
            }

            propertyBuilder.Metadata.SetSequenceName(name, fromDataAnnotation);
            propertyBuilder.Metadata.SetSequenceSchema(schema, fromDataAnnotation);

            return name == null
                ? null
                : propertyBuilder.Metadata.DeclaringType.Model.Builder.HasSequence(name, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Returns a value indicating whether the given name and schema can be set for the key value generation sequence.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="name">The name of the sequence.</param>
        /// <param name="schema">The schema of the sequence.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given name and schema can be set for the key value generation sequence.</returns>
        public static bool CanSetSequence(
            this IConventionPropertyBuilder propertyBuilder,
            string? name,
            string? schema,
            bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            return propertyBuilder.CanSetAnnotation(ActianAnnotationNames.SequenceName, name, fromDataAnnotation)
                && propertyBuilder.CanSetAnnotation(ActianAnnotationNames.SequenceSchema, schema, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseIdentityColumn(
            this PropertyBuilder propertyBuilder,
            long seed = 1,
            int increment = 1)
        {
            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityColumn);
            property.SetIdentitySeed(seed);
            property.SetIdentityIncrement(increment);
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);
            property.SetSequenceName(null);
            property.SetSequenceSchema(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseIdentityByDefaultColumn(
            this PropertyBuilder propertyBuilder,
            long seed = 1,
            int increment = 1)
        {
            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
            property.SetIdentitySeed(seed);
            property.SetIdentityIncrement(increment);
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);
            property.SetSequenceName(null);
            property.SetSequenceSchema(null);

            return propertyBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseIdentityColumn(
            this PropertyBuilder propertyBuilder,
            int seed,
            int increment = 1)
            => propertyBuilder.UseIdentityColumn((long)seed, increment);

        /// <summary>
        ///     Configures the key column to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="columnBuilder">The builder for the column being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ColumnBuilder UseIdentityColumn(
            this ColumnBuilder columnBuilder,
            long seed = 1,
            int increment = 1)
        {
            var overrides = columnBuilder.Overrides;
            overrides.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityColumn);
            overrides.SetIdentitySeed(seed);
            overrides.SetIdentityIncrement(increment);

            return columnBuilder;
        }

        /// <summary>
        ///     Configures the key column to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="columnBuilder">The builder for the column being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ColumnBuilder UseIdentityByDefaultColumn(
            this ColumnBuilder columnBuilder,
            long seed = 1,
            int increment = 1)
        {
            var overrides = columnBuilder.Overrides;
            overrides.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
            overrides.SetIdentitySeed(seed);
            overrides.SetIdentityIncrement(increment);

            return columnBuilder;
        }

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            long seed = 1,
            int increment = 1)
            => (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder, seed, increment);
/*
        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> UseIdentityByDefaultColumn<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            long seed = 1,
            int increment = 1)
            => (PropertyBuilder<TProperty>)UseIdentityByDefaultColumn((PropertyBuilder)propertyBuilder, seed, increment);
*/
        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        ///     for more information and examples.
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            int seed,
            int increment = 1)
            => (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder, (long)seed, increment);

        /// <summary>
        ///     Configures the key column to use the Actian IDENTITY feature to generate values for new entities.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="columnBuilder">The builder for the column being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ColumnBuilder<TProperty> UseIdentityColumn<TProperty>(
            this ColumnBuilder<TProperty> columnBuilder,
            long seed = 1,
            int increment = 1)
            => (ColumnBuilder<TProperty>)UseIdentityColumn((ColumnBuilder)columnBuilder, seed, increment);

        /// <summary>
        ///     Configures the seed for Actian IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasIdentityColumnSeed(
            this IConventionPropertyBuilder propertyBuilder,
            long? seed,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetIdentityColumnSeed(seed, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetIdentitySeed(seed, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the seed for Actian IDENTITY for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasIdentityColumnSeed(
            this IConventionPropertyBuilder propertyBuilder,
            long? seed,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetIdentityColumnSeed(seed, storeObject, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetIdentitySeed(seed, storeObject, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the seed for Actian IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the seed for Actian IDENTITY.</returns>
        public static bool CanSetIdentityColumnSeed(
            this IConventionPropertyBuilder propertyBuilder,
            long? seed,
            bool fromDataAnnotation = false)
            => propertyBuilder.CanSetAnnotation(ActianAnnotationNames.IdentitySeed, seed, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the seed for Actian IDENTITY
        ///     for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the seed for Actian IDENTITY.</returns>
        public static bool CanSetIdentityColumnSeed(
            this IConventionPropertyBuilder propertyBuilder,
            long? seed,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => propertyBuilder.Metadata.FindOverrides(storeObject)?.Builder
                    .CanSetAnnotation(
                        ActianAnnotationNames.IdentitySeed,
                        seed,
                        fromDataAnnotation)
                ?? true;

        /// <summary>
        ///     Configures the increment for Actian IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasIdentityColumnIncrement(
            this IConventionPropertyBuilder propertyBuilder,
            int? increment,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetIdentityColumnIncrement(increment, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetIdentityIncrement(increment, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the increment for Actian IDENTITY for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasIdentityColumnIncrement(
            this IConventionPropertyBuilder propertyBuilder,
            int? increment,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetIdentityColumnIncrement(increment, storeObject, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetIdentityIncrement(increment, storeObject, fromDataAnnotation);
                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the increment for Actian IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default increment for Actian IDENTITY.</returns>
        public static bool CanSetIdentityColumnIncrement(
            this IConventionPropertyBuilder propertyBuilder,
            int? increment,
            bool fromDataAnnotation = false)
            => propertyBuilder.CanSetAnnotation(ActianAnnotationNames.IdentityIncrement, increment, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the increment for Actian IDENTITY
        ///     for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default increment for Actian IDENTITY.</returns>
        public static bool CanSetIdentityColumnIncrement(
            this IConventionPropertyBuilder propertyBuilder,
            int? increment,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => propertyBuilder.Metadata.FindOverrides(storeObject)?.Builder
                    .CanSetAnnotation(
                        ActianAnnotationNames.IdentityIncrement,
                        increment,
                        fromDataAnnotation)
                ?? true;


        #endregion HiLo

        #region Identity always

        public static PropertyBuilder UseIdentityAlwaysColumn([NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityAlwaysColumn);
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> UseIdentityAlwaysColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseIdentityAlwaysColumn((PropertyBuilder)propertyBuilder);

        public static IConventionPropertyBuilder? UseIdentityAlwaysColumn([NotNull] this IConventionPropertyBuilder propertyBuilder)
        {
            if (propertyBuilder.CanSetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityAlwaysColumn))
            {
                var property = propertyBuilder.Metadata;
                property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityAlwaysColumn);
                property.SetHiLoSequenceName(null);
                property.SetHiLoSequenceSchema(null);

                return propertyBuilder;
            }

            return null;
        }

        #endregion Identity always

        #region Identity by default

        public static PropertyBuilder UseIdentityByDefaultColumn([NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var property = propertyBuilder.Metadata;
            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> UseIdentityByDefaultColumn<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseIdentityByDefaultColumn((PropertyBuilder)propertyBuilder);

        public static IConventionPropertyBuilder? UseIdentityByDefaultColumn([NotNull] this IConventionPropertyBuilder propertyBuilder)
        {
            if (propertyBuilder.CanSetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn))
            {
                var property = propertyBuilder.Metadata;
                property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
                property.SetHiLoSequenceName(null);
                property.SetHiLoSequenceSchema(null);

                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities,
        ///     when targeting Actian. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder UseIdentityByDefaultColumn(
            this PropertyBuilder propertyBuilder,
            int seed,
            int increment = 1)
            => propertyBuilder.UseIdentityByDefaultColumn((long)seed, increment);

        /// <summary>
        ///     Configures the key property to use the Actian IDENTITY feature to generate values for new entities,
        ///     when targeting Actian. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        ///     for more information and examples.
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static PropertyBuilder<TProperty> UseIdentityByDefaultColumn<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            int seed,
            int increment = 1)
            => (PropertyBuilder<TProperty>)UseIdentityByDefaultColumn((PropertyBuilder)propertyBuilder, (long)seed, increment);

        /// <summary>
        ///     Configures the key column to use the Actian IDENTITY feature to generate values for new entities,
        ///     when targeting Actian. This method sets the property to be <see cref="ValueGenerated.OnAdd" />.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        ///     for more information and examples.
        /// </remarks>
        /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
        /// <param name="columnBuilder">The builder for the column being configured.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ColumnBuilder<TProperty> UseIdentityByDefaultColumn<TProperty>(
            this ColumnBuilder<TProperty> columnBuilder,
            long seed = 1,
            int increment = 1)
            => (ColumnBuilder<TProperty>)UseIdentityByDefaultColumn((ColumnBuilder)columnBuilder, seed, increment);

        #endregion Identity by default

        #region Value Generation Strategy

        /// <summary>
        ///     Configures the value generation strategy for the key property, when targeting Actian.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="valueGenerationStrategy">The value generation strategy.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasValueGenerationStrategy(
            this IConventionPropertyBuilder propertyBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetAnnotation(
                    ActianAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
                if (valueGenerationStrategy != ActianValueGenerationStrategy.IdentityColumn &&
                    valueGenerationStrategy != ActianValueGenerationStrategy.IdentityByDefaultColumn)
                {
                    propertyBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                    propertyBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
                    propertyBuilder.HasSequence(null, null, fromDataAnnotation);
                }

                if (valueGenerationStrategy != ActianValueGenerationStrategy.SequenceHiLo)
                {
                    propertyBuilder.HasHiLoSequence(null, null, fromDataAnnotation);
                    propertyBuilder.HasSequence(null, null, fromDataAnnotation);
                }

                if (valueGenerationStrategy != ActianValueGenerationStrategy.Sequence)
                {
                    propertyBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                    propertyBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
                    propertyBuilder.HasHiLoSequence(null, null, fromDataAnnotation);
                }

                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the value generation strategy for the key property, when targeting Actian for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="valueGenerationStrategy">The value generation strategy.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionPropertyBuilder? HasValueGenerationStrategy(
            this IConventionPropertyBuilder propertyBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
        {
            if (propertyBuilder.CanSetValueGenerationStrategy(valueGenerationStrategy, storeObject, fromDataAnnotation))
            {
                propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, storeObject, fromDataAnnotation);
                if (valueGenerationStrategy != ActianValueGenerationStrategy.IdentityColumn &&
                    valueGenerationStrategy != ActianValueGenerationStrategy.IdentityByDefaultColumn)
                {
                    propertyBuilder.HasIdentityColumnSeed(null, storeObject, fromDataAnnotation);
                    propertyBuilder.HasIdentityColumnIncrement(null, storeObject, fromDataAnnotation);
                }

                return propertyBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the value generation strategy.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="valueGenerationStrategy">The value generation strategy.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default value generation strategy.</returns>
        public static bool CanSetValueGenerationStrategy(
            this IConventionPropertyBuilder propertyBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
            => propertyBuilder.CanSetAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the value generation strategy for a particular table.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>.
        /// </remarks>
        /// <param name="propertyBuilder">The builder for the property being configured.</param>
        /// <param name="valueGenerationStrategy">The value generation strategy.</param>
        /// <param name="storeObject">The table identifier.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default value generation strategy.</returns>
        public static bool CanSetValueGenerationStrategy(
            this IConventionPropertyBuilder propertyBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => propertyBuilder.Metadata.FindOverrides(storeObject)?.Builder
                    .CanSetAnnotation(
                        ActianAnnotationNames.ValueGenerationStrategy,
                        valueGenerationStrategy,
                        fromDataAnnotation)
                ?? true;

        #endregion Value Generation Strategy

        #region Identity options

        public static PropertyBuilder HasIdentityOptions(
            [NotNull] this PropertyBuilder propertyBuilder,
            long? startValue = null,
            long? incrementBy = null,
            long? minValue = null,
            long? maxValue = null,
            bool? isCyclic = null,
            long? numbersToCache = null)
        {
            var property = propertyBuilder.Metadata;
            property.SetIdentityStartValue(startValue);
            property.SetIdentityIncrementBy(incrementBy);
            property.SetIdentityMinValue(minValue);
            property.SetIdentityMaxValue(maxValue);
            property.SetIdentityIsCyclic(isCyclic);
            property.SetIdentityNumbersToCache(numbersToCache);
            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> HasIdentityOptions<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            long? startValue = null,
            long? incrementBy = null,
            long? minValue = null,
            long? maxValue = null,
            bool? isCyclic = null,
            long? numbersToCache = null)
            => (PropertyBuilder<TProperty>)HasIdentityOptions(
                (PropertyBuilder)propertyBuilder, startValue, incrementBy, minValue, maxValue, isCyclic, numbersToCache);

        public static IConventionPropertyBuilder? HasIdentityOptions(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            long? startValue = null,
            long? incrementBy = null,
            long? minValue = null,
            long? maxValue = null,
            bool? isCyclic = null,
            long? numbersToCache = null)
        {
            if (propertyBuilder.CanSetIdentityOptions(startValue, incrementBy, minValue, maxValue, isCyclic, numbersToCache))
            {
                var property = propertyBuilder.Metadata;
                property.SetIdentityStartValue(startValue);
                property.SetIdentityIncrementBy(incrementBy);
                property.SetIdentityMinValue(minValue);
                property.SetIdentityMaxValue(maxValue);
                property.SetIdentityIsCyclic(isCyclic);
                property.SetIdentityNumbersToCache(numbersToCache);
                return propertyBuilder;
            }

            return null;
        }

        public static bool CanSetIdentityOptions(
            [NotNull] this IConventionPropertyBuilder propertyBuilder,
            long? startValue = null,
            long? incrementBy = null,
            long? minValue = null,
            long? maxValue = null,
            bool? isCyclic = null,
            long? numbersToCache = null)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var value = new IdentitySequenceOptionsData
            {
                StartValue = startValue,
                IncrementBy = incrementBy ?? 1,
                MinValue = minValue,
                MaxValue = maxValue,
                IsCyclic = isCyclic ?? false,
                NumbersToCache = numbersToCache ?? 1
            }.Serialize();

            return propertyBuilder.CanSetAnnotation(ActianAnnotationNames.IdentityOptions, value);
        }

        #endregion Identity options
    }
}
