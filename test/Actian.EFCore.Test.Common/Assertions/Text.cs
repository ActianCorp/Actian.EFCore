﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actian.EFCore.TestUtilities;

namespace Actian.EFCore
{
    public static class Text
    {
        public static IEnumerable<TextDiffLine> Diff(string text1, string text2, bool normalize = true)
        {
            return TextDiffLine.Diff(text1, text2, normalize);
        }

        public static string NormalizeText(this string text)
        {
            return text.NormalizeText(true);
        }

        public static string NormalizeText(this string text, bool normalize)
        {
            if (!normalize)
                return text;

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
