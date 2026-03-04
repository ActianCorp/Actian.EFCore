// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using DiffPlex;

namespace Actian.EFCore
{
    public class TextDiffLine
    {
        public static List<TextDiffLine> Diff(string text1, string text2, bool normalize = true)
        {
            static IEnumerable<TextDiffLine> createDiff(string text1, string text2, bool normalize)
            {
                var diffResult = Differ.Instance.CreateLineDiffs(
                    text1.NormalizeText(normalize),
                    text2.NormalizeText(normalize),
                    ignoreWhitespace: false,
                    ignoreCase: false
                );

                var pos = 0;
                var oldLineNo = 1;
                var newLineNo = 1;

                foreach (var diffBlock in diffResult.DiffBlocks)
                {
                    while (pos < diffBlock.InsertStartB)
                    {
                        yield return Unchanged(diffResult.PiecesNew[pos], oldLineNo, newLineNo);
                        pos += 1;
                        oldLineNo += 1;
                        newLineNo += 1;
                    }

                    for (var i = 0; i < diffBlock.DeleteCountA; i++)
                    {
                        yield return Deleted(diffResult.PiecesOld[i + diffBlock.DeleteStartA], oldLineNo);
                        oldLineNo += 1;
                    }

                    for (var i = 0; i < diffBlock.InsertCountB; i++)
                    {
                        yield return Inserted(diffResult.PiecesNew[i + diffBlock.InsertStartB], newLineNo);
                        newLineNo += 1;
                    }

                    pos += diffBlock.InsertCountB;
                }

                while (pos < diffResult.PiecesNew.Count)
                {
                    yield return Unchanged(diffResult.PiecesNew[pos], oldLineNo, newLineNo);
                    pos += 1;
                    oldLineNo += 1;
                    newLineNo += 1;
                }
            }
            return createDiff(text1, text2, normalize).ToList();
        }

        public static TextDiffLine Unchanged(string line, int oldLineNo, int newLineNo)
            => new TextDiffLine(TextDiffChange.Unchanged, line, oldLineNo, newLineNo);

        public static TextDiffLine Deleted(string line, int oldLineNo)
            => new TextDiffLine(TextDiffChange.Deleted, line, oldLineNo, null);

        public static TextDiffLine Inserted(string line, int newLineNo)
            => new TextDiffLine(TextDiffChange.Inserted, line, null, newLineNo);

        private TextDiffLine(TextDiffChange change, string line, int? oldLineNo, int? newLineNo)
        {
            Change = change;
            OldLineNo = oldLineNo;
            NewLineNo = newLineNo;
            Line = line;
        }

        public TextDiffChange Change { get; }
        public string Line { get; }
        public int? OldLineNo { get; }
        public int? NewLineNo { get; }
        public bool IsUnchanged => Change == TextDiffChange.Unchanged;
        public bool IsChanged => !IsUnchanged;

        public string Format() => Change switch
        {
            TextDiffChange.Unchanged => $"  {Line}",
            TextDiffChange.Inserted => $"+ {Line}",
            TextDiffChange.Deleted => $"- {Line}",
            _ => $"  {Line}"
        };

        public string Format(int lineNoWidth, Func<string, string> formatLineNumbers = null)
        {
            lineNoWidth = Math.Max(lineNoWidth, 3);
            var lineNoFormat = $"{{{0}:{new string('0', lineNoWidth)}}}";

            var oldLineNo = OldLineNo is null
                ? new string('-', lineNoWidth)
                : string.Format(lineNoFormat, OldLineNo);

            var newLineNo = NewLineNo is null
                ? new string('-', lineNoWidth)
                : string.Format(lineNoFormat, NewLineNo);

            var lineNumbers = formatLineNumbers is null
                ? $"{oldLineNo} {newLineNo}"
                : formatLineNumbers($"{oldLineNo} {newLineNo}");

            return $"{lineNumbers} {Line}";
        }

        public override string ToString()
        {
            return Format(new[] { OldLineNo?.ToString() ?? "0", NewLineNo?.ToString() ?? "0" }
                .Select(x => x.Length)
                .Max()
            );
        }
    }
}
