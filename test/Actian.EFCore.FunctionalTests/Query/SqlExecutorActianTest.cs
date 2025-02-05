using System.Data.Common;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Ingres.Client;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Actian.EFCore.Query
{
    public class SqlExecutorActianTest : SqlExecutorTestBase<NorthwindQueryActianFixture<SqlExecutorModelCustomizer>>
    {
        public SqlExecutorActianTest(NorthwindQueryActianFixture<SqlExecutorModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ActianTodo]
        public override async Task Executes_stored_procedure(bool async)
        {
            await base.Executes_stored_procedure(async);

            AssertSql("[Ten Most Expensive Products]");
        }

        [ActianTodo]
        public override async Task Executes_stored_procedure_with_parameter(bool async)
        {
            await base.Executes_stored_procedure_with_parameter(async);

            AssertSql(
                """
@CustomerID='ALFKI' (Nullable = false) (Size = 5)

[CustOrderHist] @CustomerID
""");
        }

        [ActianTodo]
        public override async Task Executes_stored_procedure_with_generated_parameter(bool async)
        {
            await base.Executes_stored_procedure_with_generated_parameter(async);

            AssertSql(
                """
@p0='ALFKI'

[CustOrderHist] @CustomerID = @p0
""");
        }

        [ActianTodo]
        public override async Task Query_with_parameters(bool async)
        {
            await base.Query_with_parameters(async);

            AssertSql(
                """
@p0='London'
@p1='Sales Representative'

SELECT COUNT(*) FROM "Customers" WHERE "City" = @p0 AND "ContactTitle" = @p1
""");
        }

        public override async Task Query_with_dbParameter_with_name(bool async)
        {
            await base.Query_with_dbParameter_with_name(async);

            AssertSql(
                """
@city='London' (Nullable = false)

SELECT COUNT(*) FROM "Customers" WHERE "City" = @city
""");
        }

        public override async Task Query_with_positional_dbParameter_with_name(bool async)
        {
            await base.Query_with_positional_dbParameter_with_name(async);

            AssertSql(
                """
@city='London' (Nullable = false)

SELECT COUNT(*) FROM "Customers" WHERE "City" = @city
""");
        }

        public override async Task Query_with_positional_dbParameter_without_name(bool async)
        {
            await base.Query_with_positional_dbParameter_without_name(async);

            AssertSql(
                """
@p0='London' (Nullable = false)

SELECT COUNT(*) FROM "Customers" WHERE "City" = @p0
""");
        }

        public override async Task Query_with_dbParameters_mixed(bool async)
        {
            await base.Query_with_dbParameters_mixed(async);

            AssertSql(
                """
@p0='London'
@contactTitle='Sales Representative' (Nullable = false)

SELECT COUNT(*) FROM "Customers" WHERE "City" = @p0 AND "ContactTitle" = @contactTitle
""",
                //
                """
@city='London' (Nullable = false)
@p0='Sales Representative'

SELECT COUNT(*) FROM "Customers" WHERE "City" = @city AND "ContactTitle" = @p0
""");
        }

        public override async Task Query_with_parameters_interpolated(bool async)
        {
            await base.Query_with_parameters_interpolated(async);

            AssertSql(
                """
@p0='London'
@p1='Sales Representative'

SELECT COUNT(*) FROM "Customers" WHERE "City" = @p0 AND "ContactTitle" = @p1
""");
        }

        public override async Task Query_with_DbParameters_interpolated(bool async)
        {
            await base.Query_with_DbParameters_interpolated(async);

            AssertSql(
                """
city='London' (Nullable = false)
contactTitle='Sales Representative' (Nullable = false)

SELECT COUNT(*) FROM "Customers" WHERE "City" = @city AND "ContactTitle" = @contactTitle
""");
        }

        [ActianSkipIngres]
        public override async Task Throws_on_concurrent_command(bool async)
        {
            await base.Throws_on_concurrent_command(async);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new IngresParameter { ParameterName = name, Value = value };

        protected override string TenMostExpensiveProductsSproc
            => "[Ten Most Expensive Products]";

        protected override string CustomerOrderHistorySproc
            => "[CustOrderHist] @CustomerID";

        protected override string CustomerOrderHistoryWithGeneratedParameterSproc
            => "[CustOrderHist] @CustomerID = {0}";

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
