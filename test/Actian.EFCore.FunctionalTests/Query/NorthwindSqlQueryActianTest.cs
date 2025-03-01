﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Data.Common;
using System.Threading.Tasks;
using Ingres.Client;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindSqlQueryActianTest : NorthwindSqlQueryTestBase<NorthwindQueryActianFixture<NoopModelCustomizer>>
{
    public NorthwindSqlQueryActianTest(NorthwindQueryActianFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task SqlQueryRaw_over_int(bool async)
    {
        await base.SqlQueryRaw_over_int(async);

        AssertSql(
            """
SELECT "ProductID" FROM "Products"
""");
    }

    public override async Task SqlQuery_composed_Contains(bool async)
    {
        await base.SqlQuery_composed_Contains(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate"
FROM "Orders" AS "o"
WHERE "o"."OrderID" IN (
    SELECT "s"."Value"
    FROM (
        SELECT "ProductID" AS "Value" FROM "Products"
    ) AS "s"
)
""");
    }

    public override async Task SqlQuery_composed_Join(bool async)
    {
        await base.SqlQuery_composed_Join(async);

        AssertSql(
            """
SELECT "o"."OrderID", "o"."CustomerID", "o"."EmployeeID", "o"."OrderDate", CAST("s"."Value" AS integer) AS "p"
FROM "Orders" AS "o"
INNER JOIN (
    SELECT "ProductID" AS "Value" FROM "Products"
) AS "s" ON "o"."OrderID" = CAST("s"."Value" AS integer)
""");
    }

    public override async Task SqlQuery_over_int_with_parameter(bool async)
    {
        await base.SqlQuery_over_int_with_parameter(async);

        AssertSql(
            """
p0='10'

SELECT "ProductID" FROM "Products" WHERE "ProductID" = @p0
""");
    }

    protected override DbParameter CreateDbParameter(string name, object value)
        => new IngresParameter { ParameterName = name, Value = value };

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
