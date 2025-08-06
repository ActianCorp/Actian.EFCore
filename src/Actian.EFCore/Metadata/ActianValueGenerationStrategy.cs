// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Defines two strategies to use across the EF Core stack when generating key values
    /// from Actian database columns.
    /// </summary>
    public enum ActianValueGenerationStrategy
    {
        /// <summary>
        /// No Actian-specific strategy
        /// </summary>
        None,

        /// <summary>
        /// <para>
        ///     A sequence-based hi-lo pattern where blocks of IDs are allocated from the server and
        ///     used client-side for generating keys.
        /// </para>
        /// <para>
        ///     This is an advanced pattern--only use this strategy if you are certain it is what you need.
        /// </para>
        /// </summary>
        SequenceHiLo,

        /// <summary>
        /// <para>Selects the always-identity column strategy (a value cannot be provided).</para>
        /// </summary>
        IdentityAlwaysColumn,

        /// <summary>
        /// <para>Selects the by-default-identity column strategy (a value can be provided to override the identity mechanism).</para>
        /// </summary>
        IdentityByDefaultColumn,

        /// <summary>
        ///     A pattern that uses a normal <c>Identity</c> column in the same way as EF6 and earlier.
        /// </summary>
        IdentityColumn,

        /// <summary>
        ///     A pattern that uses a database sequence to generate values for the column.
        /// </summary>
        Sequence
    }

    public static class ActianValueGenerationStrategyExtensions
    {
        public static bool IsIdentity(this ActianValueGenerationStrategy strategy)
            => strategy == ActianValueGenerationStrategy.IdentityByDefaultColumn ||
               strategy == ActianValueGenerationStrategy.IdentityAlwaysColumn;

        public static bool IsIdentity(this ActianValueGenerationStrategy? strategy)
            => strategy == ActianValueGenerationStrategy.IdentityByDefaultColumn ||
               strategy == ActianValueGenerationStrategy.IdentityAlwaysColumn;
    }
}
