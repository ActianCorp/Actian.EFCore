// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Actian.EFCore.Scaffolding.Internal;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations;
using Sprache;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore.Tests.Scaffolding
{
    public class ActianForeignKeyConstraintParser_ForeignKeyConstraint
    {
        public ActianForeignKeyConstraintParser_ForeignKeyConstraint(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Theory]
        [InlineData(
            " FOREIGN KEY (\"id\") REFERENCES \"efcore_test\".test_table4(id) on update cascade on delete set null      ",
            new[] { "id" },
            "efcore_test", "test_table4", new[] { "id" },
            ReferentialAction.Cascade, ReferentialAction.SetNull
        )]
        [InlineData(
            " FOREIGN KEY (\"id\") REFERENCES \"efcore_test\".test_table4(id) on update cascade on delete set null   with something good      ",
            new[] { "id" },
            "efcore_test", "test_table4", new[] { "id" },
            ReferentialAction.Cascade, ReferentialAction.SetNull
        )]
        [InlineData(
            " FOREIGN KEY (\"id2\") REFERENCES \"efcore_test\".test_table4 on update restrict on delete no  action           ",
            new[] { "id2" },
            "efcore_test", "test_table4", null,
            ReferentialAction.Restrict, ReferentialAction.NoAction
        )]
        [InlineData(
            " FOREIGN KEY (id2, id) REFERENCES efcore_test.test_table5(id2, id)with something good  ",
            new[] { "id2", "id" },
            "efcore_test", "test_table5", new[] { "id2", "id" },
            ReferentialAction.NoAction, ReferentialAction.NoAction
        )]
        [InlineData(
            " FOREIGN KEY (id2, id) REFERENCES efcore_test.test_table5(id2, id)  with something good ",
            new[] { "id2", "id" },
            "efcore_test", "test_table5", new[] { "id2", "id" },
            ReferentialAction.NoAction, ReferentialAction.NoAction
        )]
        public void Can_parse(
            string str,
            string[] keys,
            string referencesTableSchema,
            string referencesTableName,
            string[] referencesKeys,
            ReferentialAction onUpdate,
            ReferentialAction onDelete
            )
        {
            var actual = ActianForeignKeyConstraintParser.ForeignKeyConstraint.End().Parse(str);
            //actual.WasSuccessful.Should().Be(true, actual.Message);
            actual.Should().BeEquivalentTo(new ActianForeignKeyConstraint
            {
                Keys = keys,
                ReferencesTableSchema = referencesTableSchema,
                ReferencesTableName = referencesTableName,
                ReferencesKeys = referencesKeys,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            });
        }

        [Theory]
        [InlineData(" FOREIGN KEY (\"id\", ) REFERENCES \"efcore_test\".test_table4(id) on update cascade on delete cascade      ")]
        public void Can_not_parse(string str)
        {
            var actual = ActianForeignKeyConstraintParser.ForeignKeyConstraint.End().TryParse(str);
            actual.WasSuccessful.Should().Be(false);
        }
    }
}
