// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;

namespace Actian.EFCore
{
    public static class TextDiffLineExtensions
    {
        public static TextDiffLineAssertions Should(this IEnumerable<TextDiffLine> instance)
        {
            return new TextDiffLineAssertions(instance, AssertionChain.GetOrCreate());
        }

        public static int LineNoWidth(this IEnumerable<TextDiffLine> lines)
        {
            return lines.Select(line => line.OldLineNo)
                .Concat(lines.Select(line => line.NewLineNo))
                .DefaultIfEmpty(0)
                .Max()
                .ToString()
                .Length;
        }

        public static bool HasNoDifferences(this IEnumerable<TextDiffLine> lines) => lines.All(line => line.IsUnchanged);
        public static bool HasDifferences(this IEnumerable<TextDiffLine> lines) => !HasNoDifferences(lines);

        private const string Divider = "";
        public static string Format(this IEnumerable<TextDiffLine> lines, int? context = null, Func<string, string> formatLineNumbers = null, int maxLines = int.MaxValue, int maxChanges = int.MaxValue)
        {
            static IEnumerable<string> getNoncontextLines(List<TextDiffLine> lines, Func<string, string> formatLineNumbers, int maxLines, int maxChanges)
            {
                var lineNoWidth = lines.LineNoWidth();
                var index = 0;
                var changeCount = 0;
                while (index < lines.Count && index < maxLines && changeCount < maxChanges)
                {
                    yield return lines[index].Format(lineNoWidth, formatLineNumbers);

                    if (lines[index].IsChanged)
                        changeCount += 1;

                    index += 1;
                }

                if (index < lines.Count)
                {
                    yield return "...";
                    var totalChangeCount = lines.Count(line => line.IsChanged);
                    if (changeCount < totalChangeCount)
                    {
                        yield return $"There are {lines.Count - index} more lines of which {totalChangeCount - changeCount} differ.";
                    }
                    else
                    {
                        yield return $"There are {lines.Count - index} more lines, but none of them differ.";
                    }
                }
            }

            static IEnumerable<string> getContextLines(List<TextDiffLine> lines, int context, Func<string, string> formatLineNumbers, int maxChanges)
            {
                var lineNoWidth = lines.LineNoWidth();
                var lastIndex = -1;
                var index = 0;
                var changeCount = 0;
                while (index < lines.Count && changeCount < maxChanges)
                {
                    if (lines[index].IsChanged)
                    {
                        var startIndex = Math.Max(index - context, lastIndex + 1);
                        if (startIndex > lastIndex + 1 && lastIndex >= 0)
                            yield return Divider;

                        while (startIndex < index)
                        {
                            yield return lines[startIndex].Format(lineNoWidth, formatLineNumbers);
                            startIndex += 1;
                        }

                        while (index < lines.Count && lines[index].IsChanged)
                        {
                            yield return lines[index].Format(lineNoWidth, formatLineNumbers);
                            lastIndex = index;
                            index += 1;
                            changeCount += 1;
                        }

                        var endIndex = Math.Min(index + context, lines.Count);
                        while (index < endIndex && lines[index].IsUnchanged)
                        {
                            yield return lines[index].Format(lineNoWidth, formatLineNumbers);
                            lastIndex = index;
                            index += 1;
                        }
                    }
                    else
                    {
                        index += 1;
                    }
                }

                if (index < lines.Count)
                {
                    yield return "...";
                    var totalChangeCount = lines.Count(line => line.IsChanged);
                    if (changeCount < totalChangeCount)
                    {
                        yield return $"There are {lines.Count - index} more lines of which {totalChangeCount - changeCount} differ.";
                    }
                    else
                    {
                        yield return $"There are {lines.Count - index} more lines, but none of them differ.";
                    }
                }
            }

            if (context is null)
            {
                return string.Join(
                    Environment.NewLine,
                    getNoncontextLines(lines.ToList(), formatLineNumbers, maxLines, maxChanges)
                );
            }

            return string.Join(
                Environment.NewLine,
                getContextLines(lines.ToList(), Math.Max(1, context.Value), formatLineNumbers, maxChanges)
            );
        }
    }
}
