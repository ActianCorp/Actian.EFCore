using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using System.Linq;
using Xunit;


namespace Actian.EFCore
{
    public abstract class ProxyGraphUpdatesActianTest
    {
        public abstract class ProxyGraphUpdatesActianTestBase<TFixture>(TFixture fixture) : ProxyGraphUpdatesTestBase<TFixture>(fixture)
            where TFixture : ProxyGraphUpdatesActianTestBase<TFixture>.ProxyGraphUpdatesActianFixtureBase, new()
        {
            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class ProxyGraphUpdatesActianFixtureBase : ProxyGraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory
                    => (TestSqlLoggerFactory)ListLoggerFactory;

                protected override ITestStoreFactory TestStoreFactory
                    => ActianTestStoreFactory.Instance;
            }
        }

        public class LazyLoading(LazyLoading.ProxyGraphUpdatesWithLazyLoadingActianFixture fixture)
            : ProxyGraphUpdatesActianTestBase<LazyLoading.ProxyGraphUpdatesWithLazyLoadingActianFixture>(fixture)
        {
            protected override bool DoesLazyLoading
                => true;

            protected override bool DoesChangeTracking
                => false;

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_relationships_are_one_to_one()
                => base.Optional_one_to_one_relationships_are_one_to_one();

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_with_AK_relationships_are_one_to_one()
                => base.Optional_one_to_one_with_AK_relationships_are_one_to_one();

            [ActianTodo]
            public override Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted(CascadeTiming cascadeDeleteTiming, CascadeTiming deleteOrphansTiming)
                => base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);

            [ActianTodo]
            public override Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store(CascadeTiming cascadeDeleteTiming, CascadeTiming deleteOrphansTiming)
                => base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);

            [ActianTodo]
            public override Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached(CascadeTiming cascadeDeleteTiming, CascadeTiming deleteOrphansTiming)
                => base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);

            public override Task Save_removed_required_many_to_one_dependents(ChangeMechanism changeMechanism)
                => base.Save_removed_required_many_to_one_dependents(changeMechanism);

            [ActianTodo]
            public override Task Save_removed_required_many_to_one_dependents_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Save_removed_required_many_to_one_dependents_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one_with_alternate_key(changeMechanism);

            public class ProxyGraphUpdatesWithLazyLoadingActianFixture : ProxyGraphUpdatesActianFixtureBase
            {
                protected override string StoreName
                    => "ProxyGraphLazyLoadingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseLazyLoadingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UseIdentityColumns();

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class ChangeTracking(ChangeTracking.ProxyGraphUpdatesWithChangeTrackingActianFixture fixture)
            : ProxyGraphUpdatesActianTestBase<ChangeTracking.ProxyGraphUpdatesWithChangeTrackingActianFixture>(fixture)
        {
            // Needs lazy loading
            public override Task Save_two_entity_cycle_with_lazy_loading()
                => Task.CompletedTask;

            protected override bool DoesLazyLoading
                => false;

            protected override bool DoesChangeTracking
                => true;

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_relationships_are_one_to_one()
                => base.Optional_one_to_one_relationships_are_one_to_one();

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_with_AK_relationships_are_one_to_one()
                => base.Optional_one_to_one_with_AK_relationships_are_one_to_one();

            [ActianTodo]
            public override Task Save_removed_required_many_to_one_dependents(ChangeMechanism changeMechanism)
                => base.Save_removed_required_many_to_one_dependents(changeMechanism);

            [ActianTodo]
            public override Task Save_removed_required_many_to_one_dependents_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Save_removed_required_many_to_one_dependents_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one_with_alternate_key(changeMechanism);

            public class ProxyGraphUpdatesWithChangeTrackingActianFixture : ProxyGraphUpdatesActianFixtureBase
            {
                protected override string StoreName
                    => "ProxyGraphChangeTrackingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseChangeTrackingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UseIdentityColumns();

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class ChangeTrackingAndLazyLoading(
            ChangeTrackingAndLazyLoading.ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingActianFixture fixture)
            : ProxyGraphUpdatesActianTestBase<
                ChangeTrackingAndLazyLoading.ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingActianFixture>(fixture)
        {
            protected override bool DoesLazyLoading
                => true;

            protected override bool DoesChangeTracking
                => true;

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_relationships_are_one_to_one()
                => base.Optional_one_to_one_relationships_are_one_to_one();

            [ConditionalFact(Skip = "FK constraint checking. Issue #2166")]
            public override Task Optional_one_to_one_with_AK_relationships_are_one_to_one()
                => base.Optional_one_to_one_with_AK_relationships_are_one_to_one();

            [ActianTodo]
            public override Task Save_removed_required_many_to_one_dependents_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Save_removed_required_many_to_one_dependents_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Save_required_one_to_one_changed_by_reference_with_alternate_key(ChangeMechanism changeMechanism, bool useExistingEntities)
                => base.Save_required_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_non_PK_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_non_PK_one_to_one_with_alternate_key(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one(changeMechanism);

            [ActianTodo]
            public override Task Sever_required_one_to_one_with_alternate_key(ChangeMechanism changeMechanism)
                => base.Sever_required_one_to_one_with_alternate_key(changeMechanism);

            public class ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingActianFixture : ProxyGraphUpdatesActianFixtureBase
            {
                protected override string StoreName
                    => "ProxyGraphChangeTrackingAndLazyLoadingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseLazyLoadingProxies().UseChangeTrackingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UseIdentityColumns();

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }
    }
}
