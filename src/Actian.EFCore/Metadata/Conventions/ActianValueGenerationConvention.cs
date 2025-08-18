// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq;
using Actian.EFCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

#nullable enable

// ReSharper disable once CheckNamespace
namespace Actian.EFCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures store value generation as <see cref="ValueGenerated.OnAdd" /> on properties that are
    ///     part of the primary key and not part of any foreign keys, were configured to have a database default value
    ///     or were configured to use a <see cref="ActianValueGenerationStrategy" />.
    ///     It also configures properties as <see cref="ValueGenerated.OnAddOrUpdate" /> if they were configured as computed columns.
    /// </summary>
    public class ActianValueGenerationConvention : RelationalValueGenerationConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ActianValueGenerationConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        /// <param name="relationalDependencies">  Parameter object containing relational dependencies for this convention. </param>
        public ActianValueGenerationConvention(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
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
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override void ProcessPropertyAnnotationChanged(
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            IConventionPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            if (name == ActianAnnotationNames.ValueGenerationStrategy)
            {
                propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata));
                return;
            }

            base.ProcessPropertyAnnotationChanged(propertyBuilder, name, annotation, oldAnnotation, context);
        }

        /// <summary>
        ///     Called after an annotation is changed on an entity.
        /// </summary>
        /// <param name="entityTypeBuilder">The builder for the entity type.</param>
        /// <param name="name">The annotation name.</param>
        /// <param name="annotation">The new annotation.</param>
        /// <param name="oldAnnotation">The old annotation.</param>
        /// <param name="context">Additional information associated with convention execution.</param>
        public override void ProcessEntityTypeAnnotationChanged(
            IConventionEntityTypeBuilder entityTypeBuilder,
            string name,
            IConventionAnnotation? annotation,
            IConventionAnnotation? oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            //if (name is ActianAnnotationNames.TemporalPeriodStartPropertyName or ActianAnnotationNames.TemporalPeriodEndPropertyName
            //    && annotation?.Value is string propertyName)
            //{
            //    var periodProperty = entityTypeBuilder.Metadata.FindProperty(propertyName);
            //    periodProperty?.Builder.ValueGenerated(GetValueGenerated(periodProperty));

            //    // cleanup the previous period property - its possible that it won't be deleted
            //    // (e.g. when removing period with default name, while the property with that same name has been explicitly defined)
            //    if (oldAnnotation?.Value is string oldPropertyName)
            //    {
            //        var oldPeriodProperty = entityTypeBuilder.Metadata.FindProperty(oldPropertyName);
            //        oldPeriodProperty?.Builder.ValueGenerated(GetValueGenerated(oldPeriodProperty));
            //    }
            //}

            base.ProcessEntityTypeAnnotationChanged(entityTypeBuilder, name, annotation, oldAnnotation, context);
        }

        /// <summary>
        ///     Returns the store value generation strategy to set for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The store value generation strategy to set for the given property.</returns>
        protected override ValueGenerated? GetValueGenerated(IConventionProperty property)
        {
            var declaringTable = property.GetMappedStoreObjects(StoreObjectType.Table).FirstOrDefault();
            if (declaringTable.Name == null)
            {
                return null;
            }

            // If the first mapping can be value generated then we'll consider all mappings to be value generated
            // as this is a client-side configuration and can't be specified per-table.
            return GetValueGenerated(property, declaringTable, Dependencies.TypeMappingSource);
        }

        /// <summary>
        ///     Returns the store value generation strategy to set for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the store object.</param>
        /// <returns>The store value generation strategy to set for the given property.</returns>
        public static new ValueGenerated? GetValueGenerated(IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
            => RelationalValueGenerationConvention.GetValueGenerated(property, storeObject)
                ?? (property.GetValueGenerationStrategy(storeObject) != ActianValueGenerationStrategy.None
                    ? ValueGenerated.OnAdd
                    : null);

        private static ValueGenerated? GetValueGenerated(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            ITypeMappingSource typeMappingSource)
            => GetTemporalValueGenerated(property)
                ?? RelationalValueGenerationConvention.GetValueGenerated(property, storeObject)
                ?? (property.GetValueGenerationStrategy(storeObject, typeMappingSource) != ActianValueGenerationStrategy.None
                    ? ValueGenerated.OnAdd
                    : null);

        private static ValueGenerated? GetTemporalValueGenerated(IReadOnlyProperty property)
        {
            return null;
        }
    }
}
