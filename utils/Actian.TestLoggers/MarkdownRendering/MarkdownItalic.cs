// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownItalic Italic(object contents)
        {
            return new MarkdownItalic(new MarkdownDocument(contents));
        }

        public static MarkdownItalic Italic(MarkdownBase contents)
        {
            return new MarkdownItalic(contents);
        }
    }

    public class MarkdownItalic : MarkdownBase
    {
        public MarkdownItalic(MarkdownBase contents)
        {
            Contents = contents;
        }

        public MarkdownBase Contents { get; }

        public MarkdownItalic With(
            MarkdownBase Contents = null
            )
        {
            return new MarkdownItalic(
                Contents ?? this.Contents
            );
        }

        public MarkdownItalic WithContents(MarkdownBase contents)
            => new MarkdownItalic(contents);

        public override void Render(TextWriter writer)
        {
            writer.Write("_");
            Contents.Render(writer);
            writer.Write("_");
        }
    }
}
