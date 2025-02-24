using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;
using TestEnvironment = Actian.EFCore.TestUtilities.TestEnvironment;

namespace Actian.EFCore
{
    public class WithConstructorsActianTest : WithConstructorsTestBase<WithConstructorsActianTest.WithConstructorsActianFixture>
    {
        public WithConstructorsActianTest(WithConstructorsActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture.ListLoggerFactory, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();
       
        public override void Query_with_keyless_type()
        {
            base.Query_with_keyless_type();
        }
        
        public override void Query_with_context_injected()
        {
            base.Query_with_context_injected();
        }
       
        public override void Query_with_context_injected_into_property()
        {
            base.Query_with_context_injected_into_property();
        }

        public override void Query_with_context_injected_into_constructor_with_property()
        {
            base.Query_with_context_injected_into_constructor_with_property();
        }

        [ActianTodo]
        public override Task Add_immutable_record()
        {
            return base.Add_immutable_record();
        }

        public override void Attaching_entity_sets_context()
        {
            base.Attaching_entity_sets_context();
        }

        public override void Query_with_EntityType_injected()
        {
            base.Query_with_EntityType_injected();
        }

         public override void Query_with_EntityType_injected_into_property()
        {
            base.Query_with_EntityType_injected_into_property();
        }

        public override void Query_with_EntityType_injected_into_constructor_with_property()
        {
            base.Query_with_EntityType_injected_into_constructor_with_property();
        }

        public override void Attaching_entity_sets_EntityType()
        {
            base.Attaching_entity_sets_EntityType();
        }

        public override void Query_with_StateManager_injected()
        {
            base.Query_with_StateManager_injected();
        }

        public override void Query_with_StateManager_injected_into_property()
        {
            base.Query_with_StateManager_injected_into_property();
        }

        public override void Query_with_StateManager_injected_into_constructor_with_property()
        {
            base.Query_with_StateManager_injected_into_constructor_with_property();
        }

        public override void Attaching_entity_sets_StateManager()
        {
            base.Attaching_entity_sets_StateManager();
        }

        public override void Query_with_loader_injected_for_reference()
        {
            base.Query_with_loader_injected_for_reference();
        }

        public override void Query_with_loader_injected_for_collections()
        {
            base.Query_with_loader_injected_for_collections();
        }

        public override Task Query_with_loader_injected_for_reference_async()
        {
            return base.Query_with_loader_injected_for_reference_async();
        }

        public override Task Query_with_loader_injected_for_collections_async()
        {
            return base.Query_with_loader_injected_for_collections_async();
        }

        public override void Query_with_POCO_loader_injected_for_reference()
        {
            base.Query_with_POCO_loader_injected_for_reference();
        }

        public override void Query_with_POCO_loader_injected_for_collections()
        {
            base.Query_with_POCO_loader_injected_for_collections();
        }

        public override Task Query_with_loader_delegate_injected_for_reference_async()
        {
            return base.Query_with_loader_delegate_injected_for_reference_async();
        }

        public override Task Query_with_loader_delegate_injected_for_collections_async()
        {
            return base.Query_with_loader_delegate_injected_for_collections_async();
        }

        public override void Query_with_loader_injected_into_property_for_reference()
        {
            base.Query_with_loader_injected_into_property_for_reference();
        }

        public override void Query_with_loader_injected_into_property_for_collections()
        {
            base.Query_with_loader_injected_into_property_for_collections();
        }

        public override void Attaching_entity_sets_lazy_loader()
        {
            base.Attaching_entity_sets_lazy_loader();
        }

        public override void Detaching_entity_resets_lazy_loader_so_it_can_be_reattached()
        {
            base.Detaching_entity_resets_lazy_loader_so_it_can_be_reattached();
        }

        public override void Query_with_loader_injected_into_field_for_reference()
        {
            base.Query_with_loader_injected_into_field_for_reference();
        }

        public override void Query_with_loader_injected_into_field_for_collections()
        {
            base.Query_with_loader_injected_into_field_for_collections();
        }

        public override void Attaching_entity_sets_lazy_loader_field()
        {
            base.Attaching_entity_sets_lazy_loader_field();
        }

        public override void Detaching_entity_resets_lazy_loader_field_so_it_can_be_reattached()
        {
            base.Detaching_entity_resets_lazy_loader_field_so_it_can_be_reattached();
        }

        public override void Attaching_entity_sets_lazy_loader_delegate()
        {
            base.Attaching_entity_sets_lazy_loader_delegate();
        }

        public override void Detaching_entity_resets_lazy_loader_delegate_so_it_can_be_reattached()
        {
            base.Detaching_entity_resets_lazy_loader_delegate_so_it_can_be_reattached();
        }

        public override void Query_with_loader_delegate_injected_into_property_for_reference()
        {
            base.Query_with_loader_delegate_injected_into_property_for_reference();
        }

        public override void Query_with_loader_delgate_injected_into_property_for_collections()
        {
            base.Query_with_loader_delgate_injected_into_property_for_collections();
        }

        public override Task Query_with_loader_delegate_injected_into_property_for_reference_async()
        {
            return base.Query_with_loader_delegate_injected_into_property_for_reference_async();
        }

        public override Task Query_with_loader_delegate_injected_into_property_for_collections_async()
        {
            return base.Query_with_loader_delegate_injected_into_property_for_collections_async();
        }

        public override void Query_with_loader_injected_into_property_via_constructor_for_reference()
        {
            base.Query_with_loader_injected_into_property_via_constructor_for_reference();
        }

        public override void Query_with_loader_injected_into_property_via_constructor_for_collections()
        {
            base.Query_with_loader_injected_into_property_via_constructor_for_collections();
        }

        public override void Query_with_loader_delegate_injected_into_property_via_constructor_for_reference()
        {
            base.Query_with_loader_delegate_injected_into_property_via_constructor_for_reference();
        }

        public override void Query_with_loader_delegate_injected_into_property_via_constructor_for_collections()
        {
            base.Query_with_loader_delegate_injected_into_property_via_constructor_for_collections();
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class WithConstructorsActianFixture : WithConstructorsFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

            [System.Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<BlogQuery>().HasNoKey().ToQuery(
                    () => context.Set<BlogQuery>().FromSqlRaw("SELECT * FROM Blog"));
            }
        }
    }
}
