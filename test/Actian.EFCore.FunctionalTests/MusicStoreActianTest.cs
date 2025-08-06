// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable xUnit1024 // Test methods cannot have overloads
namespace Actian.EFCore
{
    public class MusicStoreActianTest : MusicStoreTestBase<MusicStoreActianTest.MusicStoreActianFixture>, IDisposable
    {
        public MusicStoreActianTest(MusicStoreActianFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture.ListLoggerFactory, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void Dispose() => Helpers.LogSql();

        [ActianTodo]
        [ConditionalFact]
        public new Task AddressAndPayment_RedirectToCompleteWhenSuccessful()
        {
            return base.AddressAndPayment_RedirectToCompleteWhenSuccessful();
        }

        [ConditionalFact]
        public new Task AddressAndPayment_ReturnsOrderIfInvalidPromoCode()
        {
            return base.AddressAndPayment_ReturnsOrderIfInvalidPromoCode();
        }

        [ConditionalFact]
        public new Task Browse_ReturnsViewWithGenre()
        {
            return base.Browse_ReturnsViewWithGenre();
        }

        [ActianTodo]
        [ConditionalFact]
        public new Task Can_add_items_to_cart()
        {
            return base.Can_add_items_to_cart();
        }

        [ConditionalFact]
        public new Task CartSummaryComponent_returns_items()
        {
            return base.CartSummaryComponent_returns_items();
        }

        [ConditionalFact]
        public new Task Cart_has_items_once_they_have_been_added()
        {
            return base.Cart_has_items_once_they_have_been_added();
        }

        [ConditionalTheory]
        [InlineData(null)]
        [InlineData("CartId_A")]
        public new Task Cart_is_empty_when_no_items_have_been_added(string cartId)
        {
            if (cartId != null)
                return base.Cart_is_empty_when_no_items_have_been_added(cartId);
            else
                return base.Cart_is_empty_when_no_items_have_been_added("CartId_A");
        }

        [ActianTodo]
        [ConditionalFact]
        public new Task Complete_ReturnsErrorIfInvalidOrder()
        {
            return base.Complete_ReturnsErrorIfInvalidOrder();
        }

        [ActianTodo]
        [ConditionalFact]
        public new Task Complete_ReturnsOrderIdIfValid()
        {
            return base.Complete_ReturnsOrderIdIfValid();
        }

        [ConditionalFact]
        public new Task Details_ReturnsAlbumDetail()
        {
            return base.Details_ReturnsAlbumDetail();
        }

        [ConditionalFact]
        public new Task GenreMenuComponent_Returns_NineGenres()
        {
            return base.GenreMenuComponent_Returns_NineGenres();
        }

        [ConditionalFact]
        public new Task Index_CreatesViewWithGenres()
        {
            return base.Index_CreatesViewWithGenres();
        }

        [ActianTodo]
        [ConditionalFact]
        public new Task Index_GetsSixTopAlbums()
        {
            return base.Index_GetsSixTopAlbums();
        }

        [ActianTodo]
        [ConditionalFact]
        public new void Music_store_project_to_mapped_entity()
        {
            base.Music_store_project_to_mapped_entity();
        }

        [ActianTodo]
        [ConditionalFact]
        public new Task RemoveFromCart_removes_items_from_cart()
        {
            return base.RemoveFromCart_removes_items_from_cart();
        }

        public class MusicStoreActianFixture : MusicStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                ActianModelTestHelpers.MaxLengthStringKeys
                    .Normalize(modelBuilder);

                ActianModelTestHelpers.Guids
                    .Normalize(modelBuilder);
            }
        }
    }
}
