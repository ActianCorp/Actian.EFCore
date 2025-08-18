// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Text.RegularExpressions;

namespace Actian.EFCore.TestUtilities
{
    public class ActianServerVersion
    {
        private static readonly Regex ServerVersionRe = new Regex(@"^(\d+)\.(\d+).\d+\s+(\S+)\s", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        public static ActianServerVersion Parse(string str)
        {
            var match = ServerVersionRe.Match(str ?? "");
            if (!match.Success)
                throw new Exception($"Could not parse server version: {str}");
            return new ActianServerVersion(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                match.Groups[3].Value
            );
        }

        public ActianServerVersion(int major, int minor, string serverType = null)
        {
            Major = major;
            Minor = minor;
            ServerType = serverType;
        }

        public int Major { get; }
        public int Minor { get; }
        public string ServerType { get; }

        public bool Supports(ActianServerVersion minimumServerVersion)
            => Major >= minimumServerVersion.Major
            && (Major > minimumServerVersion.Major || Minor >= minimumServerVersion.Minor)
            && (minimumServerVersion.ServerType is null || ServerType == minimumServerVersion.ServerType);

        public override string ToString() => ServerType is null
            ? $"{Major}.{Minor}"
            : $"{Major}.{Minor} ({ServerType})";
    }
}
