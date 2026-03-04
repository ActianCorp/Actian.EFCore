// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Actian.EFCore.Query
{
    public class FromSqlSprocQueryActianTest : FromSqlSprocQueryTestBase<NorthwindQueryActianFixture<NoopModelCustomizer>>
    {
        public FromSqlSprocQueryActianTest(NorthwindQueryActianFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure(bool async)
        {
            await base.From_sql_queryable_stored_procedure(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_tag(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_tag(async);
            AssertSql(@"
                -- Stored Procedure
                
                ""dbo"".""Ten Most Expensive Products""
            ");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_tags(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_tags(async);
            AssertSql(@"
                -- Stored Procedure
                
                ""dbo"".""Ten Most Expensive Products""
            ");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_projection(bool async)
        {
            await base.From_sql_queryable_stored_procedure_projection(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        public override Task From_sql_queryable_stored_procedure_re_projection(bool async)
        {
            return base.From_sql_queryable_stored_procedure_re_projection(async);
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_re_projection_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_re_projection_on_client(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_parameter(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_parameter(async);
            AssertSql(@"
                p0='ALFKI' (Size = 4000)
                
                ""dbo"".""CustOrderHist"" @CustomerID = @p1
            ");
        }

        public override Task From_sql_queryable_stored_procedure_composed(bool async)
        {
            return base.From_sql_queryable_stored_procedure_composed(async);
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_composed_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_composed_on_client(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        public override Task From_sql_queryable_stored_procedure_with_parameter_composed(bool async)
        {
            return base.From_sql_queryable_stored_procedure_with_parameter_composed(async);
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_parameter_composed_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_parameter_composed_on_client(async);
            AssertSql(@"
                p0='ALFKI' (Size = 4000)
                
                ""dbo"".""CustOrderHist"" @CustomerID = @p1
            ");
        }

        public override Task From_sql_queryable_stored_procedure_take(bool async)
        {
            return base.From_sql_queryable_stored_procedure_take(async);
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_take_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_take_on_client(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_caller_info_tag(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_caller_info_tag(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_with_caller_info_tag_and_other_tags(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_caller_info_tag_and_other_tags(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        public override Task From_sql_queryable_stored_procedure_min(bool async)
        {
            return base.From_sql_queryable_stored_procedure_min(async);
        }

        [ActianTodo]
        public override async Task From_sql_queryable_stored_procedure_min_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_min_on_client(async);
            AssertSql(@"dbo"".""Ten Most Expensive Products""");
        }

        public override Task From_sql_queryable_stored_procedure_with_include_throws(bool async)
        {
            return base.From_sql_queryable_stored_procedure_with_include_throws(async);
        }

        public override Task From_sql_queryable_with_multiple_stored_procedures(bool async)
        {
            return base.From_sql_queryable_with_multiple_stored_procedures(async);
        }

        [ActianTodo]
        public override Task From_sql_queryable_with_multiple_stored_procedures_on_client(bool async)
        {
            return base.From_sql_queryable_with_multiple_stored_procedures_on_client(async);
        }

        public override Task From_sql_queryable_stored_procedure_and_select(bool async)
        {
            return base.From_sql_queryable_stored_procedure_and_select(async);
        }

        [ActianTodo]
        public override Task From_sql_queryable_stored_procedure_and_select_on_client(bool async)
        {
            return base.From_sql_queryable_stored_procedure_and_select_on_client(async);
        }

        public override Task From_sql_queryable_select_and_stored_procedure(bool async)
        {
            return base.From_sql_queryable_select_and_stored_procedure(async);
        }

        [ActianTodo]
        public override Task From_sql_queryable_select_and_stored_procedure_on_client(bool async)
        {
            return base.From_sql_queryable_select_and_stored_procedure_on_client(async);
        }

        protected override string TenMostExpensiveProductsSproc => @"""dbo"".""Ten Most Expensive Products""";

        protected override string CustomerOrderHistorySproc => @"""dbo"".""CustOrderHist"" @CustomerID = {0}";
    }
}
