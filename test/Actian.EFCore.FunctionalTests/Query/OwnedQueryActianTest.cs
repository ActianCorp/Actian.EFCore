﻿using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;
using static Actian.EFCore.TestUtilities.ActianSkipReasons;

namespace Actian.EFCore.Query
{
    public class OwnedQueryActianTest : RelationalOwnedQueryTestBase<OwnedQueryActianTest.OwnedQueryActianFixture>
    {
        public OwnedQueryActianTest(OwnedQueryActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();

        public override async Task Query_with_owned_entity_equality_operator(bool isAsync)
        {
            await base.Query_with_owned_entity_equality_operator(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o1"".""Id"", ""t0"".""Id"", ""t0"".""PersonAddress_Country_Name"", ""t0"".""PersonAddress_Country_PlanetId"", ""t2"".""Id"", ""t3"".""Id"", ""t3"".""BranchAddress_Country_Name"", ""t3"".""BranchAddress_Country_PlanetId"", ""t5"".""Id"", ""t6"".""Id"", ""t6"".""LeafAAddress_Country_Name"", ""t6"".""LeafAAddress_Country_PlanetId"", ""t"".""Id"", ""o9"".""ClientId"", ""o9"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                CROSS JOIN (
                    SELECT ""o0"".""Id"", ""o0"".""Discriminator""
                    FROM ""OwnedPerson"" AS ""o0""
                    WHERE ""o0"".""Discriminator"" = N'LeafB'
                ) AS ""t""
                LEFT JOIN ""OwnedPerson"" AS ""o1"" ON ""o"".""Id"" = ""o1"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""o2"".""PersonAddress_Country_Name"", ""o2"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o2""
                    WHERE ""o2"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t0"" ON ""o1"".""Id"" = ""t0"".""Id""
                LEFT JOIN (
                    SELECT ""o3"".""Id"", ""t1"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o3""
                    INNER JOIN (
                        SELECT ""o4"".""Id"", ""o4"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o4""
                        WHERE ""o4"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t1"" ON ""o3"".""Id"" = ""t1"".""Id""
                ) AS ""t2"" ON ""o"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""o5"".""BranchAddress_Country_Name"", ""o5"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o5""
                    WHERE ""o5"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t3"" ON ""t2"".""Id"" = ""t3"".""Id""
                LEFT JOIN (
                    SELECT ""o6"".""Id"", ""t4"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o6""
                    INNER JOIN (
                        SELECT ""o7"".""Id"", ""o7"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o7""
                        WHERE ""o7"".""Discriminator"" = N'LeafA'
                    ) AS ""t4"" ON ""o6"".""Id"" = ""t4"".""Id""
                ) AS ""t5"" ON ""o"".""Id"" = ""t5"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""o8"".""LeafAAddress_Country_Name"", ""o8"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o8""
                    WHERE ""o8"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t6"" ON ""t5"".""Id"" = ""t6"".""Id""
                LEFT JOIN ""Order"" AS ""o9"" ON ""o"".""Id"" = ""o9"".""ClientId""
                WHERE FALSE = TRUE
                ORDER BY ""o"".""Id"", ""t"".""Id"", ""o9"".""ClientId"", ""o9"".""Id""
            ");
        }

        public override Task Query_with_owned_entity_equality_method(bool isAsync)
        {
            return base.Query_with_owned_entity_equality_method(isAsync);
        }

        public override Task Query_with_owned_entity_equality_object_method(bool isAsync)
        {
            return base.Query_with_owned_entity_equality_object_method(isAsync);
        }

        public override async Task Query_for_base_type_loads_all_owned_navs(bool isAsync)
        {
            await base.Query_for_base_type_loads_all_owned_navs(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafBAddress_Country_Name"", ""t5"".""LeafBAddress_Country_PlanetId"", ""t7"".""Id"", ""t8"".""Id"", ""t8"".""LeafAAddress_Country_Name"", ""t8"".""LeafAAddress_Country_PlanetId"", ""o11"".""ClientId"", ""o11"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafB'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafBAddress_Country_Name"", ""o7"".""LeafBAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""t6"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o8""
                    INNER JOIN (
                        SELECT ""o9"".""Id"", ""o9"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o9""
                        WHERE ""o9"".""Discriminator"" = N'LeafA'
                    ) AS ""t6"" ON ""o8"".""Id"" = ""t6"".""Id""
                ) AS ""t7"" ON ""o"".""Id"" = ""t7"".""Id""
                LEFT JOIN (
                    SELECT ""o10"".""Id"", ""o10"".""LeafAAddress_Country_Name"", ""o10"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o10""
                    WHERE ""o10"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t8"" ON ""t7"".""Id"" = ""t8"".""Id""
                LEFT JOIN ""Order"" AS ""o11"" ON ""o"".""Id"" = ""o11"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o"".""Id"", ""o11"".""ClientId"", ""o11"".""Id""
            ");
        }

        public override async Task No_ignored_include_warning_when_implicit_load(bool isAsync)
        {
            await base.No_ignored_include_warning_when_implicit_load(isAsync);
            AssertSql(@"
                SELECT COUNT(*)
                FROM ""OwnedPerson"" AS ""o""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task Query_for_branch_type_loads_all_owned_navs(bool isAsync)
        {
            await base.Query_for_branch_type_loads_all_owned_navs(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafAAddress_Country_Name"", ""t5"".""LeafAAddress_Country_PlanetId"", ""o8"".""ClientId"", ""o8"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafA'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafAAddress_Country_Name"", ""o7"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN ""Order"" AS ""o8"" ON ""o"".""Id"" = ""o8"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'Branch', N'LeafA')
                ORDER BY ""o"".""Id"", ""o8"".""ClientId"", ""o8"".""Id""
            ");
        }

        public override async Task Query_for_branch_type_loads_all_owned_navs_tracking(bool isAsync)
        {
            await base.Query_for_branch_type_loads_all_owned_navs_tracking(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafAAddress_Country_Name"", ""t5"".""LeafAAddress_Country_PlanetId"", ""o8"".""ClientId"", ""o8"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafA'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafAAddress_Country_Name"", ""o7"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN ""Order"" AS ""o8"" ON ""o"".""Id"" = ""o8"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'Branch', N'LeafA')
                ORDER BY ""o"".""Id"", ""o8"".""ClientId"", ""o8"".""Id""
            ");
        }

        [ActianTodo]
        public override async Task Query_for_leaf_type_loads_all_owned_navs(bool isAsync)
        {
            await base.Query_for_leaf_type_loads_all_owned_navs(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafAAddress_Country_Name"", ""t5"".""LeafAAddress_Country_PlanetId"", ""o8"".""ClientId"", ""o8"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafA'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafAAddress_Country_Name"", ""o7"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN ""Order"" AS ""o8"" ON ""o"".""Id"" = ""o8"".""ClientId""
                WHERE ""o"".""Discriminator"" = N'LeafA'
                ORDER BY ""o"".""Id"", ""o8"".""ClientId"", ""o8"".""Id""
            ");
        }

        [ActianTodo]
        public override async Task Query_when_group_by(bool isAsync)
        {
            await base.Query_when_group_by(isAsync);
            AssertSql(
                @"
                    SELECT ""op"".""Id"", ""op"".""Discriminator"", ""t"".""Id"", ""t0"".""Id"", ""t0"".""LeafBAddress_Country_Name"", ""t0"".""LeafBAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""LeafAAddress_Country_Name"", ""t2"".""LeafAAddress_Country_PlanetId"", ""t3"".""Id"", ""t4"".""Id"", ""t4"".""BranchAddress_Country_Name"", ""t4"".""BranchAddress_Country_PlanetId"", ""t5"".""Id"", ""t6"".""Id"", ""t6"".""PersonAddress_Country_Name"", ""t6"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""op""
                    LEFT JOIN (
                        SELECT ""op.LeafBAddress"".*
                        FROM ""OwnedPerson"" AS ""op.LeafBAddress""
                        WHERE ""op.LeafBAddress"".""Discriminator"" = N'LeafB'
                    ) AS ""t"" ON ""op"".""Id"" = ""t"".""Id""
                    LEFT JOIN (
                        SELECT ""op.LeafBAddress.Country"".*
                        FROM ""OwnedPerson"" AS ""op.LeafBAddress.Country""
                        WHERE ""op.LeafBAddress.Country"".""Discriminator"" = N'LeafB'
                    ) AS ""t0"" ON ""t"".""Id"" = ""t0"".""Id""
                    LEFT JOIN (
                        SELECT ""op.LeafAAddress"".*
                        FROM ""OwnedPerson"" AS ""op.LeafAAddress""
                        WHERE ""op.LeafAAddress"".""Discriminator"" = N'LeafA'
                    ) AS ""t1"" ON ""op"".""Id"" = ""t1"".""Id""
                    LEFT JOIN (
                        SELECT ""op.LeafAAddress.Country"".*
                        FROM ""OwnedPerson"" AS ""op.LeafAAddress.Country""
                        WHERE ""op.LeafAAddress.Country"".""Discriminator"" = N'LeafA'
                    ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                    LEFT JOIN (
                        SELECT ""op.BranchAddress"".*
                        FROM ""OwnedPerson"" AS ""op.BranchAddress""
                        WHERE ""op.BranchAddress"".""Discriminator"" IN (N'LeafA', N'Branch')
                    ) AS ""t3"" ON ""op"".""Id"" = ""t3"".""Id""
                    LEFT JOIN (
                        SELECT ""op.BranchAddress.Country"".*
                        FROM ""OwnedPerson"" AS ""op.BranchAddress.Country""
                        WHERE ""op.BranchAddress.Country"".""Discriminator"" IN (N'LeafA', N'Branch')
                    ) AS ""t4"" ON ""t3"".""Id"" = ""t4"".""Id""
                    LEFT JOIN (
                        SELECT ""op.PersonAddress"".*
                        FROM ""OwnedPerson"" AS ""op.PersonAddress""
                        WHERE ""op.PersonAddress"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                    ) AS ""t5"" ON ""op"".""Id"" = ""t5"".""Id""
                    LEFT JOIN (
                        SELECT ""op.PersonAddress.Country"".*
                        FROM ""OwnedPerson"" AS ""op.PersonAddress.Country""
                        WHERE ""op.PersonAddress.Country"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                    ) AS ""t6"" ON ""t5"".""Id"" = ""t6"".""Id""
                    WHERE ""op"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                    ORDER BY ""op"".""Id""
                ",
                @"
                    SELECT ""op.Orders"".""Id"", ""op.Orders"".""ClientId""
                    FROM ""Order"" AS ""op.Orders""
                    INNER JOIN (
                        SELECT DISTINCT ""op0"".""Id""
                        FROM ""OwnedPerson"" AS ""op0""
                        LEFT JOIN (
                            SELECT ""op.LeafBAddress0"".*
                            FROM ""OwnedPerson"" AS ""op.LeafBAddress0""
                            WHERE ""op.LeafBAddress0"".""Discriminator"" = N'LeafB'
                        ) AS ""t7"" ON ""op0"".""Id"" = ""t7"".""Id""
                        LEFT JOIN (
                            SELECT ""op.LeafBAddress.Country0"".*
                            FROM ""OwnedPerson"" AS ""op.LeafBAddress.Country0""
                            WHERE ""op.LeafBAddress.Country0"".""Discriminator"" = N'LeafB'
                        ) AS ""t8"" ON ""t7"".""Id"" = ""t8"".""Id""
                        LEFT JOIN (
                            SELECT ""op.LeafAAddress0"".*
                            FROM ""OwnedPerson"" AS ""op.LeafAAddress0""
                            WHERE ""op.LeafAAddress0"".""Discriminator"" = N'LeafA'
                        ) AS ""t9"" ON ""op0"".""Id"" = ""t9"".""Id""
                        LEFT JOIN (
                            SELECT ""op.LeafAAddress.Country0"".*
                            FROM ""OwnedPerson"" AS ""op.LeafAAddress.Country0""
                            WHERE ""op.LeafAAddress.Country0"".""Discriminator"" = N'LeafA'
                        ) AS ""t10"" ON ""t9"".""Id"" = ""t10"".""Id""
                        LEFT JOIN (
                            SELECT ""op.BranchAddress0"".*
                            FROM ""OwnedPerson"" AS ""op.BranchAddress0""
                            WHERE ""op.BranchAddress0"".""Discriminator"" IN (N'LeafA', N'Branch')
                        ) AS ""t11"" ON ""op0"".""Id"" = ""t11"".""Id""
                        LEFT JOIN (
                            SELECT ""op.BranchAddress.Country0"".*
                            FROM ""OwnedPerson"" AS ""op.BranchAddress.Country0""
                            WHERE ""op.BranchAddress.Country0"".""Discriminator"" IN (N'LeafA', N'Branch')
                        ) AS ""t12"" ON ""t11"".""Id"" = ""t12"".""Id""
                        LEFT JOIN (
                            SELECT ""op.PersonAddress0"".*
                            FROM ""OwnedPerson"" AS ""op.PersonAddress0""
                            WHERE ""op.PersonAddress0"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                        ) AS ""t13"" ON ""op0"".""Id"" = ""t13"".""Id""
                        LEFT JOIN (
                            SELECT ""op.PersonAddress.Country0"".*
                            FROM ""OwnedPerson"" AS ""op.PersonAddress.Country0""
                            WHERE ""op.PersonAddress.Country0"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                        ) AS ""t14"" ON ""t13"".""Id"" = ""t14"".""Id""
                        WHERE ""op0"".""Discriminator"" IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
                    ) AS ""t15"" ON ""op.Orders"".""ClientId"" = ""t15"".""Id""
                    ORDER BY ""t15"".""Id""
                "
            );
        }

        [ActianSkip(NoOrderByOffsetFirstAndFetchInSubselects)]
        public override async Task Query_when_subquery(bool isAsync)
        {
            await base.Query_when_subquery(isAsync);
            AssertSql(@"
                @__p_0='5'
                
                SELECT ""t0"".""Id"", ""t0"".""Discriminator"", ""o1"".""Id"", ""t1"".""Id"", ""t1"".""PersonAddress_Country_Name"", ""t1"".""PersonAddress_Country_PlanetId"", ""t3"".""Id"", ""t4"".""Id"", ""t4"".""BranchAddress_Country_Name"", ""t4"".""BranchAddress_Country_PlanetId"", ""t6"".""Id"", ""t7"".""Id"", ""t7"".""LeafBAddress_Country_Name"", ""t7"".""LeafBAddress_Country_PlanetId"", ""t9"".""Id"", ""t10"".""Id"", ""t10"".""LeafAAddress_Country_Name"", ""t10"".""LeafAAddress_Country_PlanetId"", ""o12"".""ClientId"", ""o12"".""Id""
                FROM (
                    SELECT FIRST @__p_0 ""t"".""Id"", ""t"".""Discriminator""
                    FROM (
                        SELECT DISTINCT ""o"".""Id"", ""o"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o""
                        WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                    ) AS ""t""
                    ORDER BY ""t"".""Id""
                ) AS ""t0""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""t0"".""Id"" = ""o0"".""Id""
                LEFT JOIN ""OwnedPerson"" AS ""o1"" ON ""t0"".""Id"" = ""o1"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""o2"".""PersonAddress_Country_Name"", ""o2"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o2""
                    WHERE ""o2"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t1"" ON ""o1"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o3"".""Id"", ""t2"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o3""
                    INNER JOIN (
                        SELECT ""o4"".""Id"", ""o4"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o4""
                        WHERE ""o4"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t2"" ON ""o3"".""Id"" = ""t2"".""Id""
                ) AS ""t3"" ON ""t0"".""Id"" = ""t3"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""o5"".""BranchAddress_Country_Name"", ""o5"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o5""
                    WHERE ""o5"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t4"" ON ""t3"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o6"".""Id"", ""t5"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o6""
                    INNER JOIN (
                        SELECT ""o7"".""Id"", ""o7"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o7""
                        WHERE ""o7"".""Discriminator"" = N'LeafB'
                    ) AS ""t5"" ON ""o6"".""Id"" = ""t5"".""Id""
                ) AS ""t6"" ON ""t0"".""Id"" = ""t6"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""o8"".""LeafBAddress_Country_Name"", ""o8"".""LeafBAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o8""
                    WHERE ""o8"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t7"" ON ""t6"".""Id"" = ""t7"".""Id""
                LEFT JOIN (
                    SELECT ""o9"".""Id"", ""t8"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o9""
                    INNER JOIN (
                        SELECT ""o10"".""Id"", ""o10"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o10""
                        WHERE ""o10"".""Discriminator"" = N'LeafA'
                    ) AS ""t8"" ON ""o9"".""Id"" = ""t8"".""Id""
                ) AS ""t9"" ON ""t0"".""Id"" = ""t9"".""Id""
                LEFT JOIN (
                    SELECT ""o11"".""Id"", ""o11"".""LeafAAddress_Country_Name"", ""o11"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o11""
                    WHERE ""o11"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t10"" ON ""t9"".""Id"" = ""t10"".""Id""
                LEFT JOIN ""Order"" AS ""o12"" ON ""t0"".""Id"" = ""o12"".""ClientId""
                ORDER BY ""t0"".""Id"", ""o12"".""ClientId"", ""o12"".""Id""
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_projecting_scalar(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_projecting_scalar(isAsync);
            AssertSql(@"
                SELECT ""t"".""PersonAddress_Country_Name""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (""t"".""PersonAddress_Country_Name"" = N'USA')
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_projecting_entity(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_projecting_entity(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafBAddress_Country_Name"", ""t5"".""LeafBAddress_Country_PlanetId"", ""t7"".""Id"", ""t8"".""Id"", ""t8"".""LeafAAddress_Country_Name"", ""t8"".""LeafAAddress_Country_PlanetId"", ""o11"".""ClientId"", ""o11"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafB'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafBAddress_Country_Name"", ""o7"".""LeafBAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""t6"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o8""
                    INNER JOIN (
                        SELECT ""o9"".""Id"", ""o9"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o9""
                        WHERE ""o9"".""Discriminator"" = N'LeafA'
                    ) AS ""t6"" ON ""o8"".""Id"" = ""t6"".""Id""
                ) AS ""t7"" ON ""o"".""Id"" = ""t7"".""Id""
                LEFT JOIN (
                    SELECT ""o10"".""Id"", ""o10"".""LeafAAddress_Country_Name"", ""o10"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o10""
                    WHERE ""o10"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t8"" ON ""t7"".""Id"" = ""t8"".""Id""
                LEFT JOIN ""Order"" AS ""o11"" ON ""o"".""Id"" = ""o11"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (""t"".""PersonAddress_Country_Name"" = N'USA')
                ORDER BY ""o"".""Id"", ""o11"".""ClientId"", ""o11"".""Id""
            ");
        }

        public override async Task Navigation_rewrite_on_owned_collection(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_collection(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o0"".""ClientId"", ""o0"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""Order"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND ((
                    SELECT COUNT(*)
                    FROM ""Order"" AS ""o1""
                    WHERE ""o"".""Id"" = ""o1"".""ClientId"") > 0)
                ORDER BY ""o"".""Id"", ""o0"".""ClientId"", ""o0"".""Id""
            ");
        }

        [ActianSkip(NoOrderByOffsetFirstAndFetchInSubselects)]
        public override async Task Navigation_rewrite_on_owned_collection_with_composition(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_collection_with_composition(isAsync);
            AssertSql(@"
                SELECT (
                    SELECT FIRST 1 CASE
                        WHEN ""o"".""Id"" <> 42 THEN TRUE
                        ELSE FALSE
                    END
                    FROM ""Order"" AS ""o""
                    WHERE ""o0"".""Id"" = ""o"".""ClientId""
                    ORDER BY ""o"".""Id"")
                FROM ""OwnedPerson"" AS ""o0""
                WHERE ""o0"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o0"".""Id""
            ");
        }

        [ActianSkip(NoOrderByOffsetFirstAndFetchInSubselects)]
        public override async Task Navigation_rewrite_on_owned_collection_with_composition_complex(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_collection_with_composition_complex(isAsync);
            AssertSql(@"
                SELECT (
                    SELECT FIRST 1 ""t0"".""PersonAddress_Country_Name""
                    FROM ""Order"" AS ""o""
                    LEFT JOIN (
                        SELECT ""o0"".""Id"", ""o0"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o0""
                        WHERE ""o0"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                    ) AS ""t"" ON ""o"".""ClientId"" = ""t"".""Id""
                    LEFT JOIN ""OwnedPerson"" AS ""o1"" ON ""t"".""Id"" = ""o1"".""Id""
                    LEFT JOIN (
                        SELECT ""o2"".""Id"", ""o2"".""PersonAddress_Country_Name"", ""o2"".""PersonAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o2""
                        WHERE ""o2"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t0"" ON ""o1"".""Id"" = ""t0"".""Id""
                    WHERE ""o3"".""Id"" = ""o"".""ClientId""
                    ORDER BY ""o"".""Id"")
                FROM ""OwnedPerson"" AS ""o3""
                WHERE ""o3"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task SelectMany_on_owned_collection(bool isAsync)
        {
            await base.SelectMany_on_owned_collection(isAsync);
            AssertSql(@"
                SELECT ""o0"".""ClientId"", ""o0"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                INNER JOIN ""Order"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override Task Set_throws_for_owned_type(bool isAsync)
        {
            return base.Set_throws_for_owned_type(isAsync);
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity(isAsync);
            AssertSql(@"
                SELECT ""p"".""Id"", ""p"".""StarId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task Filter_owned_entity_chained_with_regular_entity_followed_by_projecting_owned_collection(bool isAsync)
        {
            await base.Filter_owned_entity_chained_with_regular_entity_followed_by_projecting_owned_collection(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o2"".""ClientId"", ""o2"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Order"" AS ""o2"" ON ""o"".""Id"" = ""o2"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND ((""p"".""Id"" <> 42) OR ""p"".""Id"" IS NULL)
                ORDER BY ""o"".""Id"", ""o2"".""ClientId"", ""o2"".""Id""
            ");
        }

        public override async Task Project_multiple_owned_navigations(bool isAsync)
        {
            await base.Project_multiple_owned_navigations(isAsync);
            AssertSql(@"
                SELECT ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""p"".""Id"", ""p"".""StarId"", ""o"".""Id"", ""o2"".""ClientId"", ""o2"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Order"" AS ""o2"" ON ""o"".""Id"" = ""o2"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o"".""Id"", ""o2"".""ClientId"", ""o2"".""Id""
            ");
        }

        public override async Task Project_multiple_owned_navigations_with_expansion_on_owned_collections(bool isAsync)
        {
            await base.Project_multiple_owned_navigations_with_expansion_on_owned_collections(isAsync);
            AssertSql(@"
                SELECT (
                    SELECT COUNT(*)
                    FROM ""Order"" AS ""o""
                    LEFT JOIN (
                        SELECT ""o0"".""Id"", ""o0"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o0""
                        WHERE ""o0"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                    ) AS ""t"" ON ""o"".""ClientId"" = ""t"".""Id""
                    LEFT JOIN ""OwnedPerson"" AS ""o1"" ON ""t"".""Id"" = ""o1"".""Id""
                    LEFT JOIN (
                        SELECT ""o2"".""Id"", ""o2"".""PersonAddress_Country_Name"", ""o2"".""PersonAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o2""
                        WHERE ""o2"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t0"" ON ""o1"".""Id"" = ""t0"".""Id""
                    LEFT JOIN ""Planet"" AS ""p"" ON ""t0"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                    LEFT JOIN ""Star"" AS ""s"" ON ""p"".""StarId"" = ""s"".""Id""
                    WHERE (""o3"".""Id"" = ""o"".""ClientId"") AND ((""s"".""Id"" <> 42) OR ""s"".""Id"" IS NULL)) AS ""Count"", ""p0"".""Id"", ""p0"".""StarId""
                FROM ""OwnedPerson"" AS ""o3""
                LEFT JOIN ""OwnedPerson"" AS ""o4"" ON ""o3"".""Id"" = ""o4"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""o5"".""PersonAddress_Country_Name"", ""o5"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o5""
                    WHERE ""o5"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t1"" ON ""o4"".""Id"" = ""t1"".""Id""
                LEFT JOIN ""Planet"" AS ""p0"" ON ""t1"".""PersonAddress_Country_PlanetId"" = ""p0"".""Id""
                WHERE ""o3"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o3"".""Id""
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_filter(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_filter(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafBAddress_Country_Name"", ""t5"".""LeafBAddress_Country_PlanetId"", ""t7"".""Id"", ""t8"".""Id"", ""t8"".""LeafAAddress_Country_Name"", ""t8"".""LeafAAddress_Country_PlanetId"", ""o11"".""ClientId"", ""o11"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafB'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafBAddress_Country_Name"", ""o7"".""LeafBAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""t6"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o8""
                    INNER JOIN (
                        SELECT ""o9"".""Id"", ""o9"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o9""
                        WHERE ""o9"".""Discriminator"" = N'LeafA'
                    ) AS ""t6"" ON ""o8"".""Id"" = ""t6"".""Id""
                ) AS ""t7"" ON ""o"".""Id"" = ""t7"".""Id""
                LEFT JOIN (
                    SELECT ""o10"".""Id"", ""o10"".""LeafAAddress_Country_Name"", ""o10"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o10""
                    WHERE ""o10"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t8"" ON ""t7"".""Id"" = ""t8"".""Id""
                LEFT JOIN ""Order"" AS ""o11"" ON ""o"".""Id"" = ""o11"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND ((""p"".""Id"" <> 7) OR ""p"".""Id"" IS NULL)
                ORDER BY ""o"".""Id"", ""o11"".""ClientId"", ""o11"".""Id""
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_property(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_property(isAsync);
            AssertSql(@"
                SELECT ""p"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""m"".""Id"", ""m"".""Diameter"", ""m"".""PlanetId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Moon"" AS ""m"" ON ""p"".""Id"" = ""m"".""PlanetId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o"".""Id"", ""m"".""Id""
            ");
        }

        public override async Task SelectMany_on_owned_reference_followed_by_regular_entity_and_collection(bool isAsync)
        {
            await base.SelectMany_on_owned_reference_followed_by_regular_entity_and_collection(isAsync);
            AssertSql(@"
                SELECT ""m"".""Id"", ""m"".""Diameter"", ""m"".""PlanetId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                INNER JOIN ""Moon"" AS ""m"" ON ""p"".""Id"" = ""m"".""PlanetId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task SelectMany_on_owned_reference_with_entity_in_between_ending_in_owned_collection(bool isAsync)
        {
            await base.SelectMany_on_owned_reference_with_entity_in_between_ending_in_owned_collection(isAsync);
            AssertSql(@"
                SELECT ""e"".""Id"", ""e"".""Name"", ""e"".""StarId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Star"" AS ""s"" ON ""p"".""StarId"" = ""s"".""Id""
                INNER JOIN ""Element"" AS ""e"" ON ""s"".""Id"" = ""e"".""StarId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection_count(bool isAsync)
        {
            return base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection_count(isAsync);
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference(isAsync);
            AssertSql(@"
                SELECT ""s"".""Id"", ""s"".""Name"", ""o"".""Id"", ""e"".""Id"", ""e"".""Name"", ""e"".""StarId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Star"" AS ""s"" ON ""p"".""StarId"" = ""s"".""Id""
                LEFT JOIN ""Element"" AS ""e"" ON ""s"".""Id"" = ""e"".""StarId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ORDER BY ""o"".""Id"", ""e"".""Id""
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_and_scalar(
            bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_and_scalar(isAsync);
            AssertSql(@"
                SELECT ""s"".""Name""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Star"" AS ""s"" ON ""p"".""StarId"" = ""s"".""Id""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ");
        }

        public override async Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_in_predicate_and_projection(bool isAsync)
        {
            await base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_in_predicate_and_projection(
                isAsync);
            AssertSql(@"
                SELECT ""s"".""Id"", ""s"".""Name"", ""o"".""Id"", ""e"".""Id"", ""e"".""Name"", ""e"".""StarId""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN ""Planet"" AS ""p"" ON ""t"".""PersonAddress_Country_PlanetId"" = ""p"".""Id""
                LEFT JOIN ""Star"" AS ""s"" ON ""p"".""StarId"" = ""s"".""Id""
                LEFT JOIN ""Element"" AS ""e"" ON ""s"".""Id"" = ""e"".""StarId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (""s"".""Name"" = N'Sol')
                ORDER BY ""o"".""Id"", ""e"".""Id""
            ");
        }

        [ActianTodo]
        public override async Task Query_with_OfType_eagerly_loads_correct_owned_navigations(bool isAsync)
        {
            await base.Query_with_OfType_eagerly_loads_correct_owned_navigations(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafAAddress_Country_Name"", ""t5"".""LeafAAddress_Country_PlanetId"", ""o8"".""ClientId"", ""o8"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafA'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafAAddress_Country_Name"", ""o7"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN ""Order"" AS ""o8"" ON ""o"".""Id"" = ""o8"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (""o"".""Discriminator"" = N'LeafA')
                ORDER BY ""o"".""Id"", ""o8"".""ClientId"", ""o8"".""Id""
            ");
        }

        public override Task Query_loads_reference_nav_automatically_in_projection(bool isAsync)
        {
            return base.Query_loads_reference_nav_automatically_in_projection(isAsync);
        }

        public override Task Throw_for_owned_entities_without_owner_in_tracking_query(bool isAsync)
        {
            return base.Throw_for_owned_entities_without_owner_in_tracking_query(isAsync);
        }

        [ActianTodo]
        public override async Task Preserve_includes_when_applying_skip_take_after_anonymous_type_select(bool isAsync)
        {
            await base.Preserve_includes_when_applying_skip_take_after_anonymous_type_select(isAsync);
            AssertSql(
                @"
                    SELECT COUNT(*)
                    FROM ""OwnedPerson"" AS ""o""
                    WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                ",
                @"
                    @__Count_0='4'
                    @__p_1='0'
                    @__p_2='100'
                    
                    SELECT ""t"".""Id"", ""t"".""Discriminator"", ""o1"".""Id"", ""t0"".""Id"", ""t0"".""PersonAddress_Country_Name"", ""t0"".""PersonAddress_Country_PlanetId"", ""t2"".""Id"", ""t3"".""Id"", ""t3"".""BranchAddress_Country_Name"", ""t3"".""BranchAddress_Country_PlanetId"", ""t5"".""Id"", ""t6"".""Id"", ""t6"".""LeafBAddress_Country_Name"", ""t6"".""LeafBAddress_Country_PlanetId"", ""t8"".""Id"", ""t9"".""Id"", ""t9"".""LeafAAddress_Country_Name"", ""t9"".""LeafAAddress_Country_PlanetId"", ""t"".""c"", ""o12"".""ClientId"", ""o12"".""Id""
                    FROM (
                        SELECT ""o"".""Id"", ""o"".""Discriminator"", @__Count_0 AS ""c""
                        FROM ""OwnedPerson"" AS ""o""
                        WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
                        ORDER BY ""o"".""Id""
                        OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
                    ) AS ""t""
                    LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""t"".""Id"" = ""o0"".""Id""
                    LEFT JOIN ""OwnedPerson"" AS ""o1"" ON ""t"".""Id"" = ""o1"".""Id""
                    LEFT JOIN (
                        SELECT ""o2"".""Id"", ""o2"".""PersonAddress_Country_Name"", ""o2"".""PersonAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o2""
                        WHERE ""o2"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t0"" ON ""o1"".""Id"" = ""t0"".""Id""
                    LEFT JOIN (
                        SELECT ""o3"".""Id"", ""t1"".""Id"" AS ""Id0""
                        FROM ""OwnedPerson"" AS ""o3""
                        INNER JOIN (
                            SELECT ""o4"".""Id"", ""o4"".""Discriminator""
                            FROM ""OwnedPerson"" AS ""o4""
                            WHERE ""o4"".""Discriminator"" IN (N'Branch', N'LeafA')
                        ) AS ""t1"" ON ""o3"".""Id"" = ""t1"".""Id""
                    ) AS ""t2"" ON ""t"".""Id"" = ""t2"".""Id""
                    LEFT JOIN (
                        SELECT ""o5"".""Id"", ""o5"".""BranchAddress_Country_Name"", ""o5"".""BranchAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o5""
                        WHERE ""o5"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t3"" ON ""t2"".""Id"" = ""t3"".""Id""
                    LEFT JOIN (
                        SELECT ""o6"".""Id"", ""t4"".""Id"" AS ""Id0""
                        FROM ""OwnedPerson"" AS ""o6""
                        INNER JOIN (
                            SELECT ""o7"".""Id"", ""o7"".""Discriminator""
                            FROM ""OwnedPerson"" AS ""o7""
                            WHERE ""o7"".""Discriminator"" = N'LeafB'
                        ) AS ""t4"" ON ""o6"".""Id"" = ""t4"".""Id""
                    ) AS ""t5"" ON ""t"".""Id"" = ""t5"".""Id""
                    LEFT JOIN (
                        SELECT ""o8"".""Id"", ""o8"".""LeafBAddress_Country_Name"", ""o8"".""LeafBAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o8""
                        WHERE ""o8"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t6"" ON ""t5"".""Id"" = ""t6"".""Id""
                    LEFT JOIN (
                        SELECT ""o9"".""Id"", ""t7"".""Id"" AS ""Id0""
                        FROM ""OwnedPerson"" AS ""o9""
                        INNER JOIN (
                            SELECT ""o10"".""Id"", ""o10"".""Discriminator""
                            FROM ""OwnedPerson"" AS ""o10""
                            WHERE ""o10"".""Discriminator"" = N'LeafA'
                        ) AS ""t7"" ON ""o9"".""Id"" = ""t7"".""Id""
                    ) AS ""t8"" ON ""t"".""Id"" = ""t8"".""Id""
                    LEFT JOIN (
                        SELECT ""o11"".""Id"", ""o11"".""LeafAAddress_Country_Name"", ""o11"".""LeafAAddress_Country_PlanetId""
                        FROM ""OwnedPerson"" AS ""o11""
                        WHERE ""o11"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                    ) AS ""t9"" ON ""t8"".""Id"" = ""t9"".""Id""
                    LEFT JOIN ""Order"" AS ""o12"" ON ""t"".""Id"" = ""o12"".""ClientId""
                    ORDER BY ""t"".""Id"", ""o12"".""ClientId"", ""o12"".""Id""
                "
            );
        }

        public override async Task Unmapped_property_projection_loads_owned_navigations(bool isAsync)
        {
            await base.Unmapped_property_projection_loads_owned_navigations(isAsync);
            AssertSql(@"
                SELECT ""o"".""Id"", ""o"".""Discriminator"", ""o0"".""Id"", ""t"".""Id"", ""t"".""PersonAddress_Country_Name"", ""t"".""PersonAddress_Country_PlanetId"", ""t1"".""Id"", ""t2"".""Id"", ""t2"".""BranchAddress_Country_Name"", ""t2"".""BranchAddress_Country_PlanetId"", ""t4"".""Id"", ""t5"".""Id"", ""t5"".""LeafBAddress_Country_Name"", ""t5"".""LeafBAddress_Country_PlanetId"", ""t7"".""Id"", ""t8"".""Id"", ""t8"".""LeafAAddress_Country_Name"", ""t8"".""LeafAAddress_Country_PlanetId"", ""o11"".""ClientId"", ""o11"".""Id""
                FROM ""OwnedPerson"" AS ""o""
                LEFT JOIN ""OwnedPerson"" AS ""o0"" ON ""o"".""Id"" = ""o0"".""Id""
                LEFT JOIN (
                    SELECT ""o1"".""Id"", ""o1"".""PersonAddress_Country_Name"", ""o1"".""PersonAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o1""
                    WHERE ""o1"".""PersonAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t"" ON ""o0"".""Id"" = ""t"".""Id""
                LEFT JOIN (
                    SELECT ""o2"".""Id"", ""t0"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o2""
                    INNER JOIN (
                        SELECT ""o3"".""Id"", ""o3"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o3""
                        WHERE ""o3"".""Discriminator"" IN (N'Branch', N'LeafA')
                    ) AS ""t0"" ON ""o2"".""Id"" = ""t0"".""Id""
                ) AS ""t1"" ON ""o"".""Id"" = ""t1"".""Id""
                LEFT JOIN (
                    SELECT ""o4"".""Id"", ""o4"".""BranchAddress_Country_Name"", ""o4"".""BranchAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o4""
                    WHERE ""o4"".""BranchAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t2"" ON ""t1"".""Id"" = ""t2"".""Id""
                LEFT JOIN (
                    SELECT ""o5"".""Id"", ""t3"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o5""
                    INNER JOIN (
                        SELECT ""o6"".""Id"", ""o6"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o6""
                        WHERE ""o6"".""Discriminator"" = N'LeafB'
                    ) AS ""t3"" ON ""o5"".""Id"" = ""t3"".""Id""
                ) AS ""t4"" ON ""o"".""Id"" = ""t4"".""Id""
                LEFT JOIN (
                    SELECT ""o7"".""Id"", ""o7"".""LeafBAddress_Country_Name"", ""o7"".""LeafBAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o7""
                    WHERE ""o7"".""LeafBAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t5"" ON ""t4"".""Id"" = ""t5"".""Id""
                LEFT JOIN (
                    SELECT ""o8"".""Id"", ""t6"".""Id"" AS ""Id0""
                    FROM ""OwnedPerson"" AS ""o8""
                    INNER JOIN (
                        SELECT ""o9"".""Id"", ""o9"".""Discriminator""
                        FROM ""OwnedPerson"" AS ""o9""
                        WHERE ""o9"".""Discriminator"" = N'LeafA'
                    ) AS ""t6"" ON ""o8"".""Id"" = ""t6"".""Id""
                ) AS ""t7"" ON ""o"".""Id"" = ""t7"".""Id""
                LEFT JOIN (
                    SELECT ""o10"".""Id"", ""o10"".""LeafAAddress_Country_Name"", ""o10"".""LeafAAddress_Country_PlanetId""
                    FROM ""OwnedPerson"" AS ""o10""
                    WHERE ""o10"".""LeafAAddress_Country_PlanetId"" IS NOT NULL
                ) AS ""t8"" ON ""t7"".""Id"" = ""t8"".""Id""
                LEFT JOIN ""Order"" AS ""o11"" ON ""o"".""Id"" = ""o11"".""ClientId""
                WHERE ""o"".""Discriminator"" IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (""o"".""Id"" = 1)
                ORDER BY ""o"".""Id"", ""o11"".""ClientId"", ""o11"".""Id""
            ");
        }

        public class OwnedQueryActianFixture : RelationalOwnedQueryFixture, IActianSqlFixture
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
