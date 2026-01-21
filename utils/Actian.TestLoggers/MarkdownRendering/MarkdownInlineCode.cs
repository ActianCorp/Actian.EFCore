// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownInlineCode InlineCode(object contents)
        {
            return new MarkdownInlineCode(new MarkdownDocument(contents));
        }

        public static MarkdownInlineCode InlineCode(MarkdownBase contents)
        {
            return new MarkdownInlineCode(contents);
        }

        public static MarkdownInlineCode Code(object contents)
        {
            return InlineCode(contents);
        }

        public static MarkdownInlineCode Code(MarkdownBase contents)
        {
            return InlineCode(contents);
        }
    }

    public class MarkdownInlineCode : MarkdownBase
    {
        public MarkdownInlineCode(MarkdownBase contents)
        {
            Contents = contents;
        }

        public MarkdownBase Contents { get; }

        public MarkdownInlineCode With(
            MarkdownBase Contents = null
            )
        {
            return new MarkdownInlineCode(
                Contents ?? this.Contents
            );
        }

        public MarkdownInlineCode WithContents(MarkdownBase contents)
            => new MarkdownInlineCode(contents);

        public override void Render(TextWriter writer)
        {
            writer.Write("`");
            Contents.Render(writer);
            writer.Write("`");
        }
    }
}
