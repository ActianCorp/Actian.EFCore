// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownLink Link(string caption, string url)
        {
            return new MarkdownLink(caption, url);
        }
    }

    public class MarkdownLink : MarkdownBase
    {
        public MarkdownLink(string caption, string url)
        {
            Caption = caption ?? "";
            Url = url;
        }

        public string Caption { get; }
        public string Url { get; }

        public MarkdownLink With(
            string Caption = null,
            string Url = null
            )
        {
            return new MarkdownLink(
                Caption ?? this.Caption,
                Url ?? this.Url
            );
        }

        public MarkdownLink WithCaption(string caption)
            => new MarkdownLink(caption, Url);

        public MarkdownLink WithContents(string url)
            => new MarkdownLink(Caption, url);

        public override void Render(TextWriter writer)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                writer.Write(Caption);
            }
            else
            {
                writer.Write("[");
                writer.Write(Caption);
                writer.Write("](");
                writer.Write(Url);
                writer.Write(")");
            }
        }
    }
}
