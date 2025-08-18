// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using NetTopologySuite.Geometries;

namespace Actian.EFCore.Query
{
    public class SpatialQueryActianGeometryTest : SpatialQueryActianFixture
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<LineStringEntity>().Property(e => e.LineString).HasColumnType("geometry");
            modelBuilder.Entity<MultiLineStringEntity>().Property(e => e.MultiLineString).HasColumnType("geometry");
            modelBuilder.Entity<PointEntity>(
                x =>
                {
                    x.Property(e => e.Geometry).HasColumnType("geometry");
                    x.Property(e => e.Point).HasColumnType("geometry");
                    x.Property(e => e.PointZ).HasColumnType("geometry");
                    x.Property(e => e.PointM).HasColumnType("geometry");
                    x.Property(e => e.PointZM).HasColumnType("geometry");
                });
            modelBuilder.Entity<PolygonEntity>().Property(e => e.Polygon).HasColumnType("geometry");
            modelBuilder.Entity<GeoPointEntity>().Property(e => e.Location).HasColumnType("geometry");
        }


        protected class ReplacementTypeMappingSource : ActianTypeMappingSource
        {
            public ReplacementTypeMappingSource(
                TypeMappingSourceDependencies dependencies,
                RelationalTypeMappingSourceDependencies relationalDependencies)
                : base(dependencies, relationalDependencies)
            {
            }

            protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
                => mappingInfo.ClrType == typeof(GeoPoint)
                    ? ((RelationalTypeMapping)base.FindMapping(typeof(Point))
                        .WithComposedConverter(new GeoPointConverter())).WithStoreTypeAndSize("geometry", null)
                    : base.FindMapping(mappingInfo);
        }
    }
}
