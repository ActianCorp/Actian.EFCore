// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Sprache;

namespace Actian.EFCore.Scaffolding.Internal
{
    public class ActianForeignKeyConstraint
    {
        public static ActianForeignKeyConstraint Parse(string str)
        {
            return ActianForeignKeyConstraintParser.ForeignKeyConstraint.End().Parse(str);
        }

        public static bool TryParse(string str, out ActianForeignKeyConstraint foreignKeyConstraint)
        {
            var result = ActianForeignKeyConstraintParser.ForeignKeyConstraint.End().TryParse(str);
            foreignKeyConstraint = result.Value;
            return result.WasSuccessful;
        }

        public IEnumerable<string> Keys { get; set; }
        public string ReferencesTableSchema { get; set; }
        public string ReferencesTableName { get; set; }
        public IEnumerable<string> ReferencesKeys { get; set; }
        public ReferentialAction OnUpdate { get; set; }
        public ReferentialAction OnDelete { get; set; }
    }
}
