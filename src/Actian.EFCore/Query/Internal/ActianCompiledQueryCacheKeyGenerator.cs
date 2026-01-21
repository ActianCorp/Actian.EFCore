// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Linq.Expressions;
using System;
using Microsoft.EntityFrameworkCore.Query;
using Actian.EFCore.Storage.Internal;

#nullable enable

namespace Actian.EFCore.Query.Internal
{
    public class ActianCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
    {
        private readonly IActianConnection _actianConnection;

        public ActianCompiledQueryCacheKeyGenerator(
            CompiledQueryCacheKeyGeneratorDependencies dependencies,
            RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies,
            IActianConnection actianConnection)
            : base(dependencies, relationalDependencies)
        {
            _actianConnection = actianConnection;
        }

        public override object GenerateCacheKey(Expression query, bool async)
            => new ActianCompiledQueryCacheKey(
                GenerateCacheKeyCore(query, async),
                _actianConnection.IsMultipleActiveResultSetsEnabled);

        private readonly struct ActianCompiledQueryCacheKey : IEquatable<ActianCompiledQueryCacheKey>
        {
            private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;
            private readonly bool _multipleActiveResultSetsEnabled;

            public ActianCompiledQueryCacheKey(
                RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey,
                bool multipleActiveResultSetsEnabled)
            {
                _relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
                _multipleActiveResultSetsEnabled = multipleActiveResultSetsEnabled;
            }

            public override bool Equals(object? obj)
                => obj is ActianCompiledQueryCacheKey actianCompiledQueryCacheKey
                    && Equals(actianCompiledQueryCacheKey);

            public bool Equals(ActianCompiledQueryCacheKey other)
                => _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey)
                    && _multipleActiveResultSetsEnabled == other._multipleActiveResultSetsEnabled;

            public override int GetHashCode()
                => HashCode.Combine(_relationalCompiledQueryCacheKey, _multipleActiveResultSetsEnabled);
        }
    }
}
