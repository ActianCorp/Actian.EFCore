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
    public static class ActianModelBuilderExtensions
    {
        #region HiLo

        public static ModelBuilder UseHiLo(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string? name = null,
            [CanBeNull] string? schema = null)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var model = modelBuilder.Model;

            name ??= ActianModelExtensions.DefaultHiLoSequenceName;

            if (model.FindSequence(name, schema) == null)
            {
                modelBuilder.HasSequence(name, schema).IncrementsBy(10);
            }

            model.SetValueGenerationStrategy(ActianValueGenerationStrategy.SequenceHiLo);
            model.SetHiLoSequenceName(name);
            model.SetHiLoSequenceSchema(schema);

            return modelBuilder;
        }

        public static IConventionSequenceBuilder? HasHiLoSequence(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string? name,
            [CanBeNull] string? schema,
            bool fromDataAnnotation = false)
        {
            if (!modelBuilder.CanSetHiLoSequence(name, schema))
            {
                return null;
            }

            modelBuilder.Metadata.SetHiLoSequenceName(name, fromDataAnnotation);
            modelBuilder.Metadata.SetHiLoSequenceSchema(schema, fromDataAnnotation);

            return name == null ? null : modelBuilder.HasSequence(name, schema, fromDataAnnotation);
        }

        public static bool CanSetHiLoSequence(
            [NotNull] this IConventionModelBuilder modelBuilder,
            [CanBeNull] string? name,
            [CanBeNull] string? schema,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            return modelBuilder.CanSetAnnotation(ActianAnnotationNames.HiLoSequenceName, name, fromDataAnnotation)
                   && modelBuilder.CanSetAnnotation(ActianAnnotationNames.HiLoSequenceSchema, schema, fromDataAnnotation);
        }

        #endregion HiLo

        #region Identity Always

        public static ModelBuilder UseIdentityAlwaysColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var property = modelBuilder.Model;

            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityAlwaysColumn);
            property.SetHiLoSequenceName(null!);
            property.SetHiLoSequenceSchema(null);

            return modelBuilder;
        }

        #endregion Identity Always

        #region Identity By Default

        public static ModelBuilder UseIdentityByDefaultColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            var property = modelBuilder.Model;

            property.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
            property.SetHiLoSequenceName(null!);
            property.SetHiLoSequenceSchema(null);

            return modelBuilder;
        }

        public static ModelBuilder UseIdentityColumns(
            [NotNull] this ModelBuilder modelBuilder)
            => modelBuilder.UseIdentityByDefaultColumns();

        /// <summary>
        ///     Configures the model to use the SQL Server IDENTITY feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting SQL Server. This is the default
        ///     behavior when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder UseIdentityColumns(
            this ModelBuilder modelBuilder,
            long seed = 1,
            int increment = 1)
        {
            var model = modelBuilder.Model;

            model.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityColumn);
            model.SetIdentitySeed(seed);
            model.SetIdentityIncrement(increment);
            model.SetSequenceNameSuffix(null);
            model.SetSequenceSchema(null);
            model.SetHiLoSequenceName(null!);
            model.SetHiLoSequenceSchema(null);

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the model to use the SQL Server IDENTITY feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting SQL Server. This is the default
        ///     behavior when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder UseIdentityByDefaultColumns(
            this ModelBuilder modelBuilder,
            long seed = 1,
            int increment = 1)
        {
            //System.Diagnostics.Debugger.Break();

            var model = modelBuilder.Model;

            model.SetValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
            model.SetIdentitySeed(seed);
            model.SetIdentityIncrement(increment);
            model.SetSequenceNameSuffix(null);
            model.SetSequenceSchema(null);
            model.SetHiLoSequenceName(null!);
            model.SetHiLoSequenceSchema(null);

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the model to use the SQL Server IDENTITY feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting SQL Server. This is the default
        ///     behavior when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder UseIdentityColumns(
            this ModelBuilder modelBuilder,
            int seed,
            int increment = 1)
            => modelBuilder.UseIdentityColumns((long)seed, increment);

        /// <summary>
        ///     Configures the model to use the SQL Server IDENTITY feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting SQL Server. This is the default
        ///     behavior when targeting SQL Server.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder UseIdentityByDefaultColumns(
            this ModelBuilder modelBuilder,
            int seed,
            int increment = 1)
            => modelBuilder.UseIdentityByDefaultColumns((long)seed, increment);

        #endregion Identity By Default

        #region Value Generation Strategy

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the default value generation strategy.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="valueGenerationStrategy">The value generation strategy.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default value generation strategy.</returns>
        public static bool CanSetValueGenerationStrategy(
            this IConventionModelBuilder modelBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
            => modelBuilder.CanSetAnnotation(
                ActianAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the default seed for SQL Server IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the seed for SQL Server IDENTITY.</returns>
        public static bool CanSetIdentityColumnSeed(
            this IConventionModelBuilder modelBuilder,
            long? seed,
            bool fromDataAnnotation = false)
            => modelBuilder.CanSetAnnotation(ActianAnnotationNames.IdentitySeed, seed, fromDataAnnotation);

        /// <summary>
        ///     Returns a value indicating whether the given value can be set as the default increment for SQL Server IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given value can be set as the default increment for SQL Server IDENTITY.</returns>
        public static bool CanSetIdentityColumnIncrement(
            this IConventionModelBuilder modelBuilder,
            int? increment,
            bool fromDataAnnotation = false)
            => modelBuilder.CanSetAnnotation(ActianAnnotationNames.IdentityIncrement, increment, fromDataAnnotation);

        /// <summary>
        ///     Configures the default seed for SQL Server IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="seed">The value that is used for the very first row loaded into the table.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder? HasIdentityColumnSeed(
            this IConventionModelBuilder modelBuilder,
            long? seed,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetIdentityColumnSeed(seed, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetIdentitySeed(seed, fromDataAnnotation);
                return modelBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Configures the default increment for SQL Server IDENTITY.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://aka.ms/efcore-docs-sqlserver">Accessing SQL Server and Azure SQL databases with EF Core</see>
        ///     for more information and examples.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="increment">The incremental value that is added to the identity value of the previous row that was loaded.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder? HasIdentityColumnIncrement(
            this IConventionModelBuilder modelBuilder,
            int? increment,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetIdentityColumnIncrement(increment, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetIdentityIncrement(increment, fromDataAnnotation);
                return modelBuilder;
            }

            return null;
        }

        public static IConventionModelBuilder? HasValueGenerationStrategy(
            this IConventionModelBuilder modelBuilder,
            ActianValueGenerationStrategy? valueGenerationStrategy,
            bool fromDataAnnotation = false)
        {
            if (modelBuilder.CanSetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation))
            {
                modelBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
                if (valueGenerationStrategy != ActianValueGenerationStrategy.IdentityColumn &&
                    valueGenerationStrategy != ActianValueGenerationStrategy.IdentityByDefaultColumn)
                {
                    modelBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                    modelBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
                }

                if (valueGenerationStrategy != ActianValueGenerationStrategy.SequenceHiLo)
                {
                    modelBuilder.HasHiLoSequence(null!, null!, fromDataAnnotation);
                }

                if (valueGenerationStrategy != ActianValueGenerationStrategy.Sequence)
                {
                    RemoveKeySequenceAnnotations();
                }

                return modelBuilder;
            }

            return null;

            void RemoveKeySequenceAnnotations()
            {
                if (modelBuilder.CanSetAnnotation(ActianAnnotationNames.SequenceNameSuffix, null)
                    && modelBuilder.CanSetAnnotation(ActianAnnotationNames.SequenceSchema, null))
                {
                    modelBuilder.Metadata.SetSequenceNameSuffix(null, fromDataAnnotation);
                    modelBuilder.Metadata.SetSequenceSchema(null, fromDataAnnotation);
                }
            }
        }

        #endregion Value Generation Strategy

        public static ModelBuilder UseKeySequences(
            this ModelBuilder modelBuilder,
            string? nameSuffix = null,
            string? schema = null)
        {
            Check.NullButNotEmpty(nameSuffix, nameof(nameSuffix));
            Check.NullButNotEmpty(schema, nameof(schema));

            var model = modelBuilder.Model;

            nameSuffix ??= ActianModelExtensions.DefaultSequenceNameSuffix;

            model.SetValueGenerationStrategy(ActianValueGenerationStrategy.Sequence);
            model.SetSequenceNameSuffix(nameSuffix);
            model.SetSequenceSchema(schema);
            model.SetHiLoSequenceName(null!);
            model.SetHiLoSequenceSchema(null);
            model.SetIdentitySeed(null);
            model.SetIdentityIncrement(null);

            return modelBuilder;
        }
    }
}
