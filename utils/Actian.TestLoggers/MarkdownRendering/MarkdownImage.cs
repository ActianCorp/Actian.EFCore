// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownImage Image(string caption, string url)
        {
            return new MarkdownImage(caption, url);
        }
    }

    public class MarkdownImage : MarkdownBase
    {
        public MarkdownImage(string caption, string url)
        {
            Caption = caption ?? "";
            Url = url;
        }

        public string Caption { get; }
        public string Url { get; }

        public MarkdownImage With(
            string Caption = null,
            string Url = null
            )
        {
            return new MarkdownImage(
                Caption ?? this.Caption,
                Url ?? this.Url
            );
        }

        public MarkdownImage WithCaption(string caption)
            => new MarkdownImage(caption, Url);

        public MarkdownImage WithContents(string url)
            => new MarkdownImage(Caption, url);

        public override void Render(TextWriter writer)
        {
            writer.Write("![");
            writer.Write(Caption);
            writer.Write("](");
            writer.Write(Url);
            writer.Write(")");
        }
    }
}
