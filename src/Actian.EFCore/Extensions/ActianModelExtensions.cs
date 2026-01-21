// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Actian.EFCore
{
    /// <summary>
    /// Extension methods for <see cref="IModel" /> for Actian-specific metadata.
    /// </summary>
    public static class ActianModelExtensions
    {
#nullable enable
        public const string DefaultHiLoSequenceName = "EntityFrameworkHiLoSequence";

        /// <summary>
        ///     The default prefix for sequences applied to properties.
        /// </summary>
        public const string DefaultSequenceNameSuffix = "Sequence";

        #region HiLo

        /// <summary>
        ///     Returns the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The name to use for the default hi-lo sequence. </returns>
        public static string GetHiLoSequenceName(this IReadOnlyModel model)
            => (string?)model[ActianAnnotationNames.HiLoSequenceName]
               ?? DefaultHiLoSequenceName;

        /// <summary>
        ///     Sets the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="name"> The value to set. </param>
        public static void SetHiLoSequenceName([NotNull] this IMutableModel model, [CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, nameof(name));

            model.SetOrRemoveAnnotation(ActianAnnotationNames.HiLoSequenceName, name);
        }

        /// <summary>
        ///     Sets the name to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="name"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static string? SetHiLoSequenceName(
            this IConventionModel model,
            string? name,
            bool fromDataAnnotation = false)
            => (string?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceName,
                Check.NullButNotEmpty(name, nameof(name)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default hi-lo sequence name.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default hi-lo sequence name. </returns>
        public static ConfigurationSource? GetHiLoSequenceNameConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.HiLoSequenceName)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the schema to use for the default hi-lo sequence.
        ///     <see cref="ActianPropertyBuilderExtensions.UseHiLo" />
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The schema to use for the default hi-lo sequence. </returns>
        public static string? GetHiLoSequenceSchema([NotNull] this IReadOnlyModel model)
            => (string?)model[ActianAnnotationNames.HiLoSequenceSchema];

        /// <summary>
        ///     Sets the schema to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetHiLoSequenceSchema(this IMutableModel model, string? value)
        {
            Check.NullButNotEmpty(value, nameof(value));

            model.SetOrRemoveAnnotation(ActianAnnotationNames.HiLoSequenceSchema, value);
        }

        /// <summary>
        ///     Sets the schema to use for the default hi-lo sequence.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static string? SetHiLoSequenceSchema(
            this IConventionModel model,
            string? value,
            bool fromDataAnnotation = false)
            => (string?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.HiLoSequenceSchema,
                Check.NullButNotEmpty(value, nameof(value)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default hi-lo sequence schema.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the default hi-lo sequence schema.</returns>
        public static ConfigurationSource? GetHiLoSequenceSchemaConfigurationSource(this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.HiLoSequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the suffix to append to the name of automatically created sequences.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The name to use for the default key value generation sequence.</returns>
        public static string GetSequenceNameSuffix(this IReadOnlyModel model)
            => (string?)model[ActianAnnotationNames.SequenceNameSuffix]
                ?? DefaultSequenceNameSuffix;

        /// <summary>
        ///     Sets the suffix to append to the name of automatically created sequences.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="name">The value to set.</param>
        public static void SetSequenceNameSuffix(this IMutableModel model, string? name)
        {
            Check.NullButNotEmpty(name, nameof(name));

            model.SetOrRemoveAnnotation(ActianAnnotationNames.SequenceNameSuffix, name);
        }

        /// <summary>
        ///     Sets the suffix to append to the name of automatically created sequences.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="name">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static string? SetSequenceNameSuffix(
            this IConventionModel model,
            string? name,
            bool fromDataAnnotation = false)
            => (string?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceNameSuffix,
                Check.NullButNotEmpty(name, nameof(name)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default value generation sequence name suffix.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the default key value generation sequence name.</returns>
        public static ConfigurationSource? GetSequenceNameSuffixConfigurationSource(this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.SequenceNameSuffix)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the schema to use for the default value generation sequence.
        ///     <see cref="ActianPropertyBuilderExtensions.UseSequence" />
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The schema to use for the default key value generation sequence.</returns>
        public static string? GetSequenceSchema(this IReadOnlyModel model)
            => (string?)model[ActianAnnotationNames.SequenceSchema];

        /// <summary>
        ///     Sets the schema to use for the default key value generation sequence.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="value">The value to set.</param>
        public static void SetSequenceSchema(this IMutableModel model, string? value)
        {
            Check.NullButNotEmpty(value, nameof(value));

            model.SetOrRemoveAnnotation(ActianAnnotationNames.SequenceSchema, value);
        }

        /// <summary>
        ///     Sets the schema to use for the default key value generation sequence.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static string? SetSequenceSchema(
            this IConventionModel model,
            string? value,
            bool fromDataAnnotation = false)
            => (string?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.SequenceSchema,
                Check.NullButNotEmpty(value, nameof(value)),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default key value generation sequence schema.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the default key value generation sequence schema.</returns>
        public static ConfigurationSource? GetSequenceSchemaConfigurationSource(this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.SequenceSchema)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the default identity seed.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The default identity seed.</returns>
        public static long GetIdentitySeed(this IReadOnlyModel model)
        {
            if (model is RuntimeModel)
            {
                throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData);
            }

            // Support pre-6.0 IdentitySeed annotations, which contained an int rather than a long
            var annotation = model.FindAnnotation(ActianAnnotationNames.IdentitySeed);
            return annotation is null || annotation.Value is null
                ? 1
                : annotation.Value is int intValue
                    ? intValue
                    : (long)annotation.Value;
        }

        /// <summary>
        ///     Sets the default identity seed.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="seed">The value to set.</param>
        public static void SetIdentitySeed(this IMutableModel model, long? seed)
            => model.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentitySeed,
                seed);

        /// <summary>
        ///     Sets the default identity seed.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="seed">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static long? SetIdentitySeed(this IConventionModel model, long? seed, bool fromDataAnnotation = false)
            => (long?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentitySeed,
                seed,
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default schema.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the default schema.</returns>
        public static ConfigurationSource? GetIdentitySeedConfigurationSource(this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.IdentitySeed)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the default identity increment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The default identity increment.</returns>
        public static int GetIdentityIncrement(this IReadOnlyModel model)
            => (model is RuntimeModel)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (int?)model[ActianAnnotationNames.IdentityIncrement] ?? 1;

        /// <summary>
        ///     Sets the default identity increment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="increment">The value to set.</param>
        public static void SetIdentityIncrement(this IMutableModel model, int? increment)
            => model.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentityIncrement,
                increment);

        /// <summary>
        ///     Sets the default identity increment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="increment">The value to set.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static int? SetIdentityIncrement(
            this IConventionModel model,
            int? increment,
            bool fromDataAnnotation = false)
            => (int?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.IdentityIncrement,
                increment,
                fromDataAnnotation)?.Value;

        #endregion

        #region Value Generation Strategy

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default identity increment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the default identity increment.</returns>
        public static ConfigurationSource? GetIdentityIncrementConfigurationSource(this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ActianValueGenerationStrategy" /> to use for properties
        ///     of keys in the model, unless the property has a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The default <see cref="ActianValueGenerationStrategy" />. </returns>
        public static ActianValueGenerationStrategy? GetValueGenerationStrategy(this IReadOnlyModel model)
            => (ActianValueGenerationStrategy?)model[ActianAnnotationNames.ValueGenerationStrategy];

        /// <summary>
        ///     Attempts to set the <see cref="ActianValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        public static void SetValueGenerationStrategy(
            this IMutableModel model,
            ActianValueGenerationStrategy? value)
            => model.SetOrRemoveAnnotation(ActianAnnotationNames.ValueGenerationStrategy, value);

        /// <summary>
        ///     Attempts to set the <see cref="ActianValueGenerationStrategy" /> to use for properties
        ///     of keys in the model that don't have a strategy explicitly set.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <param name="value"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        public static ActianValueGenerationStrategy? SetValueGenerationStrategy(
            this IConventionModel model,
            ActianValueGenerationStrategy? value,
            bool fromDataAnnotation = false)
            => (ActianValueGenerationStrategy?)model.SetOrRemoveAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy,
                value,
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the default <see cref="ActianValueGenerationStrategy" />.
        /// </summary>
        /// <param name="model"> The model. </param>
        /// <returns> The <see cref="ConfigurationSource" /> for the default <see cref="ActianValueGenerationStrategy" />. </returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource([NotNull] this IConventionModel model)
            => model.FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        #endregion

        /// <summary>
        ///     Returns the maximum size of the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The maximum size of the database.</returns>
        public static string? GetDatabaseMaxSize(this IReadOnlyModel model)
            => (model is RuntimeModel)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (string?)model[ActianAnnotationNames.MaxDatabaseSize];

        /// <summary>
        ///     Returns the service tier of the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The service tier of the database.</returns>
        public static string? GetServiceTierSql(this IReadOnlyModel model)
            => (model is RuntimeModel)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (string?)model[ActianAnnotationNames.ServiceTierSql];

        /// <summary>
        ///     Returns the performance level of the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The performance level of the database.</returns>
        public static string? GetPerformanceLevelSql(this IReadOnlyModel model)
            => (model is RuntimeModel)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (string?)model[ActianAnnotationNames.PerformanceLevelSql];
    }
}
