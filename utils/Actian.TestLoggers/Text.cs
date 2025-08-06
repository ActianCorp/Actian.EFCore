// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using System.Text;

namespace Actian.TestLoggers
{
    public static class Text
    {
        public static string Normalize(string text)
        {
            if (text is null)
                return "";

            var firstLine = -1;
            var lastLine = -1;
            var indent = -1;

            var lines = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select((line, index) =>
                {
                    line = line.TrimEnd();
                    if (line.Length > 0)
                    {
                        if (firstLine < 0)
                            firstLine = index;
                        lastLine = index;

                        var lineIndent = line.IndexOf(c => c != ' ');
                        if (lineIndent >= 0 && (indent < 0 || lineIndent < indent))
                            indent = lineIndent;
                    }
                    return line;
                })
                .ToList();

            if (firstLine < 0 || lastLine < 0)
                return "";

            if (indent < 0)
                indent = 0;

            var result = new StringBuilder();
            var first = true;
            for (var index = firstLine; index <= lastLine; index++)
            {
                if (!first)
                    result.Append('\n');
                var line = lines[index];
                if (line.Length > 0)
                    result.Append(line.Substring(indent));
                first = false;
            }
            return result.ToString();
        }
    }
}
