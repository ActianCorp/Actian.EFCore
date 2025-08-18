// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;

namespace Actian.TestLoggers
{
    internal static class ActianTestMessages
    {
        private const string ActianServerCaption = "### Actian Server:";
        private const string ActianServerPortCaption = "### Actian Server Port:";
        private const string ActianServerVersionCaption = "### Actian Server Version:";
        private const string ActianServerCompatibiltyCaption = "### Actian Server Compatibilty:";
        private const string TestClassImplementsCaption = "### Test class implements:";

        private static readonly string[] Captions = new[]
        {
            ActianServerCaption,
            ActianServerPortCaption,
            ActianServerVersionCaption,
            ActianServerCompatibiltyCaption,
            TestClassImplementsCaption
        };

        public static bool IsTestMessage(string line)
            => Captions.Any(caption => line.StartsWith(caption, StringComparison.InvariantCultureIgnoreCase));

        public static bool IsActianServer(string line, out string value)
            => IsTestMessage(ActianServerCaption, line, out value);

        public static bool IsActianServerPort(string line, out string value)
            => IsTestMessage(ActianServerPortCaption, line, out value);

        public static bool IsActianServerVersion(string line, out string value)
            => IsTestMessage(ActianServerVersionCaption, line, out value);

        public static bool IsActianServerCompatibilty(string line, out string value)
            => IsTestMessage(ActianServerCompatibiltyCaption, line, out value);

        public static bool IsTestClassImplements(string line, out string value)
            => IsTestMessage(TestClassImplementsCaption, line, out value);

        private static bool IsTestMessage(string caption, string line, out string value)
        {
            if (line.StartsWith(caption, StringComparison.InvariantCultureIgnoreCase))
            {
                value = line.Substring(caption.Length).Trim();
                return true;
            }
            value = null;
            return false;
        }
    }
}
