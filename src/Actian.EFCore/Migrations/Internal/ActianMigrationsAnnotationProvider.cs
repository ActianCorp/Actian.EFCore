// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using Actian.EFCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Actian.EFCore.Migrations.Internal
{
    public class ActianMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        public ActianMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
#pragma warning restore EF1001 // Internal EF Core API usage.
            : base(dependencies)
        {
        }

        public IEnumerable<IAnnotation> For(IProperty property)
        {
            var valueGenerationStrategy = property.GetValueGenerationStrategy();
            if (valueGenerationStrategy != ActianValueGenerationStrategy.None)
            {
                yield return new Annotation(ActianAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy);

                if (valueGenerationStrategy.IsIdentity())
                {
                    if (property[ActianAnnotationNames.IdentityOptions] is string identityOptions)
                    {
                        yield return new Annotation(ActianAnnotationNames.IdentityOptions, identityOptions);
                    }
                }
            }
        }
    }
}
