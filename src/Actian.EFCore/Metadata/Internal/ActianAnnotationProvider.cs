// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Actian.EFCore.Metadata.Internal;

public class ActianAnnotationProvider : RelationalAnnotationProvider
{
    /// <summary>
    ///     Initializes a new instance of this class.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
    public ActianAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
    {
        if (!designTime)
        {
            yield break;
        }

        var table = StoreObjectIdentifier.Table(column.Table.Name, column.Table.Schema);

        var identityProperty = column.PropertyMappings
            .Select(m => m.Property)
            .FirstOrDefault(
                p =>
                {
                    var strategy = p.GetValueGenerationStrategy(table);
                    return strategy == ActianValueGenerationStrategy.IdentityColumn ||
                           strategy == ActianValueGenerationStrategy.IdentityByDefaultColumn;
                });

        if (identityProperty != null)
        {
            var seed = identityProperty.GetIdentitySeed(table);
            var increment = identityProperty.GetIdentityIncrement(table);

            yield return new Annotation(
                ActianAnnotationNames.Identity,
                string.Format(CultureInfo.InvariantCulture, "{0}, {1}", seed ?? 1, increment ?? 1));
        }
    }
}
