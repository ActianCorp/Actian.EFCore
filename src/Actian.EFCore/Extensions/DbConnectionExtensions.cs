// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Data.Common;
using System.Linq;
using Actian.EFCore.Scaffolding.Internal;
using JetBrains.Annotations;

namespace Actian.EFCore
{
    internal static class DbConnectionExtensions
    {
        public static (ActianCasing dbNameCase, ActianCasing dbDelimitedCase) GetDbCasing([NotNull] this DbConnection connection)
        {
            return connection.Select($@"
                select dbmsinfo('db_name_case')      as db_name_case,
                       dbmsinfo('db_delimited_case') as db_delimited_case
            ", reader => (
                dbNameCase: reader.GetTrimmedChar(0).ToActianCasing(),
                dbDelimitedCase: reader.GetTrimmedChar(1).ToActianCasing()
            )).Single();
        }
    }
}
