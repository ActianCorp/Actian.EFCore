// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Runtime.CompilerServices;

namespace Actian.EFCore.TestGenerators
{
    public static class Paths
    {
        public static void Init([CallerFilePath] string callerPath = null)
        {
            ActianEFCore = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(callerPath), "..", ".."));
            Root = Path.GetFullPath(Path.Combine(ActianEFCore, "..", ".."));
        }

        public static string ActianEFCore { get; private set; }
        public static string ActianEFCoreFunctionalTests => Path.Combine(ActianEFCore, "test", "Actian.EFCore.FunctionalTests");

        public static string Root { get; private set; }
        public static string EFCore => Path.Combine(Root, "external", "efcore");
        public static string EFCoreSpecificationTests => Path.Combine(EFCore, "test", "EFCore.Specification.Tests");
        public static string EFCoreRelationalSpecificationTests => Path.Combine(EFCore, "test", "EFCore.Relational.Specification.Tests");
        public static string EFCoreSqlServerFunctionalTests => Path.Combine(EFCore, "test", "EFCore.SqlServer.FunctionalTests");
    }
}
