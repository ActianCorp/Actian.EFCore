// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Threading;
using Actian.EFCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Actian.EFCore.Query
{
    public class SpatialQueryActianGeographyFixture : SpatialQueryActianFixture
    {
        private NtsGeometryServices _geometryServices;
        private GeometryFactory _geometryFactory;

        public NtsGeometryServices GeometryServices
            => LazyInitializer.EnsureInitialized(
                ref _geometryServices,
                () => CreateGeometryServices());

        protected static NtsGeometryServices CreateGeometryServices()
            => new NtsGeometryServices(
                NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory,
                NtsGeometryServices.Instance.DefaultPrecisionModel,
                4326);

        public override GeometryFactory GeometryFactory
            => LazyInitializer.EnsureInitialized(
                ref _geometryFactory,
                () => GeometryServices.CreateGeometryFactory());

        protected override string StoreName
            => "SpatialQueryGeographyTest";

        protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            => base.AddServices(serviceCollection.AddSingleton(GeometryServices))
                .AddSingleton<IRelationalTypeMappingSource, ReplacementTypeMappingSource>();

        protected class ReplacementTypeMappingSource : ActianTypeMappingSource
        {
            public ReplacementTypeMappingSource(
                TypeMappingSourceDependencies dependencies,
                RelationalTypeMappingSourceDependencies relationalDependencies)
                : base(dependencies, relationalDependencies)
            {
            }
        }
    }
}
