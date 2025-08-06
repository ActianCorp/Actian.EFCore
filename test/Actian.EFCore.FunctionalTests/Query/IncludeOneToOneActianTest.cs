// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;
using TestEnvironment = Actian.EFCore.TestUtilities.TestEnvironment;

namespace Actian.EFCore.Query
{
    public class IncludeOneToOneActianTest : IncludeOneToOneTestBase<IncludeOneToOneActianTest.OneToOneQueryActianFixture>
    {
        public IncludeOneToOneActianTest(OneToOneQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();

        public override void Include_address()
        {
            base.Include_address();
            AssertSql(@"
                SELECT ""p"".""Id"", ""p"".""Name"", ""a"".""Id"", ""a"".""City"", ""a"".""Street""
                FROM ""Person"" AS ""p""
                LEFT JOIN ""Address"" AS ""a"" ON ""p"".""Id"" = ""a"".""Id""
            ");
        }

        public override void Include_address_shadow()
        {
            base.Include_address_shadow();
            AssertSql(@"
                SELECT ""p"".""Id"", ""p"".""Name"", ""a"".""Id"", ""a"".""City"", ""a"".""PersonId"", ""a"".""Street""
                FROM ""Person2"" AS ""p""
                LEFT JOIN ""Address2"" AS ""a"" ON ""p"".""Id"" = ""a"".""PersonId""
            ");
        }

        public override void Include_address_no_tracking()
        {
            base.Include_address_no_tracking();
        }

        public override void Include_person()
        {
            base.Include_person();
            AssertSql(@"
                SELECT ""a"".""Id"", ""a"".""City"", ""a"".""Street"", ""p"".""Id"", ""p"".""Name""
                FROM ""Address"" AS ""a""
                INNER JOIN ""Person"" AS ""p"" ON ""a"".""Id"" = ""p"".""Id""
            ");
        }

        public override void Include_person_shadow()
        {
            base.Include_person_shadow();
            AssertSql(@"
                SELECT ""a"".""Id"", ""a"".""City"", ""a"".""PersonId"", ""a"".""Street"", ""p"".""Id"", ""p"".""Name""
                FROM ""Address2"" AS ""a""
                INNER JOIN ""Person2"" AS ""p"" ON ""a"".""PersonId"" = ""p"".""Id""
            ");
        }

        public override void Include_person_no_tracking()
        {
            base.Include_person_no_tracking();
        }

        public override void Include_address_when_person_already_tracked()
        {
            base.Include_address_when_person_already_tracked();
        }

        public override void Include_person_when_address_already_tracked()
        {
            base.Include_person_when_address_already_tracked();
        }

        public class OneToOneQueryActianFixture : OneToOneQueryFixtureBase, IActianSqlFixture
        {
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

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
