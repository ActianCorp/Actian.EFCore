﻿using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindCompiledQueryActianTest : NorthwindCompiledQueryTestBase<NorthwindQueryActianFixture<NoopModelCustomizer>>
{
    public NorthwindCompiledQueryActianTest(
        NorthwindQueryActianFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override void DbSet_query()
    {
        base.DbSet_query();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""");
    }

    public override void DbSet_query_first()
    {
        base.DbSet_query_first();

        AssertSql(
            """
SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
ORDER BY "c"."CustomerID"
""");
    }

    public override void Query_ending_with_include()
    {
        base.Query_ending_with_include();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
ORDER BY "c"."CustomerID"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
ORDER BY "c"."CustomerID"
""");
    }

    public override void Untyped_context()
    {
        base.Untyped_context();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""");
    }

    public override void Query_with_single_parameter()
    {
        base.Query_with_single_parameter();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override void First_query_with_single_parameter()
    {
        base.First_query_with_single_parameter();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override void Query_with_two_parameters()
    {
        base.Query_with_two_parameters();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override void Query_with_three_parameters()
    {
        base.Query_with_three_parameters();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override void Query_with_contains()
    {
        base.Query_with_contains();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ANATR'
""");
    }

    public override void Query_with_closure()
    {
        base.Query_with_closure();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override void Compiled_query_when_does_not_end_in_query_operator()
    {
        base.Compiled_query_when_does_not_end_in_query_operator();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT COUNT(*)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override async Task Compiled_query_with_max_parameters()
    {
        await base.Compiled_query_with_max_parameters();

        AssertSql(
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
ORDER BY "c"."CustomerID"
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT COUNT(*)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
ORDER BY "c"."CustomerID"
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'
@__s15='RANDM'

SELECT COUNT(*)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14 OR "c"."CustomerID" = @__s15
""",
            //
            """
@__s1='ALFKI'
@__s2='ANATR'
@__s3='ANTON'
@__s4='AROUT'
@__s5='BERGS'
@__s6='BLAUS'
@__s7='BLONP'
@__s8='BOLID'
@__s9='BONAP'
@__s10='BSBEV'
@__s11='CACTU'
@__s12='CENTC'
@__s13='CHOPS'
@__s14='CONSH'

SELECT COUNT(*)
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__s1 OR "c"."CustomerID" = @__s2 OR "c"."CustomerID" = @__s3 OR "c"."CustomerID" = @__s4 OR "c"."CustomerID" = @__s5 OR "c"."CustomerID" = @__s6 OR "c"."CustomerID" = @__s7 OR "c"."CustomerID" = @__s8 OR "c"."CustomerID" = @__s9 OR "c"."CustomerID" = @__s10 OR "c"."CustomerID" = @__s11 OR "c"."CustomerID" = @__s12 OR "c"."CustomerID" = @__s13 OR "c"."CustomerID" = @__s14
""");
    }

    [ActianTodo]
    public override void Query_with_array_parameter()
    {
        base.Query_with_array_parameter();

        AssertSql(
            """
@__args='["ALFKI"]' (Size = 4000)

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = JSON_VALUE(@__args, '$"0]')
""",
            //
            """
@__args='["ANATR"]' (Size = 4000)

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = JSON_VALUE(@__args, '$"0]')
""");
    }

    [ActianTodo]
    public override async Task Query_with_array_parameter_async()
    {
        await base.Query_with_array_parameter_async();

        AssertSql(
            """
@__args='["ALFKI"]' (Size = 4000)

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = JSON_VALUE(@__args, '$"0]')
""",
            //
            """
@__args='["ANATR"]' (Size = 4000)

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = JSON_VALUE(@__args, '$"0]')
""");
    }

    public override void Multiple_queries()
    {
        base.Multiple_queries();

        AssertSql(
            """
SELECT FIRST 1 "c"."CustomerID"
FROM "Customers" AS "c"
ORDER BY "c"."CustomerID"
""",
            //
            """
SELECT FIRST 1 "o"."CustomerID"
FROM "Orders" AS "o"
ORDER BY "o"."CustomerID"
""",
            //
            """
SELECT FIRST 1 "c"."CustomerID"
FROM "Customers" AS "c"
ORDER BY "c"."CustomerID"
""",
            //
            """
SELECT FIRST 1 "o"."CustomerID"
FROM "Orders" AS "o"
ORDER BY "o"."CustomerID"
""");
    }

    public override void Compiled_query_when_using_member_on_context()
    {
        base.Compiled_query_when_using_member_on_context();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" LIKE N'A%'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" LIKE N'A%'
""");
    }

    public override async Task First_query_with_cancellation_async()
    {
        await base.First_query_with_cancellation_async();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override async Task DbSet_query_first_async()
    {
        await base.DbSet_query_first_async();

        AssertSql(
            """
SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
ORDER BY "c"."CustomerID"
""");
    }

    public override async Task First_query_with_single_parameter_async()
    {
        await base.First_query_with_single_parameter_async();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT FIRST 1 "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override async Task Keyless_query_first_async()
    {
        await base.Keyless_query_first_async();

        AssertSql(
            """
SELECT FIRST 1 "m"."Address", "m"."City", "m"."CompanyName", "m"."ContactName", "m"."ContactTitle"
FROM (
    SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
) AS "m"
ORDER BY "m"."CompanyName"
""");
    }

    public override async Task Query_with_closure_async_null()
    {
        await base.Query_with_closure_async_null();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE 0 = 1
""");
    }

    public override async Task Query_with_three_parameters_async()
    {
        await base.Query_with_three_parameters_async();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override async Task Query_with_two_parameters_async()
    {
        await base.Query_with_two_parameters_async();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override async Task Keyless_query_async()
    {
        await base.Keyless_query_async();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
""");
    }

    public override async Task Query_with_single_parameter_async()
    {
        await base.Query_with_single_parameter_async();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = @__customerID
""");
    }

    public override void Keyless_query_first()
    {
        base.Keyless_query_first();

        AssertSql(
            """
SELECT FIRST 1 "m"."Address", "m"."City", "m"."CompanyName", "m"."ContactName", "m"."ContactTitle"
FROM (
    SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
) AS "m"
ORDER BY "m"."CompanyName"
""");
    }

    public override void Query_with_closure_null()
    {
        base.Query_with_closure_null();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE 0 = 1
""");
    }

    public override async Task Query_with_closure_async()
    {
        await base.Query_with_closure_async();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
WHERE "c"."CustomerID" = N'ALFKI'
""");
    }

    public override async Task Untyped_context_async()
    {
        await base.Untyped_context_async();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""");
    }

    public override async Task DbSet_query_async()
    {
        await base.DbSet_query_async();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region"
FROM "Customers" AS "c"
""");
    }

    public override void Keyless_query()
    {
        base.Keyless_query();

        AssertSql(
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
""",
            //
            """
SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region" FROM "Customers" AS "c"
""");
    }

    public override void Query_with_single_parameter_with_include()
    {
        base.Query_with_single_parameter_with_include();

        AssertSql(
            """
@__customerID='ALFKI'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
WHERE "c"."CustomerID" = @__customerID
ORDER BY "c"."CustomerID"
""",
            //
            """
@__customerID='ANATR'

SELECT "c"."CustomerID", "c"."Address", "c"."City", "c"."CompanyName", "c"."ContactName", "c"."ContactTitle", "c"."Country", "c"."Fax", "c"."Phone", "c"."PostalCode", "c"."Region", "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Customers" AS "c"
LEFT JOIN "Orders" AS "o" ON "c"."CustomerID" = "o"."CustomerID"
WHERE "c"."CustomerID" = @__customerID
ORDER BY "c"."CustomerID"
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
