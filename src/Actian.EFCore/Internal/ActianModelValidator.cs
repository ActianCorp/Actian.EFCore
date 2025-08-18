// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using Actian.EFCore.Metadata.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

// TODO: ActianModelValidator

#nullable enable

namespace Actian.EFCore.Internal
{
    public class ActianModelValidator : RelationalModelValidator
    {
        public ActianModelValidator(
            [NotNull] ModelValidatorDependencies dependencies,
            [NotNull] RelationalModelValidatorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.Validate(model, logger);
        }

        protected override void ValidateValueGeneration(
            IEntityType entityType,
            IKey key,
            IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            if (entityType.GetTableName() != null
                && (string?)entityType[RelationalAnnotationNames.MappingStrategy] == RelationalAnnotationNames.TpcMappingStrategy)
            {
                foreach (var storeGeneratedProperty in key.Properties.Where(
                             p => (p.ValueGenerated & ValueGenerated.OnAdd) != 0
                                 && (p.GetValueGenerationStrategy() == ActianValueGenerationStrategy.IdentityColumn) ||
                                     p.GetValueGenerationStrategy() == ActianValueGenerationStrategy.IdentityByDefaultColumn))
                {
                    logger.TpcStoreGeneratedIdentityWarning(storeGeneratedProperty);
                }
            }
        }

        /// <inheritdoc />
        protected override void ValidateCompatible(
            IProperty property,
            IProperty duplicateProperty,
            string columnName,
            in StoreObjectIdentifier storeObject,
            IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.ValidateCompatible(property, duplicateProperty, columnName, storeObject, logger);

            var propertyStrategy = property.GetValueGenerationStrategy(storeObject);
            var duplicatePropertyStrategy = duplicateProperty.GetValueGenerationStrategy(storeObject);
            if (propertyStrategy != duplicatePropertyStrategy)
            {
                var isConflicting = ((IConventionProperty)property)
                    .FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy)
                    ?.GetConfigurationSource()
                    == ConfigurationSource.Explicit
                    || propertyStrategy != ActianValueGenerationStrategy.None;
                var isDuplicateConflicting = ((IConventionProperty)duplicateProperty)
                    .FindAnnotation(ActianAnnotationNames.ValueGenerationStrategy)
                    ?.GetConfigurationSource()
                    == ConfigurationSource.Explicit
                    || duplicatePropertyStrategy != ActianValueGenerationStrategy.None;

                if (isConflicting && isDuplicateConflicting)
                {
                    throw new InvalidOperationException(
                        ActianStrings.DuplicateColumnNameValueGenerationStrategyMismatch(
                            duplicateProperty.DeclaringType.DisplayName(),
                            duplicateProperty.Name,
                            property.DeclaringType.DisplayName(),
                            property.Name,
                            columnName,
                            storeObject.DisplayName()));
                }
            }
            else
            {
                switch (propertyStrategy)
                {
                    case ActianValueGenerationStrategy.IdentityColumn:
                        var increment = property.GetIdentityIncrement(storeObject);
                        var duplicateIncrement = duplicateProperty.GetIdentityIncrement(storeObject);
                        if (increment != duplicateIncrement)
                        {
                            throw new InvalidOperationException(
                                ActianStrings.DuplicateColumnIdentityIncrementMismatch(
                                    duplicateProperty.DeclaringType.DisplayName(),
                                    duplicateProperty.Name,
                                    property.DeclaringType.DisplayName(),
                                    property.Name,
                                    columnName,
                                    storeObject.DisplayName()));
                        }

                        var seed = property.GetIdentitySeed(storeObject);
                        var duplicateSeed = duplicateProperty.GetIdentitySeed(storeObject);
                        if (seed != duplicateSeed)
                        {
                            throw new InvalidOperationException(
                                ActianStrings.DuplicateColumnIdentitySeedMismatch(
                                    duplicateProperty.DeclaringType.DisplayName(),
                                    duplicateProperty.Name,
                                    property.DeclaringType.DisplayName(),
                                    property.Name,
                                    columnName,
                                    storeObject.DisplayName()));
                        }

                        break;
                    case ActianValueGenerationStrategy.SequenceHiLo:
                        if (property.GetHiLoSequenceName(storeObject) != duplicateProperty.GetHiLoSequenceName(storeObject)
                            || property.GetHiLoSequenceSchema(storeObject) != duplicateProperty.GetHiLoSequenceSchema(storeObject))
                        {
                            throw new InvalidOperationException(
                                ActianStrings.DuplicateColumnSequenceMismatch(
                                    duplicateProperty.DeclaringType.DisplayName(),
                                    duplicateProperty.Name,
                                    property.DeclaringType.DisplayName(),
                                    property.Name,
                                    columnName,
                                    storeObject.DisplayName()));
                        }

                        break;
                }
            }
        }
    }
}
