// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownCodeBlock CodeBlock(object contents, string language = null)
        {
            return new MarkdownCodeBlock(new MarkdownDocument(contents), language);
        }

        public static MarkdownCodeBlock CodeBlock(MarkdownBase contents, string language = null)
        {
            return new MarkdownCodeBlock(contents, language);
        }
    }

    public class MarkdownCodeBlock : MarkdownBase
    {
        public MarkdownCodeBlock(MarkdownBase contents, string language = null)
        {
            Contents = contents;
            Language = language;
        }

        public MarkdownBase Contents { get; }
        public string Language { get; }

        public MarkdownCodeBlock With(
            MarkdownBase Contents = null,
            string Language = null
            )
        {
            return new MarkdownCodeBlock(
                Contents ?? this.Contents,
                Language ?? this.Language
            );
        }

        public MarkdownCodeBlock WithContents(MarkdownBase contents)
            => new MarkdownCodeBlock(contents, Language);

        public MarkdownCodeBlock WithLanguage(string language)
            => new MarkdownCodeBlock(Contents, language);

        public override void Render(TextWriter writer)
        {
            if (Contents is null)
                return;

            writer.Write("```");
            if (!string.IsNullOrWhiteSpace(Language))
            {
                writer.Write(Language);
            }
            writer.WriteLine();
            Contents.Render(writer);
            writer.WriteLine();
            writer.WriteLine("```");
            writer.WriteLine();
        }
    }
}
