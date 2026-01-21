// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UpdateTestTodos
{
    public class Paths
    {
        public Paths([CallerFilePath] string callerPath = null)
        {
            ActianEFCore = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(callerPath), "..", ".."));
        }

        public string ActianEFCore { get; private set; }
        public string ActianEFCoreFunctionalTests => Path.Combine(ActianEFCore, "test", "Actian.EFCore.FunctionalTests");
        public string ActianEFCoreFunctionalTestResults => Path.Combine(ActianEFCoreFunctionalTests, "TestResults", "dev.json");

        public IEnumerable<string> GetTestFiles(string dir = null)
        {
            dir ??= ActianEFCoreFunctionalTests;

            if (IsBuildDirectory(dir))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(dir, "*.cs").AsEnumerable()
                .Concat(Directory.GetDirectories(dir).SelectMany(GetTestFiles));
        }

        private bool IsBuildDirectory(string dir) => Path.GetFileName(dir) switch
        {
            "bin" or "obj" => true,
            _ => false
        };
    }
}
