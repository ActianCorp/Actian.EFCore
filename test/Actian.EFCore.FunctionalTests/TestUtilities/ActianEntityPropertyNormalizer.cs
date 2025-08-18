// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Actian.EFCore.TestUtilities
{
    internal class ActianEntityPropertyNormalizer<T>
    {
        private readonly ImmutableList<Func<IMutableProperty, bool>> _predicates = ImmutableList<Func<IMutableProperty, bool>>.Empty;
        private readonly ImmutableList<Action<IMutableProperty>> _normalizers = ImmutableList<Action<IMutableProperty>>.Empty;

        internal ActianEntityPropertyNormalizer()
        {
            _predicates = _predicates.Add(property => property.ClrType == typeof(T));
        }

        private ActianEntityPropertyNormalizer(ImmutableList<Func<IMutableProperty, bool>> predicates, ImmutableList<Action<IMutableProperty>> normalizers)
        {
            _predicates = predicates ?? throw new ArgumentNullException(nameof(predicates));
            _normalizers = normalizers ?? throw new ArgumentNullException(nameof(normalizers));
        }

        public ActianEntityPropertyNormalizer<T> Where(Func<IMutableProperty, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return new ActianEntityPropertyNormalizer<T>(
                _predicates.Add(predicate),
                _normalizers
            );
        }

        public ActianEntityPropertyNormalizer<T> Except(Func<IMutableProperty, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return Where(property => !predicate(property));
        }

        public ActianEntityPropertyNormalizer<T> WhereHasNoColumnType()
        {
            return Where(property => ActianModelTestHelpers.GetColumnType(property) is null);
        }

        public ActianEntityPropertyNormalizer<T> AddNormalizer(Action<IMutableProperty> normalizer)
        {
            if (normalizer is null)
                throw new ArgumentNullException(nameof(normalizer));

            return new ActianEntityPropertyNormalizer<T>(
                _predicates,
                _normalizers.Add(normalizer)
            );
        }

        public ActianEntityPropertyNormalizer<T> SetColumnType(string columnType)
        {
            return AddNormalizer(property => ActianModelTestHelpers.SetColumnType(property, columnType));
        }

        public ActianEntityPropertyNormalizer<T> SetMaxLength(int? maxLength)
        {
            return AddNormalizer(property => property.SetMaxLength(maxLength));
        }

        public ActianEntityPropertyNormalizer<T> SetValueConverter(ValueConverter converter)
        {
            return AddNormalizer(property => property.SetValueConverter(converter));
        }

        public void Normalize(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                Normalize(entity);
            }
        }

        private void Normalize(IMutableEntityType entity)
        {
            foreach (var property in entity.GetProperties().Where(ShouldNormalize))
            {
                Normalize(property);
            }
        }

        private bool ShouldNormalize(IMutableProperty property)
        {
            foreach (var predicate in _predicates)
            {
                if (!predicate(property))
                    return false;
            }
            return true;
        }

        private void Normalize(IMutableProperty property)
        {
            foreach (var normalizer in _normalizers)
            {
                normalizer(property);
            }
        }
    }
}
