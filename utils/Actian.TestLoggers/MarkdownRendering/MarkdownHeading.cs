// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownHeading Heading(int level, object contents)
        {
            return new MarkdownHeading(level, new MarkdownDocument(contents, "\n"));
        }

        public static MarkdownHeading Heading(int level, object contents, string anchor)
        {
            return new MarkdownHeading(level, new MarkdownDocument(Anchor(anchor), contents, "\n"));
        }

        public static MarkdownHeading Heading(int level, MarkdownBase contents)
        {
            return new MarkdownHeading(level, contents);
        }

        public static MarkdownHeading Heading(int level, MarkdownBase contents, string anchor)
        {
            return new MarkdownHeading(level, new MarkdownDocument(Anchor(anchor), contents));
        }
    }

    public class MarkdownHeading : MarkdownBase
    {
        public MarkdownHeading(int level, MarkdownBase contents)
        {
            Level = level;
            Contents = contents;
        }

        public int Level { get; }
        public MarkdownBase Contents { get; }

        public MarkdownHeading With(
            int? Level = null,
            MarkdownBase Contents = null
            )
        {
            return new MarkdownHeading(
                Level ?? this.Level,
                Contents ?? this.Contents
            );
        }

        public MarkdownHeading WithLevel(int level)
            => new MarkdownHeading(level, Contents);

        public MarkdownHeading WithContents(MarkdownBase contents)
            => new MarkdownHeading(Level, contents);

        public override void Render(TextWriter writer)
        {
            writer.Write(new string('#', Level));
            writer.Write(" ");
            Contents.Render(writer);
            writer.WriteLine();
        }
    }
}
