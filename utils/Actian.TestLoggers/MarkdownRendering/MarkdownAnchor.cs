// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownAnchor Anchor(string name)
        {
            return new MarkdownAnchor(name);
        }
    }

    public class MarkdownAnchor : MarkdownBase
    {
        public MarkdownAnchor(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public MarkdownAnchor With(
            string Name = null
            )
        {
            return new MarkdownAnchor(
                Name ?? this.Name
            );
        }

        public MarkdownAnchor WithName(string name)
            => new MarkdownAnchor(name);

        public override void Render(TextWriter writer)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            writer.Write($"<a name=\"{Name}\"></a>");
        }
    }
}
