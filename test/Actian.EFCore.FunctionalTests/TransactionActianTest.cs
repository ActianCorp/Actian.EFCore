using System.Threading.Tasks;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Actian.EFCore
{
    public class TransactionActianTest : TransactionTestBase<TransactionActianTest.TransactionActianFixture>
    {
        public TransactionActianTest(TransactionActianFixture fixture)
            : base(fixture)
        {
        }

        // Test relies on savepoints, which are disabled when MARS is enabled
        [ActianTodo]
        public override Task SaveChanges_implicitly_creates_savepoint(bool async)
        {
            return base.SaveChanges_implicitly_creates_savepoint(async);
        }

        // Savepoints cannot be released in SQL Server
        public override Task Savepoint_can_be_released(bool async)
            => Task.CompletedTask;

        // Test relies on savepoints, which are disabled when MARS is enabled
        [ActianTodo]
        public override Task SaveChanges_uses_explicit_transaction_with_failure_behavior(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.SaveChanges_uses_explicit_transaction_with_failure_behavior(async, autoTransactionBehavior);
        }

        [ActianTodo]
        [ConditionalTheory]
        [InlineData(true)]
        [InlineData(false)]
        public virtual async Task Savepoints_are_disabled_with_MARS(bool async)
        {
            await using var context = CreateContextWithConnectionString(
                ActianTestStore.CreateConnectionString(TestStore.Name, multipleActiveResultSets: true));

            await using var transaction = await context.Database.BeginTransactionAsync();

            var orderId = 300;
            foreach (var _ in context.Set<TransactionCustomer>())
            {
                await context.AddAsync(new TransactionOrder { Id = orderId++, Name = "Order " + orderId });
                if (!async)
                {
                    await context.SaveChangesAsync();
                }
                else
                {
                    context.SaveChanges();
                }
            }

            await transaction.CommitAsync();

            Assert.Contains(Fixture.ListLoggerFactory.Log, t => t.Id == ActianEventId.SavepointsDisabledBecauseOfMARS);
        }

        [ActianTodo]
        public override void BeginTransaction_can_be_used_after_ambient_transaction_ended()
        {
            base.BeginTransaction_can_be_used_after_ambient_transaction_ended();
        }

        [ActianTodo]
        public override void BeginTransaction_can_be_used_after_enlisted_transaction_ended()
        {
            base.BeginTransaction_can_be_used_after_enlisted_transaction_ended();
        }

        [ActianTodo]
        public override void BeginTransaction_can_be_used_after_enlisted_transaction_if_connection_closed()
        {
            base.BeginTransaction_can_be_used_after_enlisted_transaction_if_connection_closed();
        }

        [ActianTodo]
        public override void BeginTransaction_throws_if_ambient_transaction_started()
        {
            base.BeginTransaction_throws_if_ambient_transaction_started();
        }

        [ActianTodo]
        public override void BeginTransaction_throws_if_enlisted_in_transaction()
        {
            base.BeginTransaction_throws_if_enlisted_in_transaction();
        }

        [ActianTodo]
        public override Task Can_use_open_connection_with_started_transaction(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.Can_use_open_connection_with_started_transaction(autoTransactionBehavior);
        }

        [ActianTodo]
        public override void EnlistTransaction_throws_if_ambient_transaction_started()
        {
            base.EnlistTransaction_throws_if_ambient_transaction_started();
        }

        [ActianTodo]
        public override void EnlistTransaction_throws_if_another_transaction_started()
        {
            base.EnlistTransaction_throws_if_another_transaction_started();
        }

        [ActianTodo]
        public override void Query_uses_explicit_transaction(AutoTransactionBehavior autoTransactionBehavior)
        {
            base.Query_uses_explicit_transaction(autoTransactionBehavior);
        }

        [ActianTodo]
        public override Task QueryAsync_uses_explicit_transaction(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.QueryAsync_uses_explicit_transaction(autoTransactionBehavior);
        }

        [ActianTodo]
        public override Task RelationalTransaction_can_be_committed(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.RelationalTransaction_can_be_committed(autoTransactionBehavior);
        }

        [ActianTodo]
        public override Task RelationalTransaction_can_be_committed_from_context(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.RelationalTransaction_can_be_committed_from_context(autoTransactionBehavior);
        }

        [ActianTodo]
        public override Task RelationalTransaction_can_be_rolled_back(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.RelationalTransaction_can_be_rolled_back(autoTransactionBehavior);
        }

        [ActianTodo]
        public override Task RelationalTransaction_can_be_rolled_back_from_context(AutoTransactionBehavior autoTransactionBehavior)
        {
            return base.RelationalTransaction_can_be_rolled_back_from_context(autoTransactionBehavior);
        }

        [ActianTodo]
        public override void SaveChanges_allows_independent_ambient_transaction_commits()
        {
            base.SaveChanges_allows_independent_ambient_transaction_commits();
        }

        [ActianTodo]
        public override void SaveChanges_allows_nested_ambient_transactions()
        {
            base.SaveChanges_allows_nested_ambient_transactions();
        }

        public override async Task SaveChanges_can_be_used_with_AutoTransactionBehavior_Always(bool async)
        {
            await base.SaveChanges_can_be_used_with_AutoTransactionBehavior_Always(async);
        }

        public override async Task SaveChanges_can_be_used_with_AutoTransactionBehavior_Never(bool async)
        {
            await base.SaveChanges_can_be_used_with_AutoTransactionBehavior_Never(async);
        }

        public override async Task SaveChanges_can_be_used_with_AutoTransactionsEnabled_false(bool async)
        {
            await base.SaveChanges_can_be_used_with_AutoTransactionsEnabled_false(async);
        }

        public override async Task SaveChanges_can_be_used_with_no_savepoint(bool async)
        {
            await base.SaveChanges_can_be_used_with_no_savepoint(async);
        }

        public override async Task SaveChanges_does_not_close_connection_opened_by_user(bool async)
        {
            await base.SaveChanges_does_not_close_connection_opened_by_user(async);
        }

        public override async Task SaveChanges_false_uses_explicit_transaction_without_committing_or_accepting_changes(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_false_uses_explicit_transaction_without_committing_or_accepting_changes(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override void SaveChanges_throws_for_suppressed_ambient_transactions(bool connectionString)
        {
            base.SaveChanges_throws_for_suppressed_ambient_transactions(connectionString);
        }

        [ActianTodo]
        public override async Task SaveChanges_uses_ambient_transaction(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_ambient_transaction(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override async Task SaveChanges_uses_ambient_transaction_with_connectionString(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_ambient_transaction_with_connectionString(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override async Task SaveChanges_uses_enlisted_transaction(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_enlisted_transaction(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override void SaveChanges_uses_enlisted_transaction_after_ambient_transaction()
        {
            base.SaveChanges_uses_enlisted_transaction_after_ambient_transaction();
        }

        [ActianTodo]
        public override async Task SaveChanges_uses_enlisted_transaction_after_connection_closed(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_enlisted_transaction_after_connection_closed(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override async Task SaveChanges_uses_enlisted_transaction_connectionString(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_enlisted_transaction_connectionString(async, autoTransactionBehavior);
        }

        public override async Task SaveChanges_uses_explicit_transaction_without_committing(
            bool async,
            AutoTransactionBehavior autoTransactionBehavior)
        {
            await base.SaveChanges_uses_explicit_transaction_without_committing(async, autoTransactionBehavior);
        }

        [ActianTodo]
        public override async Task Savepoint_can_be_rolled_back(bool async)
        {
            await base.Savepoint_can_be_rolled_back(async);
        }

        [ActianTodo]
        public override async Task Savepoint_name_is_quoted(bool async)
        {
            await base.Savepoint_name_is_quoted(async);
        }

        public override async Task UseTransaction_is_no_op_if_same_DbTransaction_is_used(bool async)
        {
            await base.UseTransaction_is_no_op_if_same_DbTransaction_is_used(async);
        }

        [ActianTodo]
        public override void UseTransaction_throws_if_enlisted_in_transaction()
        {
            base.UseTransaction_throws_if_enlisted_in_transaction();
        }

        protected override bool SnapshotSupported
            => true;

        protected override bool AmbientTransactionsSupported
            => true;

        protected override DbContext CreateContextWithConnectionString()
            => CreateContextWithConnectionString(null);

        protected DbContext CreateContextWithConnectionString(string connectionString)
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseActian(
                            connectionString ?? TestStore.ConnectionString,
                            b => b.ApplyConfiguration().ExecutionStrategy(c => new ActianExecutionStrategy(c))))
                .ConfigureWarnings(b => b.Log(ActianEventId.SavepointsDisabledBecauseOfMARS))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            return new DbContext(options.Options);
        }

        public class TransactionActianFixture : TransactionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new ActianDbContextOptionsBuilder(
                        base.AddOptions(builder))
                    .ExecutionStrategy(c => new ActianExecutionStrategy(c));
                builder.ConfigureWarnings(b => b.Log(ActianEventId.SavepointsDisabledBecauseOfMARS));
                return builder;
            }
        }
    }
}
