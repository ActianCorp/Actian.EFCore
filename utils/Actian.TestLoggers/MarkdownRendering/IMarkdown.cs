// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;

namespace Actian.TestLoggers.MarkdownRendering
{
    public interface IMarkdown
    {
        void Render(TextWriter writer);
    }

    public static class IMarkdownExtensions
    {
        public static string Render(this IMarkdown markdown)
        {
            using var writer = new StringWriter { NewLine = "\n" };
            markdown.Render(writer);
            return writer.ToString();
        }

        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public static string ToHtml(this object value, bool removeParagraph = false)
            => value is IMarkdown markdown ? markdown.ToHtml(removeParagraph) : value?.ToString() ?? "";

        public static string ToHtml(this IMarkdown markdown, bool removeParagraph = false)
            => RemoveParagraph(Markdown.ToHtml(markdown.Render(), MarkdownPipeline).Trim(), removeParagraph);

        private static string RemoveParagraph(string html, bool removeParagraph)
        {
            return removeParagraph && html.StartsWith("<p>") && html.EndsWith("</p>")
                ? html.Substring(3, html.Length - 7)
                : html;
        }

        public static MarkdownDocument Add(this IMarkdown markdown, IEnumerable<object> items)
        {
            if (markdown is MarkdownDocument markdownDocument)
            {
                return markdownDocument.Add(items);
            }
            else
            {
                return MarkdownDocument.Markdown.Add(items);
            }
        }

        public static MarkdownDocument Add(this IMarkdown markdown, params object[] items)
        {
            return markdown.Add(items.AsEnumerable());
        }
    }
}
