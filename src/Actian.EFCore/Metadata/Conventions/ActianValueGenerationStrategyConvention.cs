// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Actian.EFCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the default model <see cref="ActianValueGenerationStrategy" /> as
    ///     <see cref="ActianValueGenerationStrategy.IdentityByDefaultColumn" />.
    /// </summary>
    public class ActianValueGenerationStrategyConvention : IModelInitializedConvention, IModelFinalizingConvention
    {
        public ActianValueGenerationStrategyConvention(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
        {
            Dependencies = dependencies;
            RelationalDependencies = relationalDependencies;
        }

        protected virtual ProviderConventionSetBuilderDependencies Dependencies { get; }

        /// <summary>
        ///     Relational provider-specific dependencies for this service.
        /// </summary>
        protected virtual RelationalConventionSetBuilderDependencies RelationalDependencies { get; }

        public virtual void ProcessModelInitialized(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            modelBuilder.HasValueGenerationStrategy(ActianValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        public virtual void ProcessModelFinalizing(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var property in entityType.GetDeclaredProperties())
                {
                    ActianValueGenerationStrategy? strategy = null;
                    var declaringTable = property.GetMappedStoreObjects(StoreObjectType.Table).FirstOrDefault();
                    if (declaringTable.Name != null!)
                    {
                        strategy = property.GetValueGenerationStrategy(declaringTable, Dependencies.TypeMappingSource);
                        if (strategy == ActianValueGenerationStrategy.None
                            && !IsStrategyNoneNeeded(property, declaringTable))
                        {
                            strategy = null;
                        }
                    }
                    else
                    {
                        var declaringView = property.GetMappedStoreObjects(StoreObjectType.View).FirstOrDefault();
                        if (declaringView.Name != null!)
                        {
                            strategy = property.GetValueGenerationStrategy(declaringView, Dependencies.TypeMappingSource);
                            if (strategy == ActianValueGenerationStrategy.None
                                && !IsStrategyNoneNeeded(property, declaringView))
                            {
                                strategy = null;
                            }
                        }
                    }

                    // Needed for the annotation to show up in the model snapshot
                    if (strategy != null
                        && declaringTable.Name != null)
                    {
                        property.Builder.HasValueGenerationStrategy(strategy);

                        if (strategy == ActianValueGenerationStrategy.Sequence)
                        {
                            var sequence = modelBuilder.HasSequence(
                                property.GetSequenceName(declaringTable)
                                ?? entityType.GetRootType().ShortName() + modelBuilder.Metadata.GetSequenceNameSuffix(),
                                property.GetSequenceSchema(declaringTable)
                                ?? modelBuilder.Metadata.GetSequenceSchema()).Metadata;

                            property.Builder.HasDefaultValueSql(
                                RelationalDependencies.UpdateSqlGenerator.GenerateObtainNextSequenceValueOperation(
                                    sequence.Name, sequence.Schema));
                        }
                    }
                }
            }

            bool IsStrategyNoneNeeded(IReadOnlyProperty property, StoreObjectIdentifier storeObject)
            {
                if (property.ValueGenerated == ValueGenerated.OnAdd
                    && !property.TryGetDefaultValue(storeObject, out _)
                    && property.GetDefaultValueSql(storeObject) == null
                    && property.GetComputedColumnSql(storeObject) == null
                    && (property.DeclaringType.Model.GetValueGenerationStrategy() == ActianValueGenerationStrategy.IdentityColumn
                        || property.DeclaringType.Model.GetValueGenerationStrategy() == ActianValueGenerationStrategy.IdentityByDefaultColumn))
                {
                    var providerClrType = (property.GetValueConverter()
                            ?? (property.FindRelationalTypeMapping(storeObject)
                                ?? Dependencies.TypeMappingSource.FindMapping((IProperty)property))?.Converter)
                        ?.ProviderClrType.UnwrapNullableType();

                    return providerClrType != null
                        && (providerClrType.IsInteger() || providerClrType == typeof(decimal));
                }

                return false;
            }
        }

        public IModel ProcessModelFinalized(IModel model)
        {
            foreach (var type in model.GetEntityTypes())
            {
                foreach (var property in type.GetDeclaredProperties())
                {
                    var strategy = property.GetValueGenerationStrategy();
                    {
                        if (strategy != ActianValueGenerationStrategy.None)
                        {
                            var before = property.GetBeforeSaveBehavior();
                            var after = property.GetAfterSaveBehavior();
                        }
                    }
                }
            }
            return model;
        }
    }
}
