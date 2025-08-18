// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore
{
    /// <summary>
    /// Extension methods for <see cref="IProperty" /> for Actian-specific metadata.
    /// </summary>
    public static class ActianPropertyExtensions
    {
#nullable enable
        #region HiLo

        /// <summary>
        /// Returns the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property"> The property.</param>
        /// <returns>The name to use for the hi-lo sequence.</returns>
        public static string? GetHiLoSequenceName(this IReadOnlyProperty property)
            => (string?)property[ActianAnnotationNames.HiLoSequenceName];

        /// <summary>
        ///     Returns the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The name to use for the hi-lo sequence.</returns>
        public static string? GetHiLoSequenceName(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var annotation = property.FindAnnotation(ActianAnnotationNames.HiLoSequenceName);
            if (annotation != null)
            {
                return (string?)annotation.Value;
            }

            return property.FindSharedStoreObjectRootProperty(storeObject)?.GetHiLoSequenceName(storeObject);
        }

        /// <summary>
        ///     Sets the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The sequence name to use.</param>
        public static void SetHiLoSequenceName(this IMutableProperty property, string? name)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(name, nameof(name)));

        /// <summary>
        ///     Sets the name to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The sequence name to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static string? SetHiLoSequenceName(
            this IConventionProperty property,
            string? name,
            bool fromDataAnnotation = false)
            => (string?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(name, nameof(name)),
                fromDataAnnotation)?.Value;

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the hi-lo sequence name.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the hi-lo sequence name.</returns>
        public static ConfigurationSource? GetHiLoSequenceNameConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();

        /// <summary>
        /// Returns the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The schema to use for the hi-lo sequence.</returns>
        public static string? GetHiLoSequenceSchema(this IReadOnlyProperty property)
            => (string?)property[ActianAnnotationNames.HiLoSequenceSchema];

        /// <summary>
        ///     Returns the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The schema to use for the hi-lo sequence.</returns>
        public static string? GetHiLoSequenceSchema(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var annotation = property.FindAnnotation(ActianAnnotationNames.HiLoSequenceSchema);
            if (annotation != null)
            {
                return (string?)annotation.Value;
            }

            return property.FindSharedStoreObjectRootProperty(storeObject)?.GetHiLoSequenceSchema(storeObject);
        }

        /// <summary>
        ///     Sets the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="schema">The schema to use.</param>
        public static void SetHiLoSequenceSchema(this IMutableProperty property, string? schema)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)));

        /// <summary>
        /// Sets the schema to use for the hi-lo sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="schema">The schema to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static string? SetHiLoSequenceSchema(
            this IConventionProperty property,
            string? schema,
            bool fromDataAnnotation = false)
            => (string?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)),
                fromDataAnnotation)?.Value;

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the hi-lo sequence schema.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the hi-lo sequence schema.</returns>
        public static ConfigurationSource? GetHiLoSequenceSchemaConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the hi-lo pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static IReadOnlySequence? FindHiLoSequence(this IReadOnlyProperty property)
        {
            var model = property.DeclaringType.Model;

            var sequenceName = property.GetHiLoSequenceName()
                ?? model.GetHiLoSequenceName();

            var sequenceSchema = property.GetHiLoSequenceSchema()
                ?? model.GetHiLoSequenceSchema();

            return model.FindSequence(sequenceName, sequenceSchema);
        }

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the hi-lo pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static IReadOnlySequence? FindHiLoSequence(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var model = property.DeclaringType.Model;

            var sequenceName = property.GetHiLoSequenceName(storeObject)
                ?? model.GetHiLoSequenceName();

            var sequenceSchema = property.GetHiLoSequenceSchema(storeObject)
                ?? model.GetHiLoSequenceSchema();

            return model.FindSequence(sequenceName, sequenceSchema);
        }

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the hi-lo pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static ISequence? FindHiLoSequence(this IProperty property)
            => (ISequence?)((IReadOnlyProperty)property).FindHiLoSequence();

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the hi-lo pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static ISequence? FindHiLoSequence(this IProperty property, in StoreObjectIdentifier storeObject)
            => (ISequence?)((IReadOnlyProperty)property).FindHiLoSequence(storeObject);

        /// <summary>
        ///     Returns the name to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The name to use for the key value generation sequence.</returns>
        public static string? GetSequenceName(this IReadOnlyProperty property)
            => (string?)property[ActianAnnotationNames.SequenceName];

        /// <summary>
        ///     Returns the name to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The name to use for the key value generation sequence.</returns>
        public static string? GetSequenceName(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var annotation = property.FindAnnotation(ActianAnnotationNames.SequenceName);
            if (annotation != null)
            {
                return (string?)annotation.Value;
            }

            return property.FindSharedStoreObjectRootProperty(storeObject)?.GetSequenceName(storeObject);
        }

        /// <summary>
        ///     Sets the name to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The sequence name to use.</param>
        public static void SetSequenceName(this IMutableProperty property, string? name)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceName,
                Check.NullButNotEmpty(name, nameof(name)));

        /// <summary>
        ///     Sets the name to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The sequence name to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static string? SetSequenceName(
            this IConventionProperty property,
            string? name,
            bool fromDataAnnotation = false)
            => (string?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceName,
                Check.NullButNotEmpty(name, nameof(name)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the key value generation sequence name.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the key value generation sequence name.</returns>
        public static ConfigurationSource? GetSequenceNameConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.SequenceName)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the schema to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The schema to use for the key value generation sequence.</returns>
        public static string? GetSequenceSchema(this IReadOnlyProperty property)
            => (string?)property[ActianAnnotationNames.SequenceSchema];

        /// <summary>
        ///     Returns the schema to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The schema to use for the key value generation sequence.</returns>
        public static string? GetSequenceSchema(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var annotation = property.FindAnnotation(ActianAnnotationNames.SequenceSchema);
            if (annotation != null)
            {
                return (string?)annotation.Value;
            }

            return property.FindSharedStoreObjectRootProperty(storeObject)?.GetSequenceSchema(storeObject);
        }

        /// <summary>
        ///     Sets the schema to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="schema">The schema to use.</param>
        public static void SetSequenceSchema(this IMutableProperty property, string? schema)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)));

        /// <summary>
        ///     Sets the schema to use for the key value generation sequence.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="schema">The schema to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static string? SetSequenceSchema(
            this IConventionProperty property,
            string? schema,
            bool fromDataAnnotation = false)
            => (string?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceSchema,
                Check.NullButNotEmpty(schema, nameof(schema)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the key value generation sequence schema.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the key value generation sequence schema.</returns>
        public static ConfigurationSource? GetSequenceSchemaConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.SequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the key value generation pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static IReadOnlySequence? FindSequence(this IReadOnlyProperty property)
        {
            var model = property.DeclaringType.Model;

            var sequenceName = property.GetSequenceName()
                ?? model.GetSequenceNameSuffix();

            var sequenceSchema = property.GetSequenceSchema()
                ?? model.GetSequenceSchema();

            return model.FindSequence(sequenceName, sequenceSchema);
        }

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the key value generation pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static IReadOnlySequence? FindSequence(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            var model = property.DeclaringType.Model;

            var sequenceName = property.GetSequenceName(storeObject)
                ?? model.GetSequenceNameSuffix();

            var sequenceSchema = property.GetSequenceSchema(storeObject)
                ?? model.GetSequenceSchema();

            return model.FindSequence(sequenceName, sequenceSchema);
        }

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the key value generation pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static ISequence? FindSequence(this IProperty property)
            => (ISequence?)((IReadOnlyProperty)property).FindSequence();

        /// <summary>
        ///     Finds the <see cref="ISequence" /> in the model to use for the key value generation pattern.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The sequence to use, or <see langword="null" /> if no sequence exists in the model.</returns>
        public static ISequence? FindSequence(this IProperty property, in StoreObjectIdentifier storeObject)
            => (ISequence?)((IReadOnlyProperty)property).FindSequence(storeObject);

        /// <summary>
        ///     Returns the identity seed.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity seed.</returns>
        public static long? GetIdentitySeed(this IReadOnlyProperty property)
        {
            if (property is RuntimeProperty)
            {
                throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData);
            }

            // Support pre-6.0 IdentitySeed annotations, which contained an int rather than a long
            var annotation = property.FindAnnotation(ActianAnnotationNames.IdentitySeed);
            return annotation is null
                ? null
                : annotation.Value is int intValue
                    ? intValue
                    : (long?)annotation.Value;
        }

        /// <summary>
        ///     Returns the identity seed.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The identity seed.</returns>
        public static long? GetIdentitySeed(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            if (property is RuntimeProperty)
            {
                throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData);
            }

            var @override = property.FindOverrides(storeObject)?.FindAnnotation(ActianAnnotationNames.IdentitySeed);
            if (@override != null)
            {
                return (long?)@override.Value;
            }

            var annotation = property.FindAnnotation(ActianAnnotationNames.IdentitySeed);
            if (annotation is not null)
            {
                // Support pre-6.0 IdentitySeed annotations, which contained an int rather than a long
                return annotation.Value is int intValue
                    ? intValue
                    : (long?)annotation.Value;
            }

            var sharedProperty = property.FindSharedStoreObjectRootProperty(storeObject);
            return sharedProperty == null
                ? property.DeclaringType.Model.GetIdentitySeed()
                : sharedProperty.GetIdentitySeed(storeObject);
        }

        /// <summary>
        ///     Returns the identity seed.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The identity seed.</returns>
        public static long? GetIdentitySeed(this IReadOnlyRelationalPropertyOverrides overrides)
            => overrides is RuntimeRelationalPropertyOverrides
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (long?)overrides.FindAnnotation(ActianAnnotationNames.IdentitySeed)?.Value;

        /// <summary>
        ///     Sets the identity seed.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="seed">The value to set.</param>
        public static void SetIdentitySeed(this IMutableProperty property, long? seed)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentitySeed,
                seed);

        /// <summary>
        ///     Sets the identity seed.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="seed">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static long? SetIdentitySeed(
            this IConventionProperty property,
            long? seed,
            bool fromDataAnnotation = false)
            => (long?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentitySeed,
                seed,
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Sets the identity seed for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="seed">The value to set.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        public static void SetIdentitySeed(
            this IMutableProperty property,
            long? seed,
            in StoreObjectIdentifier storeObject)
            => property.GetOrCreateOverrides(storeObject)
                .SetIdentitySeed(seed);

        /// <summary>
        ///     Sets the identity seed for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="seed">The value to set.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static long? SetIdentitySeed(
            this IConventionProperty property,
            long? seed,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => property.GetOrCreateOverrides(storeObject, fromDataAnnotation)
                .SetIdentitySeed(seed, fromDataAnnotation);

        /// <summary>
        ///     Sets the identity seed for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="seed">The value to set.</param>
        public static void SetIdentitySeed(this IMutableRelationalPropertyOverrides overrides, long? seed)
            => overrides.SetOrRemoveAnnotation(ActianAnnotationNames.IdentitySeed, seed);

        /// <summary>
        ///     Sets the identity seed for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="seed">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static long? SetIdentitySeed(
            this IConventionRelationalPropertyOverrides overrides,
            long? seed,
            bool fromDataAnnotation = false)
            => (long?)overrides.SetOrRemoveAnnotation(ActianAnnotationNames.IdentitySeed, seed, fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity seed.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity seed.</returns>
        public static ConfigurationSource? GetIdentitySeedConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.IdentitySeed)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity seed for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity seed.</returns>
        public static ConfigurationSource? GetIdentitySeedConfigurationSource(
            this IConventionProperty property,
            in StoreObjectIdentifier storeObject)
            => property.FindOverrides(storeObject)?.GetIdentitySeedConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity seed for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity seed.</returns>
        public static ConfigurationSource? GetIdentitySeedConfigurationSource(
            this IConventionRelationalPropertyOverrides overrides)
            => overrides.FindAnnotation(ActianAnnotationNames.IdentitySeed)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the identity increment.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity increment.</returns>
        public static int? GetIdentityIncrement(this IReadOnlyProperty property)
            => (property is RuntimeProperty)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (int?)property[ActianAnnotationNames.IdentityIncrement]
                ?? property.DeclaringType.Model.GetIdentityIncrement();

        /// <summary>
        ///     Returns the identity increment.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The identity increment.</returns>
        public static int? GetIdentityIncrement(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        {
            if (property is RuntimeProperty)
            {
                throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData);
            }

            var @override = property.FindOverrides(storeObject)?.FindAnnotation(ActianAnnotationNames.IdentityIncrement);
            if (@override != null)
            {
                return (int?)@override.Value;
            }

            var annotation = property.FindAnnotation(ActianAnnotationNames.IdentityIncrement);
            if (annotation != null)
            {
                return (int?)annotation.Value;
            }

            var sharedProperty = property.FindSharedStoreObjectRootProperty(storeObject);
            return sharedProperty == null
                ? property.DeclaringType.Model.GetIdentityIncrement()
                : sharedProperty.GetIdentityIncrement(storeObject);
        }

        /// <summary>
        ///     Returns the identity increment.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The identity increment.</returns>
        public static int? GetIdentityIncrement(this IReadOnlyRelationalPropertyOverrides overrides)
            => overrides is RuntimeRelationalPropertyOverrides
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (int?)overrides.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.Value;

        /// <summary>
        ///     Sets the identity increment.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="increment">The value to set.</param>
        public static void SetIdentityIncrement(this IMutableProperty property, int? increment)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentityIncrement,
                increment);

        /// <summary>
        ///     Sets the identity increment.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="increment">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static int? SetIdentityIncrement(
            this IConventionProperty property,
            int? increment,
            bool fromDataAnnotation = false)
            => (int?)property.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentityIncrement,
                increment,
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Sets the identity increment for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="increment">The value to set.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        public static void SetIdentityIncrement(
            this IMutableProperty property,
            int? increment,
            in StoreObjectIdentifier storeObject)
            => property.GetOrCreateOverrides(storeObject)
                .SetIdentityIncrement(increment);

        /// <summary>
        ///     Sets the identity increment for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="increment">The value to set.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static int? SetIdentityIncrement(
            this IConventionProperty property,
            int? increment,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => property.GetOrCreateOverrides(storeObject, fromDataAnnotation)
                .SetIdentityIncrement(increment, fromDataAnnotation);

        /// <summary>
        ///     Sets the identity increment for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="increment">The value to set.</param>
        public static void SetIdentityIncrement(this IMutableRelationalPropertyOverrides overrides, int? increment)
            => overrides.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityIncrement, increment);

        /// <summary>
        ///     Sets the identity increment for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="increment">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static int? SetIdentityIncrement(
            this IConventionRelationalPropertyOverrides overrides,
            int? increment,
            bool fromDataAnnotation = false)
            => (int?)overrides.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityIncrement, increment, fromDataAnnotation)
                ?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity increment.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity increment.</returns>
        public static ConfigurationSource? GetIdentityIncrementConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity increment for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity increment.</returns>
        public static ConfigurationSource? GetIdentityIncrementConfigurationSource(
            this IConventionProperty property,
            in StoreObjectIdentifier storeObject)
            => property.FindOverrides(storeObject)?.GetIdentityIncrementConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the identity increment for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity increment.</returns>
        public static ConfigurationSource? GetIdentityIncrementConfigurationSource(
            this IConventionRelationalPropertyOverrides overrides)
            => overrides.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.GetConfigurationSource();

        /// <summary>
        /// Removes all identity sequence annotations from the property.
        /// </summary>
        public static void RemoveHiLoOptions([NotNull] this IMutableProperty property)
        {
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);
        }

        /// <summary>
        /// Removes all identity sequence annotations from the property.
        /// </summary>
        public static void RemoveHiLoOptions([NotNull] this IConventionProperty property)
        {
            property.SetHiLoSequenceName(null);
            property.SetHiLoSequenceSchema(null);
        }

        #endregion HiLo

        #region Value Generation Strategy

        /// <summary>
        ///     Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="value">The strategy to use.</param>
        public static void SetValueGenerationStrategy(
            this IMutableRelationalPropertyOverrides overrides,
            ActianValueGenerationStrategy? value)
            => overrides.SetOrRemoveAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy(overrides.Property, value));

        /// <summary>
        /// Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        public static void SetValueGenerationStrategy(
            this IMutableProperty property,
            ActianValueGenerationStrategy? value)
            => property.SetOrRemoveAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy((IProperty)property, value));

        /// <summary>
        ///     Returns the <see cref="ActianValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <remarks>
        ///     If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        /// </remarks>
        /// <param name="property">The property.</param>
        /// <returns>The strategy, or <see cref="ActianValueGenerationStrategy.None" /> if none was set.</returns>
        public static ActianValueGenerationStrategy GetValueGenerationStrategy(this IReadOnlyProperty property)
        {
            var annotation = property.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy);
            if (annotation != null)
            {
                return (ActianValueGenerationStrategy?)annotation.Value ?? ActianValueGenerationStrategy.None;
            }

            var defaultValueGenerationStrategy = GetDefaultValueGenerationStrategy(property);

            if (property.ValueGenerated != ValueGenerated.OnAdd
                || property.IsForeignKey()
                || property.TryGetDefaultValue(out _)
                || (defaultValueGenerationStrategy != ActianValueGenerationStrategy.Sequence && property.GetDefaultValueSql() != null)
                || property.GetComputedColumnSql() != null)
            {
                return ActianValueGenerationStrategy.None;
            }

            return defaultValueGenerationStrategy;
        }

        /// <summary>
        ///     Returns the <see cref="ActianValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <remarks>
        ///     If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        /// </remarks>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The strategy, or <see cref="ActianValueGenerationStrategy.None" /> if none was set.</returns>
        public static ActianValueGenerationStrategy GetValueGenerationStrategy(
            this IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject)
            => GetValueGenerationStrategy(property, storeObject, null);

        internal static ActianValueGenerationStrategy GetValueGenerationStrategy(
            this IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            ITypeMappingSource? typeMappingSource)
        {
            var @override = property.FindOverrides(storeObject)?.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy);
            if (@override != null)
            {
                return (ActianValueGenerationStrategy?)@override.Value ?? ActianValueGenerationStrategy.None;
            }

            var annotation = property.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy);
            if (annotation?.Value != null
                && StoreObjectIdentifier.Create(property.DeclaringType, storeObject.StoreObjectType) == storeObject)
            {
                return (ActianValueGenerationStrategy)annotation.Value;
            }

            var table = storeObject;
            var sharedTableRootProperty = property.FindSharedStoreObjectRootProperty(storeObject);
            if (sharedTableRootProperty != null)
            {
                var strategy = sharedTableRootProperty.GetValueGenerationStrategy(storeObject, typeMappingSource);
                bool isIdentityColumn = strategy == ActianValueGenerationStrategy.IdentityColumn;
                bool isIdentityByDefaultColumn = strategy == ActianValueGenerationStrategy.IdentityByDefaultColumn;
                bool isTable = table.StoreObjectType == StoreObjectType.Table;

                if ((isIdentityColumn || isIdentityByDefaultColumn) && isTable)
                {
                    bool hasInvalidForeignKey = property.GetContainingForeignKeys().Any(fk =>
                        !fk.IsBaseLinking() ||
                        (StoreObjectIdentifier.Create(fk.PrincipalEntityType, StoreObjectType.Table) is StoreObjectIdentifier principal
                            && fk.GetConstraintName(table, principal) != null));

                    if (!hasInvalidForeignKey)
                    {
                        return isIdentityColumn
                            ? ActianValueGenerationStrategy.IdentityColumn
                            : ActianValueGenerationStrategy.IdentityByDefaultColumn;
                    }
                }
            }

            if (property.ValueGenerated != ValueGenerated.OnAdd
                || table.StoreObjectType != StoreObjectType.Table
                || property.TryGetDefaultValue(storeObject, out _)
                || property.GetDefaultValueSql(storeObject) != null
                || property.GetComputedColumnSql(storeObject) != null
                || property.GetContainingForeignKeys()
                    .Any(
                        fk =>
                            !fk.IsBaseLinking()
                            || (StoreObjectIdentifier.Create(fk.PrincipalEntityType, StoreObjectType.Table)
                                    is StoreObjectIdentifier principal
                                && fk.GetConstraintName(table, principal) != null)))
            {
                return ActianValueGenerationStrategy.None;
            }

            var defaultStrategy = GetDefaultValueGenerationStrategy(property, storeObject, typeMappingSource);
            if (defaultStrategy != ActianValueGenerationStrategy.None)
            {
                if (annotation != null)
                {
                    return (ActianValueGenerationStrategy?)annotation.Value ?? ActianValueGenerationStrategy.None;
                }
            }

            return defaultStrategy;
        }

        /// <summary>
        ///     Returns the <see cref="ActianValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <remarks>
        ///     If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        /// </remarks>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The strategy, or <see cref="ActianValueGenerationStrategy.None" /> if none was set.</returns>
        public static ActianValueGenerationStrategy? GetValueGenerationStrategy(
            this IReadOnlyRelationalPropertyOverrides overrides)
            => (ActianValueGenerationStrategy?)overrides.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy)
                ?.Value;

        private static ActianValueGenerationStrategy GetDefaultValueGenerationStrategy(IReadOnlyProperty property)
        {
            var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

            if (modelStrategy is ActianValueGenerationStrategy.SequenceHiLo or ActianValueGenerationStrategy.Sequence
                && IsCompatibleWithValueGeneration(property))
            {
                return modelStrategy.Value;
            }

            return (modelStrategy == ActianValueGenerationStrategy.IdentityColumn
                    || modelStrategy == ActianValueGenerationStrategy.IdentityByDefaultColumn)
                && IsCompatibleWithValueGeneration(property)
                ? (modelStrategy == ActianValueGenerationStrategy.IdentityColumn
                    ? ActianValueGenerationStrategy.IdentityColumn
                    : ActianValueGenerationStrategy.IdentityByDefaultColumn)
                : ActianValueGenerationStrategy.None;
        }

        private static ActianValueGenerationStrategy GetDefaultValueGenerationStrategy(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            ITypeMappingSource? typeMappingSource)
        {
            var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

            if (modelStrategy is ActianValueGenerationStrategy.SequenceHiLo or ActianValueGenerationStrategy.Sequence
                && IsCompatibleWithValueGeneration(property, storeObject, typeMappingSource))
            {
                return modelStrategy.Value;
            }

            return (modelStrategy == ActianValueGenerationStrategy.IdentityColumn
                    || modelStrategy == ActianValueGenerationStrategy.IdentityByDefaultColumn)
                && IsCompatibleWithValueGeneration(property, storeObject, typeMappingSource)
                ? (property.DeclaringType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy
                    ? ActianValueGenerationStrategy.Sequence
                    : (modelStrategy == ActianValueGenerationStrategy.IdentityColumn
                        ? ActianValueGenerationStrategy.IdentityColumn
                        : ActianValueGenerationStrategy.IdentityByDefaultColumn))
                : ActianValueGenerationStrategy.None;
        }

        private static bool IsCompatibleWithValueGeneration(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            ITypeMappingSource? typeMappingSource)
        {
            if (storeObject.StoreObjectType != StoreObjectType.Table)
            {
                return false;
            }

            var valueConverter = property.GetValueConverter()
                ?? (property.FindRelationalTypeMapping(storeObject)
                    ?? typeMappingSource?.FindMapping((IProperty)property))?.Converter;

            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();

            return (type.IsInteger()
                || type.IsEnum
                || type == typeof(decimal));
        }

        /// <summary>
        ///     Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        public static void SetValueGenerationStrategy(
            this IMutableProperty property,
            ActianValueGenerationStrategy? value,
            in StoreObjectIdentifier storeObject)
            => property.GetOrCreateOverrides(storeObject)
                .SetValueGenerationStrategy(value);

        /// <summary>
        ///     Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static ActianValueGenerationStrategy? SetValueGenerationStrategy(
            this IConventionProperty property,
            ActianValueGenerationStrategy? value,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => property.GetOrCreateOverrides(storeObject, fromDataAnnotation)
                .SetValueGenerationStrategy(value, fromDataAnnotation);

        /// <summary>
        ///     Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static ActianValueGenerationStrategy? SetValueGenerationStrategy(
            this IConventionRelationalPropertyOverrides overrides,
            ActianValueGenerationStrategy? value,
            bool fromDataAnnotation = false)
            => (ActianValueGenerationStrategy?)overrides.SetOrRemoveAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy(overrides.Property, value),
                fromDataAnnotation)?.Value;

        /// <summary>
        /// Sets the <see cref="ActianValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetValueGenerationStrategy(
            [NotNull] this IConventionProperty property, ActianValueGenerationStrategy? value, bool fromDataAnnotation = false)
        {
            CheckValueGenerationStrategy(property, value);

            property.SetOrRemoveAnnotation(ActianAnnotationNames.ValueGenerationStrategy, value, fromDataAnnotation);
        }

        private static ActianValueGenerationStrategy? CheckValueGenerationStrategy(
            IReadOnlyProperty property,
            ActianValueGenerationStrategy? value)
        {
            if (value == null)
            {
                return null;
            }

            var propertyType = property.ClrType;

            if ((value == ActianValueGenerationStrategy.IdentityAlwaysColumn || value == ActianValueGenerationStrategy.IdentityByDefaultColumn)
                    && !propertyType.IsIntegerForValueGeneration())
            {
                throw new ArgumentException($"Identity value generation cannot be used for the property '{property.Name}' on entity type '{property.DeclaringType.DisplayName()}' because the property type is '{propertyType.ShortDisplayName()}'. Identity columns can only be of type short, int or long.");
            }

            if (value == ActianValueGenerationStrategy.SequenceHiLo && !propertyType.IsInteger())
            {
                throw new ArgumentException($"Actian sequences cannot be used to generate values for the property '{property.Name}' on entity type '{property.DeclaringType.DisplayName()}' because the property type is '{propertyType.ShortDisplayName()}'. Sequences can only be used with integer properties.");
            }

            return value;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the <see cref="ActianValueGenerationStrategy" />.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the <see cref="ActianValueGenerationStrategy" />.</returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(
            [NotNull] this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        /// <summary>
        /// Returns a value indicating whether the property is compatible with any <see cref="ActianValueGenerationStrategy"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if compatible.</returns>
        public static bool IsCompatibleWithValueGeneration(IReadOnlyProperty property)
        {
            var valueConverter = property.GetValueConverter()
                ?? property.FindTypeMapping()?.Converter;

            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type.IsInteger()
                || type.IsEnum
                || type == typeof(decimal);
        }

        static bool IsIntegerForValueGeneration(this Type type)
        {
            type = type.UnwrapNullableType();
            return type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(UInt32);
        }

        #endregion Value Generation Strategy
        #region Identity sequence options

        /// <summary>
        /// Returns the identity start value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity start value.</returns>
        public static long? GetIdentityStartValue([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).StartValue;

        /// <summary>
        /// Sets the identity start value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="startValue">The value to set.</param>
        public static void SetIdentityStartValue([NotNull] this IMutableProperty property, long? startValue)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.StartValue = startValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets the identity start value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="startValue">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityStartValue(
            [NotNull] this IConventionProperty property, long? startValue, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.StartValue = startValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns the identity increment value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity increment value.</returns>
        public static long? GetIdentityIncrementBy([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).IncrementBy;

        /// <summary>
        /// Sets the identity increment value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="incrementBy">The value to set.</param>
        public static void SetIdentityIncrementBy([NotNull] this IMutableProperty property, long? incrementBy)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.IncrementBy = incrementBy ?? 1;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets the identity increment value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="incrementBy">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityIncrementBy(
            [NotNull] this IConventionProperty property, long? incrementBy, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.IncrementBy = incrementBy ?? 1;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns the identity minimum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity minimum value.</returns>
        public static long? GetIdentityMinValue([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).MinValue;

        /// <summary>
        /// Sets the identity minimum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="minValue">The value to set.</param>
        public static void SetIdentityMinValue([NotNull] this IMutableProperty property, long? minValue)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.MinValue = minValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets the identity minimum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="minValue">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityMinValue(
            [NotNull] this IConventionProperty property, long? minValue, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.MinValue = minValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns the identity maximum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The identity maximum value.</returns>
        public static long? GetIdentityMaxValue([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).MaxValue;

        /// <summary>
        /// Sets the identity maximum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="maxValue">The value to set.</param>
        public static void SetIdentityMaxValue([NotNull] this IMutableProperty property, long? maxValue)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.MaxValue = maxValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets the identity maximum value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="maxValue">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityMaxValue(
            [NotNull] this IConventionProperty property, long? maxValue, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.MaxValue = maxValue;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns whether the identity's sequence is cyclic.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Whether the identity's sequence is cyclic.</returns>
        public static bool? GetIdentityIsCyclic([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).IsCyclic;

        /// <summary>
        /// Sets whether the identity's sequence is cyclic.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isCyclic">The value to set.</param>
        public static void SetIdentityIsCyclic([NotNull] this IMutableProperty property, bool? isCyclic)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.IsCyclic = isCyclic ?? false;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets whether the identity's sequence is cyclic.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isCyclic">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityIsCyclic(
            [NotNull] this IConventionProperty property, bool? isCyclic, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.IsCyclic = isCyclic ?? false;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns the number of sequence numbers to be preallocated and stored in memory for faster access.
        /// Defaults to 1 (no cache).
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The number of sequence numbers to be cached.</returns>
        public static long? GetIdentityNumbersToCache([NotNull] this IProperty property)
            => IdentitySequenceOptionsData.Get(property).NumbersToCache;

        /// <summary>
        /// Sets the number of sequence numbers to be preallocated and stored in memory for faster access.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="numbersToCache">The value to set.</param>
        public static void SetIdentityNumbersToCache([NotNull] this IMutableProperty property, long? numbersToCache)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.NumbersToCache = numbersToCache ?? 1;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize());
        }

        /// <summary>
        /// Sets the number of sequence numbers to be preallocated and stored in memory for faster access.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="numbersToCache">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static void SetIdentityNumbersToCache(
            [NotNull] this IConventionProperty property, long? numbersToCache, bool fromDataAnnotation = false)
        {
            var options = IdentitySequenceOptionsData.Get((IAnnotatable)property);
            options.NumbersToCache = numbersToCache ?? 1;
            property.SetOrRemoveAnnotation(ActianAnnotationNames.IdentityOptions, options.Serialize(), fromDataAnnotation);
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the identity sequence options.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the identity sequence options.</returns>
        public static ConfigurationSource? GetIdentityOptionsConfigurationSource([NotNull] this IConventionProperty property)
            => property.FindAnnotation(ActianAnnotationNames.IdentityOptions)?.GetConfigurationSource();


        /// <summary>
        /// Removes identity sequence options from the property.
        /// </summary>
        public static void RemoveIdentityOptions([NotNull] this IMutableProperty property)
            => property.RemoveAnnotation(ActianAnnotationNames.IdentityOptions);

        /// <summary>
        /// Removes identity sequence options from the property.
        /// </summary>
        public static void RemoveIdentityOptions([NotNull] this IConventionProperty property)
            => property.RemoveAnnotation(ActianAnnotationNames.IdentityOptions);

        #endregion Identity sequence options

    }
}
