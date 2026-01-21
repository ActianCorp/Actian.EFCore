// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Linq;
using Actian.EFCore.Parsing.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Sprache;
using static Actian.EFCore.Parsing.Internal.ActianSqlGrammar;
using static Sprache.Parse;

namespace Actian.EFCore.Scaffolding.Internal
{
    public static class ActianForeignKeyConstraintParser
    {
        public enum RuleWhen
        {
            Update,
            Delete
        }

        public static readonly Parser<RuleWhen> When =
            IgnoreCase("update").Return(RuleWhen.Update)
            .Or(IgnoreCase("delete").Return(RuleWhen.Delete));

        public static readonly Parser<ReferentialAction> Action =
            KeyWord("cascade").Return(ReferentialAction.Cascade)
            .Or(KeyWord("set null").Return(ReferentialAction.SetNull))
            .Or(KeyWord("restrict").Return(ReferentialAction.Restrict))
            .Or(KeyWord("no action").Return(ReferentialAction.NoAction))
            .Or(KeyWord("set default").Return(ReferentialAction.SetDefault));

        public static readonly Parser<(RuleWhen when, ReferentialAction action)> ReferentialActionRule =
            from _1 in KeyWord("on")
            from when in Space.Then(When)
            from action in Space.Then(Action)
            select (when, action);

        private static readonly IEnumerable<(RuleWhen when, ReferentialAction action)> NoRules =
            Enumerable.Empty<(RuleWhen when, ReferentialAction action)>();

        public static readonly Parser<IEnumerable<(RuleWhen when, ReferentialAction action)>> ReferentialActionRules =
            from first in ReferentialActionRule.Once()
            from rest in Space.Then(ReferentialActionRule).Many()
            select first.Concat(rest);

        public static readonly Parser<IEnumerable<string>> ForeignKeyKeys =
            KeyWord("foreign key")
            .Then(WSpace)
            .Then(Keys);

        public static readonly Parser<(string schema, string name)> ReferencesTable =
            KeyWord("references")
            .Then(Space)
            .Then(TableName);

        public static readonly Parser<IEnumerable<string>> ReferencesKeys =
            Keys.Optional(null);

        // We don't need with clause information
        public static Parser<Unit> WithClause =
            KeyWord("with")
            .Then(Space)
            .Then(AnyChar.Many())
            .Return(Unit.Value);

        public static readonly Parser<(IEnumerable<(RuleWhen when, ReferentialAction action)> rules, Unit with)> Slam = OneOf(
            from rules in ReferentialActionRules
            from with in Space.Then(WithClause)
            select (rules, with),

            from with in WithClause
            select (NoRules, with)
        );

        public static readonly Parser<ActianForeignKeyConstraint> ForeignKeyConstraint = (
            from keys in ForeignKeyKeys
            from referencesTable in WSpace.Then(ReferencesTable)
            from referencesKeys in WSpace.Then(ReferencesKeys)
            from referentialActions in WSpace.Then(ReferentialActionRules).Optional()
            from with in If(referentialActions.IsDefined, Space, WSpace).Then(WithClause).Optional(Unit.Value)
            select new ActianForeignKeyConstraint
            {
                Keys = keys,
                ReferencesTableSchema = referencesTable.schema,
                ReferencesTableName = referencesTable.name,
                ReferencesKeys = referencesKeys,
                OnUpdate = GetAction(referentialActions.GetOrElse(NoRules), RuleWhen.Update),
                OnDelete = GetAction(referentialActions.GetOrElse(NoRules), RuleWhen.Delete)
            }
        ).Between(WSpace);

        private static ReferentialAction GetAction(IEnumerable<(RuleWhen when, ReferentialAction action)> actions, RuleWhen when)
        {
            return actions.Where(a => a.when == when).Select(a => a.action).DefaultIfEmpty(ReferentialAction.NoAction).Last();
        }
    }
}
