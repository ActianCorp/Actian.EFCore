// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Actian.EFCore.ValueGeneration.Internal
{
    public class ActianValueGeneratorCache : ValueGeneratorCache, IActianValueGeneratorCache
    {
        private readonly ConcurrentDictionary<string, ActianSequenceValueGeneratorState> _sequenceGeneratorCache
            = new ConcurrentDictionary<string, ActianSequenceValueGeneratorState>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueGeneratorCache" /> class.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        public ActianValueGeneratorCache([NotNull] ValueGeneratorCacheDependencies dependencies)
            : base(dependencies)
        {
        }

        public virtual ActianSequenceValueGeneratorState GetOrAddSequenceState(
            IProperty property,
            IRelationalConnection connection)
        {
            var sequence = property.FindHiLoSequence();

            Debug.Assert(sequence != null);

            return _sequenceGeneratorCache.GetOrAdd(
                GetSequenceName(sequence, connection),
                sequenceName => new ActianSequenceValueGeneratorState(sequence));
        }

        private static string GetSequenceName(ISequence sequence, IRelationalConnection connection)
        {
            var dbConnection = connection.DbConnection;

            return dbConnection.Database.ToUpperInvariant()
                + "::"
                + dbConnection.DataSource?.ToUpperInvariant()
                + "::"
                + (sequence.Schema == null ? "" : sequence.Schema + ".")
                + sequence.Name;
        }
    }
}
