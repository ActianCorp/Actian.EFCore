// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Utilities;
using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Actian.EFCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

namespace Actian.EFCore.Design.Internal
{
    public class ActianAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        #region MethodInfos

        private static readonly MethodInfo ModelUseIdentityColumnsMethodInfo
            = typeof(ActianModelBuilderExtensions).GetRuntimeMethod(
                nameof(ActianModelBuilderExtensions.UseIdentityColumns), new[] { typeof(ModelBuilder), typeof(long), typeof(int) })!;

        private static readonly MethodInfo ModelUseIdentityByDefaultColumnsMethodInfo
            = typeof(ActianModelBuilderExtensions).GetRuntimeMethod(
                nameof(ActianModelBuilderExtensions.UseIdentityByDefaultColumns), new[] { typeof(ModelBuilder), typeof(long), typeof(int) })!;

        private static readonly MethodInfo ModelUseHiLoMethodInfo
            = typeof(ActianModelBuilderExtensions).GetRuntimeMethod(
                nameof(ActianModelBuilderExtensions.UseHiLo), new[] { typeof(ModelBuilder), typeof(string), typeof(string) })!;

        private static readonly MethodInfo ModelUseKeySequencesMethodInfo
            = typeof(ActianModelBuilderExtensions).GetRuntimeMethod(
                nameof(ActianModelBuilderExtensions.UseKeySequences), new[] { typeof(ModelBuilder), typeof(string), typeof(string) })!;

        private static readonly MethodInfo ModelHasAnnotationMethodInfo
            = typeof(ModelBuilder).GetRuntimeMethod(
                nameof(ModelBuilder.HasAnnotation), new[] { typeof(string), typeof(object) })!;

        private static readonly MethodInfo EntityTypeToTableMethodInfo
            = typeof(RelationalEntityTypeBuilderExtensions).GetRuntimeMethod(
                nameof(RelationalEntityTypeBuilderExtensions.ToTable), new[] { typeof(EntityTypeBuilder), typeof(string) })!;

        //private static readonly MethodInfo PropertyIsSparseMethodInfo
        //    = typeof(ActianPropertyBuilderExtensions).GetRuntimeMethod(
        //        nameof(ActianPropertyBuilderExtensions.IsSparse), new[] { typeof(PropertyBuilder), typeof(bool) })!;

        private static readonly MethodInfo PropertyUseIdentityColumnsMethodInfo
            = typeof(ActianPropertyBuilderExtensions).GetRuntimeMethod(
                nameof(ActianPropertyBuilderExtensions.UseIdentityColumn), new[] { typeof(PropertyBuilder), typeof(long), typeof(int) })!;

        private static readonly MethodInfo PropertyUseIdentityByDefaultColumnsMethodInfo
    = typeof(ActianPropertyBuilderExtensions).GetRuntimeMethod(
        nameof(ActianPropertyBuilderExtensions.UseIdentityByDefaultColumn), new[] { typeof(PropertyBuilder), typeof(long), typeof(int) })!;

        private static readonly MethodInfo PropertyUseHiLoMethodInfo
            = typeof(ActianPropertyBuilderExtensions).GetRuntimeMethod(
                nameof(ActianPropertyBuilderExtensions.UseHiLo), new[] { typeof(PropertyBuilder), typeof(string), typeof(string) })!;

        private static readonly MethodInfo PropertyUseSequenceMethodInfo
            = typeof(ActianPropertyBuilderExtensions).GetRuntimeMethod(
                nameof(ActianPropertyBuilderExtensions.UseSequence), new[] { typeof(PropertyBuilder), typeof(string), typeof(string) })!;

        #endregion MethodInfos

        public ActianAnnotationCodeGenerator(AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
            IModel model,
            IDictionary<string, IAnnotation> annotations)
        {
            Check.NotNull(model, nameof(model));

            var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(model, annotations));

            if (GenerateValueGenerationStrategy(annotations, model, onModel: true) is MethodCallCodeFragment valueGenerationStrategy)
            {
                fragments.Add(valueGenerationStrategy);
            }

            return fragments;
        }

        public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
            IProperty property,
            IDictionary<string, IAnnotation> annotations)
        {
            var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(property, annotations));

            if (GenerateValueGenerationStrategy(annotations, property.DeclaringType.Model, onModel: false) is MethodCallCodeFragment
                valueGenerationStrategy)
            {
                fragments.Add(valueGenerationStrategy);
            }

            return fragments;
        }

        public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
            IEntityType entityType,
            IDictionary<string, IAnnotation> annotations)
        {
            var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(entityType, annotations));

            return fragments;
        }

        protected override bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            if (annotation.Name == RelationalAnnotationNames.DefaultSchema)
            {
                return (string?)annotation.Value == "public";
            }

            return annotation.Name == ActianAnnotationNames.ValueGenerationStrategy
                && (ActianValueGenerationStrategy)annotation.Value! == ActianValueGenerationStrategy.IdentityColumn
                && (ActianValueGenerationStrategy)annotation.Value! == ActianValueGenerationStrategy.IdentityByDefaultColumn;
        }

        protected override bool IsHandledByConvention(IProperty property, IAnnotation annotation)
        {
            if (annotation.Name == ActianAnnotationNames.ValueGenerationStrategy)
            {
                return (ActianValueGenerationStrategy)annotation.Value! == property.DeclaringType.Model.GetValueGenerationStrategy();
            }

            return base.IsHandledByConvention(property, annotation);
        }

        private static MethodCallCodeFragment? GenerateValueGenerationStrategy(
            IDictionary<string, IAnnotation> annotations,
            IModel model,
            bool onModel)
        {

            ActianValueGenerationStrategy strategy;
            if (annotations.TryGetValue(ActianAnnotationNames.ValueGenerationStrategy, out var strategyAnnotation)
                && strategyAnnotation.Value != null)
            {
                annotations.Remove(ActianAnnotationNames.ValueGenerationStrategy);
                strategy = (ActianValueGenerationStrategy)strategyAnnotation.Value;
            }
            else
            {
                return null;
            }

            switch (strategy)
            {
                case ActianValueGenerationStrategy.IdentityColumn:
                    // Support pre-6.0 IdentitySeed annotations, which contained an int rather than a long
                    if (annotations.TryGetValue(ActianAnnotationNames.IdentitySeed, out var seedAnnotation)
                        && seedAnnotation.Value != null)
                    {
                        annotations.Remove(ActianAnnotationNames.IdentitySeed);
                    }
                    else
                    {
                        seedAnnotation = model.FindAnnotation(ActianAnnotationNames.IdentitySeed);
                    }

                    var seed = seedAnnotation is null
                        ? 1L
                        : seedAnnotation.Value is int intValue
                            ? intValue
                            : (long?)seedAnnotation.Value ?? 1L;

                    var increment = GetAndRemove<int?>(annotations, ActianAnnotationNames.IdentityIncrement)
                        ?? model.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.Value as int?
                        ?? 1;
                    return new MethodCallCodeFragment(
                        onModel ? ModelUseIdentityColumnsMethodInfo : PropertyUseIdentityColumnsMethodInfo,
                        (seed, increment) switch
                        {
                            (1L, 1) => Array.Empty<object>(),
                            (_, 1) => new object[] { seed },
                            _ => new object[] { seed, increment }
                        });

                case ActianValueGenerationStrategy.IdentityByDefaultColumn:
                    // Support pre-6.0 IdentitySeed annotations, which contained an int rather than a long
                    if (annotations.TryGetValue(ActianAnnotationNames.IdentitySeed, out var _seedAnnotation)
                        && _seedAnnotation.Value != null)
                    {
                        annotations.Remove(ActianAnnotationNames.IdentitySeed);
                    }
                    else
                    {
                        _seedAnnotation = model.FindAnnotation(ActianAnnotationNames.IdentitySeed);
                    }

                    var _seed = _seedAnnotation is null
                        ? 1L
                        : _seedAnnotation.Value is int _intValue
                            ? _intValue
                            : (long?)_seedAnnotation.Value ?? 1L;

                    var _increment = GetAndRemove<int?>(annotations, ActianAnnotationNames.IdentityIncrement)
                        ?? model.FindAnnotation(ActianAnnotationNames.IdentityIncrement)?.Value as int?
                        ?? 1;
                    return new MethodCallCodeFragment(
                        onModel ? ModelUseIdentityByDefaultColumnsMethodInfo : PropertyUseIdentityByDefaultColumnsMethodInfo,
                        (_seed, _increment) switch
                        {
                            (1L, 1) => Array.Empty<object>(),
                            (_, 1) => new object[] { _seed },
                            _ => new object[] { _seed, _increment }
                        });

                case ActianValueGenerationStrategy.SequenceHiLo:
                {
                    var name = GetAndRemove<string>(annotations, ActianAnnotationNames.HiLoSequenceName);
                    var schema = GetAndRemove<string>(annotations, ActianAnnotationNames.HiLoSequenceSchema);
                    return new MethodCallCodeFragment(
                        onModel ? ModelUseHiLoMethodInfo : PropertyUseHiLoMethodInfo,
                        (name, schema) switch
                        {
                            (null, null) => Array.Empty<object>(),
                            (_, null) => new object[] { name },
                            _ => new object[] { name!, schema }
                        });
                }

                case ActianValueGenerationStrategy.Sequence:
                {
                    var nameOrSuffix = GetAndRemove<string>(
                        annotations,
                        onModel ? ActianAnnotationNames.SequenceNameSuffix : ActianAnnotationNames.SequenceName);

                    var schema = GetAndRemove<string>(annotations, ActianAnnotationNames.SequenceSchema);
                    return new MethodCallCodeFragment(
                        onModel ? ModelUseKeySequencesMethodInfo : PropertyUseSequenceMethodInfo,
                        (name: nameOrSuffix, schema) switch
                        {
                            (null, null) => Array.Empty<object>(),
                            (_, null) => new object[] { nameOrSuffix },
                            _ => new object[] { nameOrSuffix!, schema }
                        });
                }

                case ActianValueGenerationStrategy.None:
                    return new MethodCallCodeFragment(
                        ModelHasAnnotationMethodInfo,
                        ActianAnnotationNames.ValueGenerationStrategy,
                        ActianValueGenerationStrategy.None);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static T? GetAndRemove<T>(IDictionary<string, IAnnotation> annotations, string annotationName)
        {
            if (annotations.TryGetValue(annotationName, out var annotation)
                && annotation.Value != null)
            {
                annotations.Remove(annotationName);
                return (T)annotation.Value;
            }

            return default;
        }

        private static void GenerateSimpleFluentApiCall(
            IDictionary<string, IAnnotation> annotations,
            string annotationName,
            MethodInfo methodInfo,
            List<MethodCallCodeFragment> methodCallCodeFragments)
        {
            if (annotations.TryGetValue(annotationName, out var annotation))
            {
                annotations.Remove(annotationName);
                if (annotation.Value is object annotationValue)
                {
                    methodCallCodeFragments.Add(
                        new MethodCallCodeFragment(methodInfo, annotationValue));
                }
            }
        }
    }
}
