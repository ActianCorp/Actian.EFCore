﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using ActianConditionAttribute = Actian.EFCore.TestUtilities.ActianConditionAttribute;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindFunctionsQueryActianTest : NorthwindFunctionsQueryRelationalTestBase<
    NorthwindQueryActianFixture<NoopModelCustomizer>>
{
    public NorthwindFunctionsQueryActianTest(
        NorthwindQueryActianFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        ClearLog();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task TimeSpan_Compare_to_simple_zero(bool async, bool compareTo)
    {
        await base.TimeSpan_Compare_to_simple_zero(async, compareTo);

        AssertSql(
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" = @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <> @__myDatetime_0 OR "o"."OrderDate" IS NULL
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" > @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <= @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" > @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <= @__myDatetime_0
""");
    }

    public override async Task String_StartsWith_Literal(bool async)
    {
        await base.String_StartsWith_Literal(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'M%'
""");
    }

    public override async Task String_StartsWith_Parameter(bool async)
    {
        await base.String_StartsWith_Parameter(async);

        AssertSql(
            """
@__pattern_0_startswith='M%'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE @__pattern_0_startswith ESCAPE N'\\'
""");
    }

    public override async Task String_StartsWith_Identity(bool async)
    {
        await base.String_StartsWith_Identity(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND LEFT("c"."ContactName", LENGTH("c"."ContactName")) = "c"."ContactName"
""");
    }

    public override async Task String_StartsWith_Column(bool async)
    {
        await base.String_StartsWith_Column(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND LEFT("c"."ContactName", LENGTH("c"."ContactName")) = "c"."ContactName"
""");
    }

    public override async Task String_StartsWith_MethodCall(bool async)
    {
        await base.String_StartsWith_MethodCall(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'M%'
""");
    }

    public override async Task String_EndsWith_Literal(bool async)
    {
        await base.String_EndsWith_Literal(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'%b'
""");
    }

    public override async Task String_EndsWith_Parameter(bool async)
    {
        await base.String_EndsWith_Parameter(async);

        AssertSql(
            """
@__pattern_0_endswith='%b'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE @__pattern_0_endswith ESCAPE N'\\'
""");
    }

    public override async Task String_EndsWith_Identity(bool async)
    {
        await base.String_EndsWith_Identity(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND RIGHT("c"."ContactName", LENGTH("c"."ContactName")) = "c"."ContactName"
""");
    }

    public override async Task String_EndsWith_Column(bool async)
    {
        await base.String_EndsWith_Column(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND RIGHT("c"."ContactName", LENGTH("c"."ContactName")) = "c"."ContactName"
""");
    }

    public override async Task String_EndsWith_MethodCall(bool async)
    {
        await base.String_EndsWith_MethodCall(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'%m'
""");
    }

    [ActianTodo] //Expected: 34 Actual:   19
    public override async Task String_Contains_Literal(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.ContactName.Contains("M")), // case-insensitive
            ss => ss.Set<Customer>().Where(c => c.ContactName.Contains("M") || c.ContactName.Contains("m"))); // case-sensitive

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'%M%'
""");
    }

    public override async Task String_Contains_Identity(bool async)
    {
        await base.String_Contains_Identity(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND (POSITION("c"."ContactName", "c"."ContactName") > 0 OR ("c"."ContactName" LIKE N''))
""");
    }

    public override async Task String_Contains_Column(bool async)
    {
        await base.String_Contains_Column(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" IS NOT NULL AND (POSITION("c"."ContactName", "c"."CompanyName") > 0 OR ("c"."ContactName" LIKE N''))
""");
    }

    public override async Task String_Contains_constant_with_whitespace(bool async)
    {
        await base.String_Contains_constant_with_whitespace(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'%     %'
""");
    }

    public override async Task String_Contains_parameter_with_whitespace(bool async)
    {
        await base.String_Contains_parameter_with_whitespace(async);

        AssertSql(
            """
@__pattern_0_contains='%     %'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE @__pattern_0_contains ESCAPE N'\\'
""");
    }

    public override async Task String_FirstOrDefault_MethodCall(bool async)
    {
        await base.String_FirstOrDefault_MethodCall(async);
        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE SUBSTRING("c"."ContactName", 1, 1) = N'A'
""");
    }

    public override async Task String_LastOrDefault_MethodCall(bool async)
    {
        await base.String_LastOrDefault_MethodCall(async);
        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE SUBSTRING("c"."ContactName", LENGTH("c"."ContactName"), 1) = N's'
""");
    }


    public override async Task String_Contains_MethodCall(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Customer>().Where(c => c.ContactName.Contains(LocalMethod1())), // case-insensitive
            ss => ss.Set<Customer>().Where(
                c => c.ContactName.Contains(LocalMethod1())
                    || c.ContactName.Contains(LocalMethod1()))); // case-insensitive

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactName" LIKE N'%M%'
""");
    }

    [ActianCondition(ActianCondition.SupportsFunctions2017)]
    public override async Task String_Join_over_non_nullable_column(bool async)
    {
        await base.String_Join_over_non_nullable_column(async);

        AssertSql(
            """
SELECT "c"."City", COALESCE(STRING_AGG("c"."CustomerID", N'|'), N'') AS "Customers"
FROM "Customers" AS "c"
GROUP BY "c"."City"
""");
    }

    [ActianCondition(ActianCondition.SupportsFunctions2017)]
    public override async Task String_Join_over_nullable_column(bool async)
    {
        await base.String_Join_over_nullable_column(async);

        AssertSql(
            """
SELECT "c"."City", COALESCE(STRING_AGG(COALESCE("c"."Region", N''), N'|'), N'') AS "Regions"
FROM "Customers" AS "c"
GROUP BY "c"."City"
""");
    }

    public override async Task String_Join_with_predicate(bool async)
    {
        await base.String_Join_with_predicate(async);

        AssertSql(
            """
SELECT "c1"."City", "c2"."CustomerID"
FROM (
    SELECT "c"."City"
    FROM "Customers" AS "c"
    GROUP BY "c"."City"
) AS "c1"
LEFT JOIN (
    SELECT "c0"."CustomerID", "c0"."City"
    FROM "Customers" AS "c0"
    WHERE CAST(LENGTH("c0"."ContactName") AS integer) > 10
) AS "c2" ON "c1"."City" = "c2"."City"
ORDER BY "c1"."City"
""");
    }

    [ActianCondition(ActianCondition.SupportsFunctions2017)]
    public override async Task String_Join_with_ordering(bool async)
    {
        await base.String_Join_with_ordering(async);

        AssertSql(
            """
SELECT "c"."City", COALESCE(STRING_AGG("c"."CustomerID", N'|') WITHIN GROUP (ORDER BY "c"."CustomerID" DESC), N'') AS "Customers"
FROM "Customers" AS "c"
GROUP BY "c"."City"
""");
    }

    [ActianCondition(ActianCondition.SupportsFunctions2017)]
    public override async Task String_Concat(bool async)
    {
        await base.String_Concat(async);

        AssertSql(
            """
SELECT "c"."City", COALESCE(STRING_AGG("c"."CustomerID", N''), N'') AS "Customers"
FROM "Customers" AS "c"
GROUP BY "c"."City"
""");
    }

    public override async Task String_Compare_simple_zero(bool async)
    {
        await base.String_Compare_simple_zero(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <> N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""");
    }

    public override async Task String_Compare_simple_one(bool async)
    {
        await base.String_Compare_simple_one(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'AROUT'
""");
    }

    public override async Task String_compare_with_parameter(bool async)
    {
        await base.String_compare_with_parameter(async);

        AssertSql(
            """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= @__customer_CustomerID_0
""");
    }

    [ActianTodo] //Expected: 0  Actual: 91
    public override async Task String_Compare_simple_more_than_one(bool async)
    {
        await base.String_Compare_simple_more_than_one(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END = 42
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END > 42
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE 42 > CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END
""");
    }

    [ActianTodo] //Expected: 91 Actual: 0
    public override async Task String_Compare_nested(bool async)
    {
        await base.String_Compare_nested(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'M' + "c"."CustomerID"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <> UPPER("c"."CustomerID")
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > REPLACE(N'ALFKI', N'ALF', "c"."CustomerID")
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'M' + "c"."CustomerID"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > UPPER("c"."CustomerID")
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < REPLACE(N'ALFKI', N'ALF', "c"."CustomerID")
""");
    }

    public override async Task String_Compare_multi_predicate(bool async)
    {
        await base.String_Compare_multi_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'ALFKI' AND "c"."CustomerID" < N'CACTU'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactTitle" = N'Owner' AND ("c"."Country" <> N'USA' OR "c"."Country" IS NULL)
""");
    }

    public override async Task String_Compare_to_simple_zero(bool async)
    {
        await base.String_Compare_to_simple_zero(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <> N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""");
    }

    public override async Task String_Compare_to_simple_one(bool async)
    {
        await base.String_Compare_to_simple_one(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'AROUT'
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'AROUT'
""");
    }

    public override async Task String_compare_to_with_parameter(bool async)
    {
        await base.String_compare_to_with_parameter(async);

        AssertSql(
            """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= @__customer_CustomerID_0
""",
                //
                """
@__customer_CustomerID_0='AROUT'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= @__customer_CustomerID_0
""");
    }

    [ActianTodo] //Expected: 0 Actual:   91
    public override async Task String_Compare_to_simple_more_than_one(bool async)
    {
        await base.String_Compare_to_simple_more_than_one(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END = 42
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END > 42
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE 42 > CASE
    WHEN "c"."CustomerID" = N'ALFKI' THEN 0
    WHEN "c"."CustomerID" > N'ALFKI' THEN 1
    WHEN "c"."CustomerID" < N'ALFKI' THEN -1
END
""");
    }

    public override async Task String_Compare_to_nested(bool async)
    {
        await base.String_Compare_to_nested(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <> (N'M' + "c"."CustomerID")
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = UPPER("c"."CustomerID")
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > REPLACE(N'AROUT', N'OUT', "c"."CustomerID")
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" <= (N'M' + "c"."CustomerID")
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" > UPPER("c"."CustomerID")
""",
                //
                """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" < REPLACE(N'AROUT', N'OUT', "c"."CustomerID")
""");
    }

    public override async Task String_Compare_to_multi_predicate(bool async)
    {
        await base.String_Compare_to_multi_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" >= N'ALFKI' AND "c"."CustomerID" < N'CACTU'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."ContactTitle" = N'Owner' AND ("c"."Country" <> N'USA' OR "c"."Country" IS NULL)
""");
    }

    public override async Task DateTime_Compare_to_simple_zero(bool async, bool compareTo)
    {
        await base.DateTime_Compare_to_simple_zero(async, compareTo);

        AssertSql(
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" = @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <> @__myDatetime_0 OR "o"."OrderDate" IS NULL
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" > @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <= @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" > @__myDatetime_0
""",
            //
            """
@__myDatetime_0='1998-05-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" <= @__myDatetime_0
""");
    }

    public override async Task Int_Compare_to_simple_zero(bool async)
    {
        await base.Int_Compare_to_simple_zero(async);

        AssertSql(
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" = @__orderId_0
""",
            //
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" <> @__orderId_0
""",
            //
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" > @__orderId_0
""",
            //
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" <= @__orderId_0
""",
            //
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" > @__orderId_0
""",
            //
            """
@__orderId_0='10250'

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" <= @__orderId_0
""");
    }

    public override async Task Where_math_abs1(bool async)
    {
        await base.Where_math_abs1(async);

        AssertSql(
            """
SELECT "p"."ProductID", "p"."Discontinued", "p"."ProductName", "p"."SupplierID", "p"."UnitPrice", "p"."UnitsInStock"
FROM "Products" AS "p"
WHERE ABS("p"."ProductID") > 10
""");
    }

    public override async Task Where_math_abs2(bool async)
    {
        await base.Where_math_abs2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."UnitPrice" < 7.0 AND ABS("o"."Quantity") > 10
""");
    }

    public override async Task Where_math_abs3(bool async)
    {
        await base.Where_math_abs3(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5 AND ABS("o"."UnitPrice") > 10.0
""");
    }

    public override async Task Where_math_abs_uncorrelated(bool async)
    {
        await base.Where_math_abs_uncorrelated(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."UnitPrice" < 7.0 AND 10 < "o"."ProductID"
""");
    }

    public override async Task Where_math_ceiling1(bool async)
    {
        await base.Where_math_ceiling1(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."UnitPrice" < 7.0 AND CEILING(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_ceiling2(bool async)
    {
        await base.Where_math_ceiling2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5 AND CEILING("o"."UnitPrice") > 10.0
""");
    }

    public override async Task Where_math_floor(bool async)
    {
        await base.Where_math_floor(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5 AND FLOOR("o"."UnitPrice") > 10.0
""");
    }

    public override async Task Where_math_power(bool async)
    {
        await base.Where_math_power(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE POWER(CAST("o"."Discount" AS float), 3.0) > 0.004999999888241291
""");
    }

    public override async Task Where_math_square(bool async)
    {
        await base.Where_math_square(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE POWER(CAST("o"."Discount" AS float), 2.0) > 0.05000000074505806
""");
    }

    [ActianTodo]
    public override async Task Where_math_round(bool async)
    {
        await base.Where_math_round(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < CAST(5 AS smallint) AND ROUND("o"."UnitPrice", 0) > 10.0
""");
    }

    [ActianTodo]
    public override async Task Sum_over_round_works_correctly_in_projection(bool async)
    {
        await base.Sum_over_round_works_correctly_in_projection(async);

        AssertSql(
            """
SELECT "o"."OrderID", (
    SELECT COALESCE(SUM(ROUND("o0"."UnitPrice", 2)), 0.0)
    FROM "Order Details" AS "o0"
    WHERE "o"."OrderID" = "o0"."OrderID") AS "Sum"
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10300
""");
    }

    [ActianTodo]
    public override async Task Sum_over_round_works_correctly_in_projection_2(bool async)
    {
        await base.Sum_over_round_works_correctly_in_projection_2(async);

        AssertSql(
            """
SELECT "o"."OrderID", (
    SELECT COALESCE(SUM(ROUND("o0"."UnitPrice" * "o0"."UnitPrice", 2)), 0.0)
    FROM "Order Details" AS "o0"
    WHERE "o"."OrderID" = "o0"."OrderID") AS "Sum"
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10300
""");
    }

    [ActianTodo]
    public override async Task Sum_over_truncate_works_correctly_in_projection(bool async)
    {
        await base.Sum_over_truncate_works_correctly_in_projection(async);

        AssertSql(
            """
SELECT "o"."OrderID", (
    SELECT COALESCE(SUM(ROUND("o0"."UnitPrice", 0, 1)), 0.0)
    FROM "Order Details" AS "o0"
    WHERE "o"."OrderID" = "o0"."OrderID") AS "Sum"
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10300
""");
    }

    [ActianTodo]
    public override async Task Sum_over_truncate_works_correctly_in_projection_2(bool async)
    {
        await base.Sum_over_truncate_works_correctly_in_projection_2(async);

        AssertSql(
            """
SELECT "o"."OrderID", (
    SELECT COALESCE(SUM(ROUND("o0"."UnitPrice" * "o0"."UnitPrice", 0, 1)), 0.0)
    FROM "Order Details" AS "o0"
    WHERE "o"."OrderID" = "o0"."OrderID") AS "Sum"
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10300
""");
    }

    public override async Task Select_math_round_int(bool async)
    {
        await base.Select_math_round_int(async);

        AssertSql(
            """
SELECT ROUND(CAST("o"."OrderID" AS float), 0) AS "A"
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10250
""");
    }

    [ActianTodo]
    public override async Task Select_math_truncate_int(bool async)
    {
        await base.Select_math_truncate_int(async);

        AssertSql(
            """
SELECT CAST("o"."OrderID" AS float)
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10250
""");
    }

    [ActianTodo]
    public override async Task Where_math_round2(bool async)
    {
        await base.Where_math_round2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE ROUND("o"."UnitPrice", 2) > 100.0
""");
    }

    [ActianTodo]
    public override async Task Where_math_truncate(bool async)
    {
        await base.Where_math_truncate(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < CAST(5 AS smallint) AND ROUND("o"."UnitPrice", 0, 1) > 10.0
""");
    }

    public override async Task Where_math_exp(bool async)
    {
        await base.Where_math_exp(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND EXP(CAST("o"."Discount" AS float)) > 1.0
""");
    }

    [ActianTodo]
    public override async Task Where_math_log10(bool async)
    {
        await base.Where_math_log10(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > CAST(0 AS real) AND LOG10(CAST("o"."Discount" AS float)) < 0.0E0
""");
    }

    public override async Task Where_math_log(bool async)
    {
        await base.Where_math_log(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > 0 AND LOG(CAST("o"."Discount" AS float)) < 0.0
""");
    }

    [ActianTodo]
    public override async Task Where_math_log_new_base(bool async)
    {
        await base.Where_math_log_new_base(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > CAST(0 AS real) AND LOG(CAST("o"."Discount" AS float), 7.0) < 0.0
""");
    }

    public override async Task Where_math_sqrt(bool async)
    {
        await base.Where_math_sqrt(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SQRT(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_acos(bool async)
    {
        await base.Where_math_acos(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ACOS(CAST("o"."Discount" AS float)) > 1.0
""");
    }

    public override async Task Where_math_asin(bool async)
    {
        await base.Where_math_asin(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ASIN(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_atan(bool async)
    {
        await base.Where_math_atan(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ATAN(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_atan2(bool async)
    {
        await base.Where_math_atan2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ATAN2(CAST("o"."Discount" AS float), 1.0) > 0.0
""");
    }

    public override async Task Where_math_cos(bool async)
    {
        await base.Where_math_cos(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND COS(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    [ActianTodo]
    public override async Task Where_math_sin(bool async)
    {
        await base.Where_math_sin(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SIN(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_tan(bool async)
    {
        await base.Where_math_tan(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND TAN(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_math_sign(bool async)
    {
        await base.Where_math_sign(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SIGN("o"."Discount") > 0
""");
    }

    public override async Task Where_math_min(bool async)
    {
        // Translate Math.Min.
        await AssertTranslationFailed(() => base.Where_math_min(async));

        AssertSql();
    }

    public override async Task Where_math_max(bool async)
    {
        // Translate Math.Max.
        await AssertTranslationFailed(() => base.Where_math_max(async));

        AssertSql();
    }

    [ActianTodo]
    public override async Task Where_math_degrees(bool async)
    {
        await base.Where_math_degrees(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND DEGREES(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    [ActianTodo]
    public override async Task Where_math_radians(bool async)
    {
        await base.Where_math_radians(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND RADIANS(CAST("o"."Discount" AS float)) > 0.0
""");
    }

    public override async Task Where_mathf_abs1(bool async)
    {
        await base.Where_mathf_abs1(async);

        AssertSql(
            """
SELECT "p"."ProductID", "p"."Discontinued", "p"."ProductName", "p"."SupplierID", "p"."UnitPrice", "p"."UnitsInStock"
FROM "Products" AS "p"
WHERE ABS(CAST("p"."ProductID" AS float4)) > 10
""");
    }

    public override async Task Where_mathf_ceiling1(bool async)
    {
        await base.Where_mathf_ceiling1(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."UnitPrice" < 7.0 AND CEILING("o"."Discount") > 0
""");
    }

    public override async Task Where_mathf_floor(bool async)
    {
        await base.Where_mathf_floor(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5 AND FLOOR(CAST("o"."UnitPrice" AS float4)) > 10
""");
    }

    public override async Task Where_mathf_power(bool async)
    {
        await base.Where_mathf_power(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE POWER("o"."Discount", 3) > 0.005
""");
    }

    public override async Task Where_mathf_square(bool async)
    {
        await base.Where_mathf_square(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE POWER("o"."Discount", 2) > 0.05
""");
    }

    public override async Task Where_mathf_round2(bool async)
    {
        await base.Where_mathf_round2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE CAST(ROUND(CAST("o"."UnitPrice" AS float4), 2) AS float4) > 100
""");
    }

    public override async Task Select_mathf_round(bool async)
    {
        await base.Select_mathf_round(async);

        AssertSql(
            """
SELECT CAST(ROUND(CAST("o"."OrderID" AS float4), 0) AS float4)
FROM "Orders" AS "o"
WHERE "o"."OrderID" < 10250
""");
    }

    public override async Task Select_mathf_round2(bool async)
    {
        await base.Select_mathf_round2(async);

        AssertSql(
            """
SELECT CAST(ROUND(CAST("o"."UnitPrice" AS float4), 2) AS float4)
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_truncate(bool async)
    {
        await base.Where_mathf_truncate(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < CAST(5 AS smallint) AND CAST(ROUND(CAST("o"."UnitPrice" AS real), 0, 1) AS real) > CAST(10 AS real)
""");
    }

    [ActianTodo]
    public override async Task Select_mathf_truncate(bool async)
    {
        await base.Select_mathf_truncate(async);

        AssertSql(
            """
SELECT CAST("o"."UnitPrice" AS float4)
FROM "Order Details" AS "o"
WHERE "o"."Quantity" < 5
""");
    }

    public override async Task Where_mathf_exp(bool async)
    {
        await base.Where_mathf_exp(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND EXP("o"."Discount") > 1
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_log10(bool async)
    {
        await base.Where_mathf_log10(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > 0 AND LOG10("o"."Discount") < 0
""");
    }

    public override async Task Where_mathf_log(bool async)
    {
        await base.Where_mathf_log(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > 0 AND LOG("o"."Discount") < 0
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_log_new_base(bool async)
    {
        await base.Where_mathf_log_new_base(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND "o"."Discount" > 0 AND LOG("o"."Discount", 7) < 0
""");
    }

    public override async Task Where_mathf_sqrt(bool async)
    {
        await base.Where_mathf_sqrt(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SQRT("o"."Discount") > 0
""");
    }

    public override async Task Where_mathf_acos(bool async)
    {
        await base.Where_mathf_acos(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ACOS("o"."Discount") > 1
""");
    }

    public override async Task Where_mathf_asin(bool async)
    {
        await base.Where_mathf_asin(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ASIN("o"."Discount") > 0
""");
    }

    public override async Task Where_mathf_atan(bool async)
    {
        await base.Where_mathf_atan(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ATAN("o"."Discount") > 0
""");
    }

    public override async Task Where_mathf_atan2(bool async)
    {
        await base.Where_mathf_atan2(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND ATAN2("o"."Discount", 1) > 0
""");
    }

    public override async Task Where_mathf_cos(bool async)
    {
        await base.Where_mathf_cos(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND COS("o"."Discount") > 0
""");
    }

    public override async Task Where_mathf_sin(bool async)
    {
        await base.Where_mathf_sin(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SIN("o"."Discount") > 0
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_tan(bool async)
    {
        await base.Where_mathf_tan(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND TAN("o"."Discount") > 0
""");
    }

    //[ActianTodo]
    public override async Task Where_mathf_sign(bool async)
    {
        await base.Where_mathf_sign(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND SIGN("o"."Discount") > 0
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_degrees(bool async)
    {
        await base.Where_mathf_degrees(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND DEGREES("o"."Discount") > 0
""");
    }

    [ActianTodo]
    public override async Task Where_mathf_radians(bool async)
    {
        await base.Where_mathf_radians(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."ProductID", "o"."Discount", "o"."Quantity", "o"."UnitPrice"
FROM "Order Details" AS "o"
WHERE "o"."OrderID" = 11077 AND RADIANS("o"."Discount") > CAST(0 AS real)
""");
    }

    [ActianTodo]
    public override async Task Where_guid_newguid(bool async)
    {
        await base.Where_guid_newguid(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE NEWID() <> '00000000-0000-0000-0000-000000000000'
""");
    }

    public override async Task Where_string_to_upper(bool async)
    {
        await base.Where_string_to_upper(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE UPPER("c"."CustomerID") = N'ALFKI'
""");
    }

    public override async Task Where_string_to_lower(bool async)
    {
        await base.Where_string_to_lower(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE LOWER("c"."CustomerID") = N'alfki'
""");
    }

    public override async Task Where_functions_nested(bool async)
    {
        await base.Where_functions_nested(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE POWER(CAST(CAST(LENGTH("c"."CustomerID") AS integer) AS float), 2.0) = 25.0
""");
    }

    [ActianTodo] //There is no such function as 'convert'.
    public override async Task Convert_ToBoolean(bool async)
    {
        await base.Convert_ToBoolean(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(bit, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(tinyint, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(decimal(18, 2), "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(float, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CAST(CONVERT(float, "o"."OrderID" % 3) AS real)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(smallint, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(int, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bit, CONVERT(bigint, "o"."OrderID" % 3)) = CAST(1 AS boolean)
""");
    }

    [ActianTodo]
    public override async Task Convert_ToByte(bool async)
    {
        await base.Convert_ToByte(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(bit, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(tinyint, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(float, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(smallint, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(int, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(bigint, "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(tinyint, CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= CAST(0 AS tinyint)
""");
    }

    [ActianTodo]
    public override async Task Convert_ToDecimal(bool async)
    {
        await base.Convert_ToDecimal(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(bit, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(tinyint, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(float, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(smallint, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(int, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(bigint, "o"."OrderID" % 1)) >= 0.0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(decimal(18, 2), CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= 0.0
""");
    }

    [ActianTodo]
    public override async Task Convert_ToDouble(bool async)
    {
        await base.Convert_ToDouble(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(bit, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(tinyint, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(float, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(smallint, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(int, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(bigint, "o"."OrderID" % 1)) >= 0.0E0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(float, CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= 0.0E0
""");
    }

    [ActianTodo]
    public override async Task Convert_ToInt16(bool async)
    {
        await base.Convert_ToInt16(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(bit, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(tinyint, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(float, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(smallint, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(int, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(bigint, "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(smallint, CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= CAST(0 AS smallint)
""");
    }

    [ActianTodo]
    public override async Task Convert_ToInt32(bool async)
    {
        await base.Convert_ToInt32(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(bit, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(tinyint, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(float, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(smallint, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(int, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(bigint, "o"."OrderID" % 1)) >= 0
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(int, CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= 0
""");
    }

    [ActianTodo]
    public override async Task Convert_ToInt64(bool async)
    {
        await base.Convert_ToInt64(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(bit, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(tinyint, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(decimal(18, 2), "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(float, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(smallint, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(int, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(bigint, "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(bigint, CONVERT(nvarchar(max), "o"."OrderID" % 1)) >= CAST(0 AS bigint)
""");
    }

    [ActianTodo]
    public override async Task Convert_ToString(bool async)
    {
        await base.Convert_ToString(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(bit, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(tinyint, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(decimal(18, 2), "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(float, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CAST(CONVERT(float, "o"."OrderID" % 1) AS real)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(smallint, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(int, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(bigint, "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND CONVERT(nvarchar(max), CONVERT(nvarchar(max), "o"."OrderID" % 1)) <> N'10'
""",
            //
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."CustomerID" = N'ALFKI' AND (CONVERT(nvarchar(max), "o"."OrderDate") LIKE N'%1997%' OR CONVERT(nvarchar(max), "o"."OrderDate") LIKE N'%1998%')
""");
    }

    public override async Task Indexof_with_emptystring(bool async)
    {
        await base.Indexof_with_emptystring(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE CASE
    WHEN "c"."Region" IS NOT NULL THEN 0
END = 0
""");
    }

    public override async Task Indexof_with_one_constant_arg(bool async)
    {
        await base.Indexof_with_one_constant_arg(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE (POSITION(N'a', "c"."ContactName") - 1) = 1
""");
    }

    [ActianTodo]
    public override async Task Indexof_with_one_parameter_arg(bool async)
    {
        await base.Indexof_with_one_parameter_arg(async);

        AssertSql(
            """
@__pattern_0='a' (Size = 30)

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE (POSITION(@__pattern_0, "c"."ContactName") - CASE
    WHEN @__pattern_0 = N'' THEN 0
    ELSE 1
END) = 1
""");
    }

    public override async Task Indexof_with_constant_starting_position(bool async)
    {
        await base.Indexof_with_constant_starting_position(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE (POSITION(N'a', "c"."ContactName", 3) - 1) = 4
""");
    }

    [ActianTodo]
    public override async Task Indexof_with_parameter_starting_position(bool async)
    {
        await base.Indexof_with_parameter_starting_position(async);

        AssertSql(
            """
@__start_0='2'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE (POSITION(N'a', "c"."ContactName", @__start_0 + 1) - 1) = 4
""");
    }

    public override async Task Replace_with_emptystring(bool async)
    {
        await base.Replace_with_emptystring(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE REPLACE("c"."ContactName", N'ia', N'') = N'Mar Anders'
""");
    }

    public override async Task Replace_using_property_arguments(bool async)
    {
        await base.Replace_using_property_arguments(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE REPLACE("c"."ContactName", "c"."ContactName", "c"."CustomerID") = "c"."CustomerID"
""");
    }

    public override async Task Substring_with_one_arg_with_zero_startindex(bool async)
    {
        await base.Substring_with_one_arg_with_zero_startindex(async);

        AssertSql(
            """
SELECT "c"."ContactName"
FROM "Customers" AS "c"
WHERE SUBSTRING("c"."CustomerID", 0 + 1, LENGTH("c"."CustomerID")) = N'ALFKI'
""");
    }

    public override async Task Substring_with_one_arg_with_constant(bool async)
    {
        await base.Substring_with_one_arg_with_constant(async);

        AssertSql(
            """
SELECT "c"."ContactName"
FROM "Customers" AS "c"
WHERE SUBSTRING("c"."CustomerID", 1 + 1, LENGTH("c"."CustomerID")) = N'LFKI'
""");
    }

    [ActianTodo]
    public override async Task Substring_with_one_arg_with_closure(bool async)
    {
        await base.Substring_with_one_arg_with_closure(async);

        AssertSql(
            """
@__start_0='2'

SELECT "c"."ContactName"
FROM "Customers" AS "c"
WHERE SUBSTRING("c"."CustomerID", @__start_0 + 1, LENGTH("c"."CustomerID")) = N'FKI'
""");
    }

    public override async Task Substring_with_two_args_with_zero_startindex(bool async)
    {
        await base.Substring_with_two_args_with_zero_startindex(async);

        AssertSql(
            """
SELECT SUBSTRING("c"."ContactName", 0 + 1, 3)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task Substring_with_two_args_with_zero_length(bool async)
    {
        await base.Substring_with_two_args_with_zero_length(async);

        AssertSql(
            """
SELECT SUBSTRING("c"."ContactName", 2 + 1, 0)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task Substring_with_two_args_with_constant(bool async)
    {
        await base.Substring_with_two_args_with_constant(async);

        AssertSql(
            """
SELECT SUBSTRING("c"."ContactName", 1 + 1, 3)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task Substring_with_two_args_with_closure(bool async)
    {
        await base.Substring_with_two_args_with_closure(async);

        AssertSql(
            """
@__start_0='2'

SELECT SUBSTRING("c"."ContactName", @__start_0 + 1, 3)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task Substring_with_two_args_with_Index_of(bool async)
    {
        await base.Substring_with_two_args_with_Index_of(async);

        AssertSql(
            """
SELECT SUBSTRING("c"."ContactName", (POSITION(N'a', "c"."ContactName") - 1) + 1, 3)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task IsNullOrEmpty_in_predicate(bool async)
    {
        await base.IsNullOrEmpty_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."Region" IS NULL OR ("c"."Region" LIKE N'')
""");
    }

    public override async Task IsNullOrEmpty_in_projection(bool async)
    {
        await base.IsNullOrEmpty_in_projection(async);

        AssertSql(
            """
SELECT "c"."CustomerID" AS "Id", CASE
    WHEN "c"."Region" IS NULL OR ("c"."Region" LIKE N'') THEN CAST(1 AS boolean)
    ELSE CAST(0 AS boolean)
END AS "Value"
FROM "Customers" AS "c"
""");
    }

    public override async Task IsNullOrEmpty_negated_in_predicate(bool async)
    {
        await base.IsNullOrEmpty_negated_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."Region" IS NOT NULL AND "c"."Region" NOT LIKE N''
""");
    }

    public override async Task IsNullOrEmpty_negated_in_projection(bool async)
    {
        await base.IsNullOrEmpty_negated_in_projection(async);

        AssertSql(
            """
SELECT "c"."CustomerID" AS "Id", CASE
    WHEN "c"."Region" IS NOT NULL AND "c"."Region" NOT LIKE N'' THEN CAST(1 AS boolean)
    ELSE CAST(0 AS boolean)
END AS "Value"
FROM "Customers" AS "c"
""");
    }

    public override async Task IsNullOrWhiteSpace_in_predicate(bool async)
    {
        await base.IsNullOrWhiteSpace_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."Region" IS NULL OR "c"."Region" = N''
""");
    }

    public override async Task IsNullOrWhiteSpace_in_predicate_on_non_nullable_column(bool async)
    {
        await base.IsNullOrWhiteSpace_in_predicate_on_non_nullable_column(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N''
""");
    }

    public override async Task TrimStart_without_arguments_in_predicate(bool async)
    {
        await base.TrimStart_without_arguments_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE LTRIM("c"."ContactTitle") = N'Owner'
""");
    }

    public override async Task TrimStart_with_char_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.TrimStart_with_char_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task TrimStart_with_char_array_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.TrimStart_with_char_array_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task TrimEnd_without_arguments_in_predicate(bool async)
    {
        await base.TrimEnd_without_arguments_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE RTRIM("c"."ContactTitle") = N'Owner'
""");
    }

    public override async Task TrimEnd_with_char_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.TrimEnd_with_char_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task TrimEnd_with_char_array_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.TrimEnd_with_char_array_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task Trim_without_argument_in_predicate(bool async)
    {
        await base.Trim_without_argument_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE LTRIM(RTRIM("c"."ContactTitle")) = N'Owner'
""");
    }

    public override async Task Trim_with_char_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.Trim_with_char_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task Trim_with_char_array_argument_in_predicate(bool async)
    {
        // String.Trim with parameters. Issue #22927.
        await AssertTranslationFailed(() => base.Trim_with_char_array_argument_in_predicate(async));

        AssertSql();
    }

    public override async Task Order_by_length_twice(bool async)
    {
        await base.Order_by_length_twice(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
ORDER BY CAST(LENGTH("c"."CustomerID") AS integer), "c"."CustomerID"
""");
    }

    public override async Task Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(bool async)
    {
        await base.Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
ORDER BY CAST(LENGTH("c"."CustomerID") AS integer), "c"."CustomerID"
""");
    }

    public override async Task Static_string_equals_in_predicate(bool async)
    {
        await base.Static_string_equals_in_predicate(async);

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ANATR'
""");
    }

    [ActianTodo]
    public override async Task Static_equals_nullable_datetime_compared_to_non_nullable(bool async)
    {
        await base.Static_equals_nullable_datetime_compared_to_non_nullable(async);

        AssertSql(
            """
@__arg_0='1996-07-04T00:00:00.0000000' (DbType = DateTime)

SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" = @__arg_0
""");
    }

    public override async Task Static_equals_int_compared_to_long(bool async)
    {
        await base.Static_equals_int_compared_to_long(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE 0 = 1
""");
    }

    [ActianTodo]
    public override async Task Where_DateOnly_FromDateTime(bool async)
    {
        await base.Where_DateOnly_FromDateTime(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderDate" IS NOT NULL AND CAST("o"."OrderDate" AS date) = '1996-09-16'
""");
    }

    [ActianTodo]
    public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice(bool async)
        => await base.Projecting_Math_Truncate_and_ordering_by_it_twice(async);

    // issue #16038
    //            AssertSql(
    //                @"SELECT ROUND(CAST("o"."OrderID" AS float), 0, 1) AS "A"
    //FROM "Orders" AS "o"
    //WHERE "o"."OrderID" < 10250
    //ORDER BY "A"");
    [ActianTodo]
    public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice2(bool async)
        => await base.Projecting_Math_Truncate_and_ordering_by_it_twice2(async);

    // issue #16038
    //            AssertSql(
    //                @"SELECT ROUND(CAST("o"."OrderID" AS float), 0, 1) AS "A"
    //FROM "Orders" AS "o"
    //WHERE "o"."OrderID" < 10250
    //ORDER BY "A" DESC");
    [ActianTodo]
    public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice3(bool async)
        => await base.Projecting_Math_Truncate_and_ordering_by_it_twice3(async);

    // issue #16038
    //            AssertSql(
    //                @"SELECT ROUND(CAST("o"."OrderID" AS float), 0, 1) AS "A"
    //FROM "Orders" AS "o"
    //WHERE "o"."OrderID" < 10250
    //ORDER BY "A" DESC");
    public override Task Regex_IsMatch_MethodCall(bool async)
        => AssertTranslationFailed(() => base.Regex_IsMatch_MethodCall(async));

    public override Task Regex_IsMatch_MethodCall_constant_input(bool async)
        => AssertTranslationFailed(() => base.Regex_IsMatch_MethodCall_constant_input(async));

    public override Task Datetime_subtraction_TotalDays(bool async)
        => AssertTranslationFailed(() => base.Datetime_subtraction_TotalDays(async));

    [ActianTodo]
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task StandardDeviation(bool async)
    {
        await using var ctx = CreateContext();

        var query = ctx.Set<OrderDetail>()
            .GroupBy(od => od.ProductID)
            .Select(
                g => new
                {
                    ProductID = g.Key,
                    SampleStandardDeviation = EF.Functions.StandardDeviationSample(g.Select(od => od.UnitPrice)),
                    PopulationStandardDeviation = EF.Functions.StandardDeviationPopulation(g.Select(od => od.UnitPrice))
                });

        var results = async
            ? await query.ToListAsync()
            : query.ToList();

        var product9 = results.Single(r => r.ProductID == 9);
        Assert.Equal(8.675943752699023, product9.SampleStandardDeviation.Value, 5);
        Assert.Equal(7.759999999999856, product9.PopulationStandardDeviation.Value, 5);

        AssertSql(
            """
SELECT "o"."ProductID", STDEV("o"."UnitPrice") AS "SampleStandardDeviation", STDEVP("o"."UnitPrice") AS "PopulationStandardDeviation"
FROM "Order Details" AS "o"
GROUP BY "o"."ProductID"
""");
    }

    [ActianTodo]
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Variance(bool async)
    {
        await using var ctx = CreateContext();

        var query = ctx.Set<OrderDetail>()
            .GroupBy(od => od.ProductID)
            .Select(
                g => new
                {
                    ProductID = g.Key,
                    SampleStandardDeviation = EF.Functions.VarianceSample(g.Select(od => od.UnitPrice)),
                    PopulationStandardDeviation = EF.Functions.VariancePopulation(g.Select(od => od.UnitPrice))
                });

        var results = async
            ? await query.ToListAsync()
            : query.ToList();

        var product9 = results.Single(r => r.ProductID == 9);
        Assert.Equal(75.2719999999972, product9.SampleStandardDeviation.Value, 5);
        Assert.Equal(60.217599999997766, product9.PopulationStandardDeviation.Value, 5);

        AssertSql(
            """
SELECT "o"."ProductID", VAR("o"."UnitPrice") AS "SampleStandardDeviation", VARP("o"."UnitPrice") AS "PopulationStandardDeviation"
FROM "Order Details" AS "o"
GROUP BY "o"."ProductID"
""");
    }

    public override async Task String_StartsWith_with_StringComparison_Ordinal(bool async)
    {
        await base.String_StartsWith_with_StringComparison_Ordinal(async);

        AssertSql();
    }

    public override async Task String_StartsWith_with_StringComparison_OrdinalIgnoreCase(bool async)
    {
        await base.String_StartsWith_with_StringComparison_OrdinalIgnoreCase(async);

        AssertSql();
    }

    public override async Task String_EndsWith_with_StringComparison_Ordinal(bool async)
    {
        await base.String_EndsWith_with_StringComparison_Ordinal(async);

        AssertSql();
    }

    public override async Task String_EndsWith_with_StringComparison_OrdinalIgnoreCase(bool async)
    {
        await base.String_EndsWith_with_StringComparison_OrdinalIgnoreCase(async);

        AssertSql();
    }

    public override async Task String_Contains_with_StringComparison_Ordinal(bool async)
    {
        await base.String_Contains_with_StringComparison_Ordinal(async);

        AssertSql();
    }

    public override async Task String_Contains_with_StringComparison_OrdinalIgnoreCase(bool async)
    {
        await base.String_Contains_with_StringComparison_OrdinalIgnoreCase(async);

        AssertSql();
    }

    public override async Task String_StartsWith_with_StringComparison_unsupported(bool async)
    {
        await base.String_StartsWith_with_StringComparison_unsupported(async);

        AssertSql();
    }

    public override async Task String_EndsWith_with_StringComparison_unsupported(bool async)
    {
        await base.String_EndsWith_with_StringComparison_unsupported(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task String_Contains_in_projection(bool async)
    {
        await base.String_Contains_in_projection(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task String_Contains_negated_in_predicate(bool async)
    {
        await base.String_Contains_negated_in_predicate(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task String_Contains_negated_in_projection(bool async)
    {
        await base.String_Contains_negated_in_projection(async);

        AssertSql();
    }

    public override async Task String_Contains_with_StringComparison_unsupported(bool async)
    {
        await base.String_Contains_with_StringComparison_unsupported(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task String_Join_non_aggregate(bool async)
    {
        await base.String_Join_non_aggregate(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Where_math_max_nested(bool async)
    {
        await base.Where_math_max_nested(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Where_math_max_nested_twice(bool async)
    {
        await base.Where_math_max_nested_twice(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Where_math_min_nested(bool async)
    {
        await base.Where_math_min_nested(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Where_math_min_nested_twice(bool async)
    {
        await base.Where_math_min_nested_twice(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Select_ToString_IndexOf(bool async)
    {
        await base.Select_ToString_IndexOf(async);

        AssertSql();
    }

    [ActianTodo]
    public override async Task Select_IndexOf_ToString(bool async)
    {
        await base.Select_IndexOf_ToString(async);

        AssertSql();
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();
}
