// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.IO;
using System.Linq;

namespace Actian.EFCore.TestGenerators
{
    public static class TextWriterExtensions
    {
        public static void WriteText(this TextWriter writer, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var indent = -1;

            var lines = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select((line, index) =>
                {
                    line = line.TrimEnd();
                    if (line.Length > 0)
                    {
                        var lineIndent = line.IndexOf(c => c != ' ');
                        if (lineIndent >= 0 && (indent < 0 || lineIndent < indent))
                            indent = lineIndent;
                    }
                    return line;
                })
                .ToList();

            if (lines.Count >= 2)
            {
                if (lines.First() == "")
                    lines.RemoveAt(0);
                if (lines.Last() == "")
                    lines.RemoveAt(lines.Count - 1);
            }
            else if (lines.Count == 1)
            {
                if (lines.First() == "")
                    lines.RemoveAt(0);
            }

            if (indent < 0)
                indent = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    writer.WriteLine();
                else
                    writer.WriteLine(line.Substring(indent));
            }
        }
    }
}
