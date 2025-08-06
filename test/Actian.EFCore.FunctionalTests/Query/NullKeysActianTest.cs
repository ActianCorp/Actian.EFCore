// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Actian.EFCore.Query
{
    public class NullKeysActianTest : NullKeysTestBase<NullKeysActianTest.NullKeysActianFixture>
    {
        public NullKeysActianTest(NullKeysActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture.ListLoggerFactory, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();

        public override void Include_with_null_FKs_and_nullable_PK()
        {
            base.Include_with_null_FKs_and_nullable_PK();
        }
        
        public override void Include_with_non_nullable_FKs_and_nullable_PK()
        {
            base.Include_with_non_nullable_FKs_and_nullable_PK();
        }
        
        public override void Include_with_null_fKs_and_non_nullable_PK()
        {
            base.Include_with_null_fKs_and_non_nullable_PK();
        }
        
        public override void Include_with_null_fKs_and_nullable_PK()
        {
            base.Include_with_null_fKs_and_nullable_PK();
        }
        
        public override void One_to_one_self_ref_Include()
        {
            base.One_to_one_self_ref_Include();
        }
        
        public class NullKeysActianFixture : NullKeysFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                ActianModelTestHelpers.MaxLengthStringKeys
                    .Normalize(modelBuilder);

                ActianModelTestHelpers.Guids
                    .Normalize(modelBuilder);
            }
        }
    }
}
