// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Actian.EFCore.ValueGeneration.Internal
{
    public class ActianSequenceValueGeneratorState : HiLoValueGeneratorState
    {
        public ActianSequenceValueGeneratorState([NotNull] ISequence sequence)
            : base(Check.NotNull(sequence, nameof(sequence)).IncrementBy)
        {
            Sequence = sequence;
        }

        public virtual ISequence Sequence { get; }
    }
}
