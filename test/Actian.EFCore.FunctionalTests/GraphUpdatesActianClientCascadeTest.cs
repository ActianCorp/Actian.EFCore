using System.Linq;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Actian.EFCore
{
    public class GraphUpdatesActianClientCascadeTest : GraphUpdatesActianTestBase<
        GraphUpdatesActianClientCascadeTest.ActianFixture>
    {
        public GraphUpdatesActianClientCascadeTest(ActianFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class ActianFixture : GraphUpdatesActianFixtureBase
        {
            public override bool NoStoreCascades
                => true;

            protected override string StoreName
                => "GraphClientCascadeUpdatesTest";

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                foreach (var foreignKey in modelBuilder.Model
                             .GetEntityTypes()
                             .SelectMany(e => e.GetDeclaredForeignKeys())
                             .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade))
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                }
            }
        }

        public override Task Alternate_key_over_foreign_key_doesnt_bypass_delete_behavior(bool async)
        => ExecuteWithStrategyInTransactionAsync(
            async context =>
            {
                var parent = new NaiveParent { Children = { new SneakyChild() } };
                context.Add(parent);

                _ = async
                    ? await context.SaveChangesAsync()
                    : context.SaveChanges();

                Assert.Equal(2, context.ChangeTracker.Entries().Count());

                parent.Children.Remove(parent.Children.First());
                _ = async
                    ? await context.SaveChangesAsync()
                    : context.SaveChanges();

                Assert.Equal(1, context.ChangeTracker.Entries().Count());
            });

        [ActianTodo]
        public override async Task Can_insert_when_composite_FK_has_default_value_for_one_part(bool async)
        {
            await base.Can_insert_when_composite_FK_has_default_value_for_one_part(async);
        }

       [ActianTodo]
        public override async Task Can_insert_when_FK_has_default_value(bool async)
        {
            await base.Can_insert_when_FK_has_default_value(async);
        }

        [ActianTodo]
        public override async Task Can_insert_when_FK_has_sentinel_value(bool async)
        {
            await base.Can_insert_when_FK_has_sentinel_value(async);
        }

        public override async Task Clearing_shadow_key_owned_collection_throws(bool async, bool useUpdate, bool addNew)
        {
            await base.Clearing_shadow_key_owned_collection_throws(async, useUpdate, addNew);
        }

        public override async Task Clearing_CLR_key_owned_collection(bool async, bool useUpdate, bool addNew)
        {
            await base.Clearing_CLR_key_owned_collection(async, useUpdate, addNew);
        }

        [ActianTodo]
        public override async Task Delete_principal_with_CLR_key_owned_collection(bool async)
        {
            await base.Delete_principal_with_CLR_key_owned_collection(async);
        }

        public override async Task Delete_principal_with_shadow_key_owned_collection_throws(bool async)
        {
            await base.Delete_principal_with_shadow_key_owned_collection_throws(async);
        }

        [ActianTodo]
        public override async Task Reset_unknown_original_value_when_current_value_is_set(bool async)
        {
            await base.Reset_unknown_original_value_when_current_value_is_set(async);
        }

        public override async Task Can_insert_when_bool_PK_in_composite_key_has_sentinel_value(bool async, bool initialValue)
        {            
            if (initialValue)
                await base.Can_insert_when_bool_PK_in_composite_key_has_sentinel_value(async, initialValue);
            else //Todo
                Assert.False(initialValue);
        }

        public override async Task Can_insert_when_int_PK_in_composite_key_has_sentinel_value(bool async, int initialValue)
        {
            if (initialValue != 0)
                await base.Can_insert_when_int_PK_in_composite_key_has_sentinel_value(async, initialValue);
            else //Todo
                Assert.True(initialValue == 0 );
        }

        public override async Task Mark_explicitly_set_dependent_appropriately_with_any_inheritance_and_stable_generator(bool async, bool useAdd)
        {
            if (useAdd)
                await base.Mark_explicitly_set_dependent_appropriately_with_any_inheritance_and_stable_generator(async, useAdd);
            else //Todo
                Assert.False(useAdd);
        }

        [ConditionalFact]
        public override Task Avoid_nulling_shared_FK_property_when_deleting()
        => ExecuteWithStrategyInTransactionAsync(
            async context =>
            {
                var root = await context
                    .Set<SharedFkRoot>()
                    .Include(e => e.Parents)
                    .Include(e => e.Dependants)
                    .SingleAsync();

                Assert.Equal(3, context.ChangeTracker.Entries().Count());

                var dependent = root.Dependants.Single();
                var parent = root.Parents.Single();

                Assert.Same(root, dependent.Root);
                Assert.Same(parent, dependent.Parent);
                Assert.Same(root, parent.Root);
                Assert.Same(dependent, parent.Dependant);

                Assert.Equal(root.Id, dependent.RootId);
                Assert.Equal(root.Id, parent.RootId);
                Assert.Equal(dependent.Id, parent.DependantId);

                context.Remove(dependent);

                Assert.Equal(3, context.ChangeTracker.Entries().Count());

                if (Fixture.ForceClientNoAction)
                {
                    Assert.Equal(EntityState.Unchanged, context.Entry(root).State);
                    Assert.Equal(EntityState.Unchanged, context.Entry(parent).State);
                    Assert.Equal(EntityState.Deleted, context.Entry(dependent).State);

                    Assert.Same(root, dependent.Root);
                    Assert.Same(parent, dependent.Parent);
                    Assert.Same(root, parent.Root);
                    Assert.Same(dependent, parent.Dependant);

                    Assert.Equal(root.Id, dependent.RootId);
                    Assert.Equal(root.Id, parent.RootId);
                    Assert.Equal(parent.Id, parent.DependantId);

                    await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
                }
                else
                {
                    Assert.Same(root, dependent.Root);
                    Assert.Same(parent, dependent.Parent);
                    Assert.Same(root, parent.Root);
                    Assert.Null(parent.Dependant);

                    Assert.Equal(root.Id, dependent.RootId);
                    Assert.Equal(root.Id, parent.RootId);
                    Assert.Null(parent.DependantId);

                    await context.SaveChangesAsync();

                    Assert.Equal(2, context.ChangeTracker.Entries().Count());

                    Assert.Equal(EntityState.Unchanged, context.Entry(root).State);
                    Assert.Equal(EntityState.Unchanged, context.Entry(parent).State);
                    Assert.Equal(EntityState.Detached, context.Entry(dependent).State);

                    Assert.Same(root, dependent.Root);
                    Assert.Same(parent, dependent.Parent);
                    Assert.Same(root, parent.Root);
                    Assert.Null(parent.Dependant);

                    Assert.Equal(root.Id, dependent.RootId);
                    Assert.Equal(root.Id, parent.RootId);
                    Assert.Null(parent.DependantId);
                }
            }, async context =>
            {
                if (!Fixture.ForceClientNoAction)
                {
                    var root = await context
                        .Set<SharedFkRoot>()
                        .Include(e => e.Parents)
                        .Include(e => e.Dependants)
                        .SingleAsync();

                    Assert.Equal(2, context.ChangeTracker.Entries().Count());

                    Assert.Empty(root.Dependants);
                    var parent = root.Parents.Single();

                    Assert.Same(root, parent.Root);
                    Assert.Null(parent.Dependant);

                    Assert.Equal(root.Id, parent.RootId);
                    Assert.Null(parent.DependantId);
                }
            });

        public override async Task Avoid_nulling_shared_FK_property_when_nulling_navigation(bool nullPrincipal)
        {
            await base.Avoid_nulling_shared_FK_property_when_nulling_navigation(nullPrincipal);
        }

        public override async Task Changes_to_Added_relationships_are_picked_up(ChangeMechanism changeMechanism)
        {
            await base.Changes_to_Added_relationships_are_picked_up(changeMechanism);
        }

        public override async Task New_FK_is_not_cleared_on_old_dependent_delete(
            bool loadNewParent,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.New_FK_is_not_cleared_on_old_dependent_delete(loadNewParent, deleteOrphansTiming);
        }

        public override async Task No_fixup_to_Deleted_entities(
            CascadeTiming? deleteOrphansTiming)
        {
            await base.No_fixup_to_Deleted_entities(deleteOrphansTiming);
        }

        public override async Task Save_optional_many_to_one_dependents(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_optional_many_to_one_dependents(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_required_many_to_one_dependents(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_many_to_one_dependents(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_removed_optional_many_to_one_dependents(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_removed_optional_many_to_one_dependents(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Resetting_a_deleted_reference_fixes_up_again()
        {
            await base.Resetting_a_deleted_reference_fixes_up_again();
        }

        public override async Task Detaching_principal_entity_will_remove_references_to_it()
        {
            await base.Detaching_principal_entity_will_remove_references_to_it();
        }

        public override async Task Detaching_dependent_entity_will_not_remove_references_to_it()
        {
            await base.Detaching_dependent_entity_will_not_remove_references_to_it();
        }

        public override async Task Save_removed_required_many_to_one_dependents(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_removed_required_many_to_one_dependents(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Save_changed_optional_one_to_one(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_changed_optional_one_to_one(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_required_one_to_one_changed_by_reference(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_one_to_one_changed_by_reference(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Save_required_non_PK_one_to_one_changed_by_reference(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_non_PK_one_to_one_changed_by_reference(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Sever_optional_one_to_one(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_optional_one_to_one(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Sever_required_one_to_one(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_required_one_to_one(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Sever_required_non_PK_one_to_one(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_required_non_PK_one_to_one(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Reparent_optional_one_to_one(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_optional_one_to_one(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Reparent_required_one_to_one(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_required_one_to_one(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Reparent_required_non_PK_one_to_one(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_required_non_PK_one_to_one(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Reparent_to_different_one_to_many(
            ChangeMechanism changeMechanism,
            bool useExistingParent,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_to_different_one_to_many(changeMechanism, useExistingParent, deleteOrphansTiming);
        }

        public override async Task Reparent_one_to_many_overlapping(
            ChangeMechanism changeMechanism,
            bool useExistingParent,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_one_to_many_overlapping(changeMechanism, useExistingParent, deleteOrphansTiming);
        }

        public override async Task Reparent_dependent_one_to_many(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_dependent_one_to_many(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Reparent_dependent_one_to_many_ak(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_dependent_one_to_many_ak(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Mark_modified_one_to_many_overlapping(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Mark_modified_one_to_many_overlapping(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Save_optional_many_to_one_dependents_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_optional_many_to_one_dependents_with_alternate_key(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_required_many_to_one_dependents_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_many_to_one_dependents_with_alternate_key(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_removed_optional_many_to_one_dependents_with_alternate_key(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_removed_optional_many_to_one_dependents_with_alternate_key(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Save_removed_required_many_to_one_dependents_with_alternate_key(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_removed_required_many_to_one_dependents_with_alternate_key(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Save_changed_optional_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_changed_optional_one_to_one_with_alternate_key(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Save_changed_optional_one_to_one_with_alternate_key_in_store()
        {
            await base.Save_changed_optional_one_to_one_with_alternate_key_in_store();
        }

        public override async Task Save_required_one_to_one_changed_by_reference_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Saving_multiple_modified_entities_with_the_same_key_does_not_overflow(bool async)
        {
            await base.Saving_multiple_modified_entities_with_the_same_key_does_not_overflow(async);
        }

        public override async Task Saving_unknown_key_value_marks_it_as_unmodified(bool async)
        {
            await base.Saving_unknown_key_value_marks_it_as_unmodified(async);
        }

        public override async Task Sever_relationship_that_will_later_be_deleted(bool async)
        {
            await base.Sever_relationship_that_will_later_be_deleted(async);
        }

        public override async Task Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingEntities,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Save_required_non_PK_one_to_one_changed_by_reference_with_alternate_key(changeMechanism, useExistingEntities, deleteOrphansTiming);
        }

        public override async Task Sever_optional_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_optional_one_to_one_with_alternate_key(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Sever_required_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_required_one_to_one_with_alternate_key(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Sever_required_non_PK_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Sever_required_non_PK_one_to_one_with_alternate_key(changeMechanism, deleteOrphansTiming);
        }

        public override async Task Reparent_optional_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_optional_one_to_one_with_alternate_key(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Reparent_required_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_required_one_to_one_with_alternate_key(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Reparent_required_non_PK_one_to_one_with_alternate_key(
            ChangeMechanism changeMechanism,
            bool useExistingRoot,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Reparent_required_non_PK_one_to_one_with_alternate_key(changeMechanism, useExistingRoot, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }


        public override async Task Required_many_to_one_dependent_leaves_can_be_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependent_leaves_can_be_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_are_orphaned(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_are_orphaned(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependent_leaves_can_be_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependent_leaves_can_be_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_are_orphaned(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_are_orphaned(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_leaf_can_be_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_leaf_can_be_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Optional_one_to_one_relationships_are_one_to_one(
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_relationships_are_one_to_one(deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Optional_one_to_one_with_AK_relationships_are_one_to_one(
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_with_AK_relationships_are_one_to_one(deleteOrphansTiming);
        }        

        [ActianTodo]
        public override async Task Required_one_to_one_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_leaf_can_be_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_leaf_can_be_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_leaf_can_be_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_leaf_can_be_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_with_alternate_key_are_orphaned(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_with_alternate_key_are_orphaned(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_with_alternate_key_are_orphaned(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_with_alternate_key_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_with_alternate_key_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_one_to_one_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_one_to_one_with_alternate_key_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_with_alternate_key_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_are_orphaned_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_are_orphaned_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_are_orphaned_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_are_orphaned_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_with_alternate_key_are_orphaned_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_with_alternate_key_are_orphaned_in_store(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_with_alternate_key_are_orphaned_in_store(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_are_orphaned_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_are_orphaned_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_are_orphaned_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_are_orphaned_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_many_to_one_dependents_with_alternate_key_are_orphaned_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_many_to_one_dependents_with_alternate_key_are_orphaned_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_with_alternate_key_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Optional_one_to_one_with_alternate_key_are_orphaned_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Optional_one_to_one_with_alternate_key_are_orphaned_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_with_alternate_key_are_cascade_detached_when_Added(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_with_alternate_key_are_cascade_detached_when_Added(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_with_alternate_key_are_cascade_deleted_starting_detached(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_are_cascade_detached_when_Added(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_are_cascade_detached_when_Added(cascadeDeleteTiming, deleteOrphansTiming);
        }

        [ActianTodo]
        public override async Task Required_one_to_one_are_cascade_detached_when_Added(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_one_to_one_are_cascade_detached_when_Added(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_non_PK_one_to_one_are_cascade_detached_when_Added(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_non_PK_one_to_one_are_cascade_detached_when_Added(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Required_many_to_one_dependents_with_alternate_key_are_cascade_detached_when_Added(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Required_many_to_one_dependents_with_alternate_key_are_cascade_detached_when_Added(cascadeDeleteTiming, deleteOrphansTiming);
        }

        public override async Task Re_childing_parent_to_new_child_with_delete(
            CascadeTiming? cascadeDeleteTiming,
            CascadeTiming? deleteOrphansTiming)
        {
            await base.Re_childing_parent_to_new_child_with_delete(cascadeDeleteTiming, deleteOrphansTiming);
        }

//        [ActianTodo]
        public override async Task Sometimes_not_calling_DetectChanges_when_required_does_not_throw_for_null_ref()
        {
            await base.Sometimes_not_calling_DetectChanges_when_required_does_not_throw_for_null_ref();
        }

        public override async Task Can_add_valid_first_dependent_when_multiple_possible_principal_sides()
        {
            await base.Can_add_valid_first_dependent_when_multiple_possible_principal_sides();
        }

        public override async Task Can_add_valid_second_dependent_when_multiple_possible_principal_sides()
        {
            await base.Can_add_valid_second_dependent_when_multiple_possible_principal_sides();
        }

        public override Task Can_add_multiple_dependents_when_multiple_possible_principal_sides()
    => ExecuteWithStrategyInTransactionAsync(
        async context =>
        {
            var quizTask = new QuizTask();
            quizTask.Choices.Add(new TaskChoice());
            quizTask.Choices.Add(new TaskChoice());

            context.Add(quizTask);

            var hiddenAreaTask = new HiddenAreaTask();
            hiddenAreaTask.Choices.Add(new TaskChoice());
            hiddenAreaTask.Choices.Add(new TaskChoice());

            context.Add(hiddenAreaTask);

            Assert.True(context.ChangeTracker.HasChanges());

            await context.SaveChangesAsync();

            Assert.False(context.ChangeTracker.HasChanges());
        }, async context =>
        {
            var quizTask = await context.Set<QuizTask>().Include(e => e.Choices).SingleAsync();
            var hiddenAreaTask = await context.Set<HiddenAreaTask>().Include(e => e.Choices).SingleAsync();

            Assert.Equal(2, quizTask.Choices.Count);
            foreach (var quizTaskChoice in quizTask.Choices)
            {
                Assert.Equal(quizTask.Id, quizTaskChoice.QuestTaskId);
            }

            Assert.Equal(2, hiddenAreaTask.Choices.Count);
            foreach (var hiddenAreaTaskChoice in hiddenAreaTask.Choices)
            {
                Assert.Equal(hiddenAreaTask.Id, hiddenAreaTaskChoice.QuestTaskId);
            }

            foreach (var taskChoice in context.Set<TaskChoice>())
            {
                Assert.Equal(
                    1,
                    quizTask.Choices.Count(e => e.Id == taskChoice.Id)
                    + hiddenAreaTask.Choices.Count(e => e.Id == taskChoice.Id));
            }
        });

        public override async Task Update_principal_with_CLR_key_owned_collection(bool async)
        {
            await base.Update_principal_with_CLR_key_owned_collection(async);
        }

        public override async Task Update_principal_with_non_generated_shadow_key_owned_collection_throws(bool async, bool delete)
        {
            await base.Update_principal_with_non_generated_shadow_key_owned_collection_throws(async, delete);
        }

        public override async Task Update_principal_with_shadow_key_owned_collection_throws(bool async)
        {
            await base.Update_principal_with_shadow_key_owned_collection_throws(async);
        }

        public override async Task Update_root_by_collection_replacement_of_deleted_first_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_deleted_first_level(async);
        }

        public override async Task Update_root_by_collection_replacement_of_deleted_second_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_deleted_second_level(async);
        }

        public override async Task Update_root_by_collection_replacement_of_deleted_third_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_deleted_third_level(async);
        }

        public override async Task Update_root_by_collection_replacement_of_inserted_first_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_inserted_first_level(async);
        }

        public override async Task Update_root_by_collection_replacement_of_inserted_first_level_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_inserted_first_level_level(async);
        }

        public override async Task Update_root_by_collection_replacement_of_inserted_second_level(bool async)
        {
            await base.Update_root_by_collection_replacement_of_inserted_second_level(async);
        }
    }
}
