// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using Actian.EFCore.Metadata.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Actian.EFCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that ensures that properties aren't configured to have a default value, as computed column
    ///     or using a <see cref="ActianValueGenerationStrategy" /> at the same time.
    /// </summary>
    public class ActianStoreGenerationConvention : StoreGenerationConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ActianStoreGenerationConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        /// <param name="relationalDependencies">  Parameter object containing relational dependencies for this convention. </param>
        public ActianStoreGenerationConvention(
            [NotNull] ProviderConventionSetBuilderDependencies dependencies,
            [NotNull] RelationalConventionSetBuilderDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     Called after an annotation is changed on a property.
        /// </summary>
        /// <param name="propertyBuilder"> The builder for the property. </param>
        /// <param name="name"> The annotation name. </param>
        /// <param name="annotation"> The new annotation. </param>
        /// <param name="oldAnnotation"> The old annotation.  </param>
        /// <param name="context"> Additional information associated with convention execution. </param>
        public override void ProcessPropertyAnnotationChanged(
            IConventionPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            if (annotation == null
                || oldAnnotation?.Value != null)
            {
                return;
            }

            var configurationSource = annotation.GetConfigurationSource();
            var fromDataAnnotation = configurationSource != ConfigurationSource.Convention;
            switch (name)
            {
                case RelationalAnnotationNames.DefaultValue:
                    if (propertyBuilder.HasValueGenerationStrategy(null, fromDataAnnotation) == null
                        && propertyBuilder.HasDefaultValue(null, fromDataAnnotation) != null)
                    {
                        context.StopProcessing();
                        return;
                    }

                    break;
                case RelationalAnnotationNames.DefaultValueSql:
                    if (propertyBuilder.HasValueGenerationStrategy(null, fromDataAnnotation) == null
                        && propertyBuilder.HasDefaultValueSql(null, fromDataAnnotation) != null)
                    {
                        context.StopProcessing();
                        return;
                    }

                    break;
                case RelationalAnnotationNames.ComputedColumnSql:
                    if (propertyBuilder.HasValueGenerationStrategy(null, fromDataAnnotation) == null
                        && propertyBuilder.HasComputedColumnSql(null, fromDataAnnotation) != null)
                    {
                        context.StopProcessing();
                        return;
                    }

                    break;
                case ActianAnnotationNames.ValueGenerationStrategy:
                    if ((propertyBuilder.HasDefaultValue(null, fromDataAnnotation) == null
                            | propertyBuilder.HasDefaultValueSql(null, fromDataAnnotation) == null
                            | propertyBuilder.HasComputedColumnSql(null, fromDataAnnotation) == null)
                        && propertyBuilder.HasValueGenerationStrategy(null, fromDataAnnotation) != null)
                    {
                        context.StopProcessing();
                        return;
                    }

                    break;
            }

            base.ProcessPropertyAnnotationChanged(propertyBuilder, name, annotation, oldAnnotation, context);
        }

        protected override void Validate ([NotNull] IConventionProperty property, in StoreObjectIdentifier storeObject)
        {
            var generationStrategy = property.GetValueGenerationStrategy(storeObject, Dependencies.TypeMappingSource);
            if (generationStrategy == ActianValueGenerationStrategy.None)
            {
                base.Validate(property, storeObject);
                return;
            }

            if (property.GetDefaultValue() != null)
            {
                throw new InvalidOperationException(
                    RelationalStrings.ConflictingColumnServerGeneration(
                        "ActianValueGenerationStrategy", property.Name, "DefaultValue"));
            }

            if (property.GetDefaultValueSql() != null)
            {
                throw new InvalidOperationException(
                    RelationalStrings.ConflictingColumnServerGeneration(
                        "ActianValueGenerationStrategy", property.Name, "DefaultValueSql"));
            }

            if (property.GetComputedColumnSql() != null)
            {
                throw new InvalidOperationException(
                    RelationalStrings.ConflictingColumnServerGeneration(
                        "ActianValueGenerationStrategy", property.Name, "ComputedColumnSql"));
            }

            base.Validate(property, storeObject);
        }
    }
}
