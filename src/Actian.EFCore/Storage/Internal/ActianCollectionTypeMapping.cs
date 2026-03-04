// Copyright (c) 2026 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    /// <summary>
    ///     A placeholder type mapping for collection parameters (e.g. <c>ids.Contains(x)</c>).
    ///     Actian/Ingres does not support JSON-based collection parameters, so EF Core 10's
    ///     <see cref="ParameterTranslationMode.MultipleParameters"/> mode decomposes the
    ///     collection into individual scalar parameters before SQL generation. This mapping
    ///     only exists to satisfy the type-mapping metadata chain; its store type is never
    ///     emitted to the database.
    /// </summary>
    public class ActianCollectionTypeMapping : RelationalTypeMapping
    {
        private readonly CoreTypeMapping _elementMapping;

        public ActianCollectionTypeMapping(Type clrType, CoreTypeMapping elementMapping)
            : base(new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(clrType, elementMapping: elementMapping),
                "COLLECTION_VOID"))
        {
            _elementMapping = elementMapping;
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianCollectionTypeMapping(parameters.CoreParameters.ClrType, _elementMapping);
    }
}
