// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Actian.EFCore
{
    public class SpatialActianFixture : SpatialFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => ActianTestStoreFactory.Instance;

        //protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
        //    => base.AddServices(serviceCollection)
        //        .AddEntityFrameworkActianNetTopologySuite();

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            //new ActianDbContextOptionsBuilder(optionsBuilder).UseNetTopologySuite();

            return optionsBuilder;
        }

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
                });
            modelBuilder.Entity<PolygonEntity>().Property(e => e.Polygon).HasColumnType("geometry");
            modelBuilder.Entity<GeoPointEntity>().Property(e => e.Location).HasColumnType("geometry");
        }
    }
}
