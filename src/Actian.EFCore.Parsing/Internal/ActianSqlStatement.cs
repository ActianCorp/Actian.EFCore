// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Text;
using System.Text.RegularExpressions;

namespace Actian.EFCore.Parsing.Internal
{
    public class ActianSqlStatement
    {
        internal ActianSqlStatement(StringBuilder commandText, StringBuilder sql, bool hasStatement)
        {
            CommandText = ToString(commandText);
            Sql = ToString(sql);
            HasStatement = hasStatement;
        }

        public string CommandText { get; }
        public string Sql { get; }
        public bool HasStatement { get; }

        private static string ToString(StringBuilder text)
        {
            var result = text?.ToString() ?? "";
            result = Regex.Replace(result, @"^(?:[\t ]*[\r\n])+", string.Empty);
            result = Regex.Replace(result, @"(?:[\r\n][\t ]+)+$", string.Empty);
            return result;
        }

        private static int FirstNonNewlineChar(string str)
        {
            var index = 0;
            while (index < str.Length)
            {
                var c = str[index];
                if (c != '\r' && c != '\n')
                {
                    return index;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return CommandText;
        }
    }
}
