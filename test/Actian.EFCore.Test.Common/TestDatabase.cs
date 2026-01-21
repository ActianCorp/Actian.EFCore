// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ingres.Client;

namespace Actian.EFCore.TestUtilities
{
    public class TestDatabase
    {
        private HashSet<string> _aliases = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        public const string DefaultDbmsUser = "dbo";
        public const bool Pooling = true;

        public string Name { get; set; }
        public string DbmsUser { get; set; } = DefaultDbmsUser;
        public bool Drop { get; set; } = false;
        public bool Create { get; set; } = false;
        public bool Valid { get; set; } = false;
        public bool IsTemplateDatabase { get; set; } = false;
        public IEnumerable<string> Messages { get; set; }

        public IEnumerable<string> Aliases
        {
            get => _aliases;
            set => _aliases = new HashSet<string>(value ?? Enumerable.Empty<string>(), StringComparer.InvariantCultureIgnoreCase);
        }

        public string ConnectionString
        {
            get
            {
                var builder = new IngresConnectionStringBuilder(TestEnvironment.EFCoreTestConnectionString)
                {
                    Database = Name,
                    DbmsUser = !string.IsNullOrWhiteSpace(DbmsUser) ? $@"""{DbmsUser}""" : null,
                    Pooling = Pooling
                };
                return builder.ConnectionString;
            }
        }

        public bool Matches(string name)
        {
            return string.Equals(name, Name, StringComparison.InvariantCultureIgnoreCase)
                || Aliases.Contains(name);
        }

        public override string ToString()
        {
            return $"{Name} (DBA: {DbmsUser})";
        }
    }
}
