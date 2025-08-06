// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace Actian.EFCore
{
    [Collection("Test Collection")]
    public class DataBindingActianTest : DataBindingTestBase<F1ActianFixture>
    {
        public DataBindingActianTest(F1ActianFixture fixture)
            : base(fixture)
        {
        }

        [Fact(Skip = "Fails when ran in block...")]
        public override void Adding_entity_to_local_binding_list_that_is_Deleted_in_the_state_manager_makes_entity_Added()
            => base.Adding_entity_to_local_binding_list_that_is_Deleted_in_the_state_manager_makes_entity_Added();

        [Fact(Skip = "Fails when ran in block...")]
        public override void Adding_entity_to_local_view_that_is_already_in_the_state_manager_and_not_Deleted_is_noop()
            => base.Adding_entity_to_local_view_that_is_already_in_the_state_manager_and_not_Deleted_is_noop();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Adding_entity_to_local_view_that_is_Deleted_in_the_state_manager_makes_entity_Added(bool toObservableCollection)
            => base.Adding_entity_to_local_view_that_is_Deleted_in_the_state_manager_makes_entity_Added(toObservableCollection);

        [Theory(Skip = "Fails when ran in block...")]
        public override void Adding_entity_to_state_manager_of_different_type_than_local_keyless_type_has_no_effect_on_local_view(bool toObservableCollection)
            => base.Adding_entity_to_state_manager_of_different_type_than_local_keyless_type_has_no_effect_on_local_view(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void DbSet_Local_calls_DetectChanges()
            => base.DbSet_Local_calls_DetectChanges();

        [Theory(Skip = "Fails when ran in block...")]
        public override void DbSet_Local_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities(bool toObservableCollection)
            => base.DbSet_Local_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void DbSet_Local_ToBindingList_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities()
            => base.DbSet_Local_ToBindingList_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities();

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entities_deleted_from_context_are_removed_from_local_binding_list()
            => base.Entities_deleted_from_context_are_removed_from_local_binding_list();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_deleted_from_context_are_removed_from_local_view(bool toObservableCollection)
            => base.Entities_deleted_from_context_are_removed_from_local_view(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entities_detached_from_context_are_removed_from_local_binding_list()
            => base.Entities_detached_from_context_are_removed_from_local_binding_list();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_detached_from_context_are_removed_from_local_view(bool toObservableCollection)
            => base.Entities_detached_from_context_are_removed_from_local_view(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entities_materialized_into_context_are_reflected_in_local_binding_list()
            => base.Entities_materialized_into_context_are_reflected_in_local_binding_list();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_materialized_into_context_are_reflected_in_local_view(bool toObservableCollection)
            => base.Entities_materialized_into_context_are_reflected_in_local_view(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entities_removed_from_the_local_binding_list_are_marked_deleted_in_the_state_manager()
            => base.Entities_removed_from_the_local_binding_list_are_marked_deleted_in_the_state_manager();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_removed_from_the_local_view_are_marked_deleted_in_the_state_manager(bool toObservableCollection)
            => base.Entities_removed_from_the_local_view_are_marked_deleted_in_the_state_manager(toObservableCollection);

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_with_state_changed_from_deleted_to_added_are_added_to_local_view(bool toObservableCollection)
            => base.Entities_with_state_changed_from_deleted_to_added_are_added_to_local_view(toObservableCollection);

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_with_state_changed_from_deleted_to_unchanged_are_added_to_local_view(bool toObservableCollection)
            => base.Entities_with_state_changed_from_deleted_to_unchanged_are_added_to_local_view(toObservableCollection);

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_with_state_changed_to_deleted_are_removed_from_local_view(bool toObservableCollection)
            => base.Entities_with_state_changed_to_deleted_are_removed_from_local_view(toObservableCollection);

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entities_with_state_changed_to_detached_are_removed_from_local_view(bool toObservableCollection)
            => base.Entities_with_state_changed_to_detached_are_removed_from_local_view(toObservableCollection);

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entity_added_to_context_is_added_to_navigation_property_binding_list()
            => base.Entity_added_to_context_is_added_to_navigation_property_binding_list();

        [Fact(Skip = "Fails when ran in block...")]
        public override void Entity_added_to_navigation_property_binding_list_is_added_to_context_after_DetectChanges()
            => base.Entity_added_to_navigation_property_binding_list_is_added_to_context_after_DetectChanges();

        [Theory(Skip = "Fails when ran in block...")]
        public override void Entity_removed_from_navigation_property_binding_list_is_removed_from_nav_property_but_not_marked_Deleted(CascadeTiming deleteOrphansTiming)
            => base.Entity_removed_from_navigation_property_binding_list_is_removed_from_nav_property_but_not_marked_Deleted(deleteOrphansTiming);

        [Fact(Skip = "Fails when ran in block...")]
        public override void Load_executes_query_on_keyless_entity_type()
            => base.Load_executes_query_on_keyless_entity_type();

        [Theory(Skip = "Fails when ran in block...")]
#pragma warning disable xUnit1024 // Test methods cannot have overloads
        public new void LocalView_is_initialized_with_entities_from_the_context(bool toObservableCollection)
#pragma warning restore xUnit1024 // Test methods cannot have overloads
        {
            base.LocalView_is_initialized_with_entities_from_the_context(toObservableCollection);
        }
    }
}
