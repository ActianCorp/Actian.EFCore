// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Ingres.Client;

namespace Actian.EFCore.Build
{
    public static class Config
    {
        public static string ExecutingDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static string SolutionRoot => GetDirUp("Actian.EFCore.sln");
        public static string TestDatabasesPath => GetPathUp("test-databases.json");
        public static string FunctionalTestsDir => Path.Combine(SolutionRoot, "test", "Actian.EFCore.FunctionalTests");
        public static string NorthwindDir => Path.Combine(FunctionalTestsDir, "Northwind");
        public static string NorthwindSqlPath => Path.Combine(NorthwindDir, "Northwind.sql");
        public static string InstallationPath => Environment.GetEnvironmentVariable("II_SYSTEM");
        public static string InstallationCode => GetInstallationCode();
        public static string EFCoreTestConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable($"ACTIAN_TEST_CONNECTION_STRING");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception($"Test connection string not found");
                return connectionString;
            }
        }

        public static string GetConnectionString(string database, string dbmsUser = null)
        {
            var connectionStringBuilder = new IngresConnectionStringBuilder(EFCoreTestConnectionString)
            {
                Database = database
            };
            if (!string.IsNullOrWhiteSpace(dbmsUser))
            {
                connectionStringBuilder.DbmsUser = $"\"{dbmsUser}\"";
            }
            connectionStringBuilder["Trim Chars"] = true;
            return connectionStringBuilder.ConnectionString;
        }

        private static string GetDirUp(string filename)
            => Path.GetDirectoryName(GetPathUp(filename));

        private static string GetPathUp(string filename)
        {
            static string getPathUpInternal(string dir, string filename)
            {
                if (File.Exists(Path.Combine(dir, filename)))
                    return Path.Combine(dir, filename);

                if (dir == Path.GetPathRoot(dir))
                    throw new Exception($"Could not find {Path.Combine(dir, filename)}");

                return getPathUpInternal(Path.GetDirectoryName(dir), filename);
            }

            return getPathUpInternal(ExecutingDirectory, filename);
        }

        private static string GetInstallationCode()
        {
            var installationCodeRe = new Regex(@"^\s*ii\.[^.]*\.setup\.ii_installation:\s*(\S\S)\s*$", RegexOptions.Compiled);
            foreach (var line in File.ReadLines(Path.Combine(InstallationPath, "ingres", "files", "config.dat")))
            {
                var match = installationCodeRe.Match(line);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return null;
        }
    }
}
