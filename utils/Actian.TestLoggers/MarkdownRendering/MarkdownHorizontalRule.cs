// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownHorizontalRule HorizontalRule => MarkdownHorizontalRule.Default;
    }

    public class MarkdownHorizontalRule : MarkdownBase
    {
        public static readonly MarkdownHorizontalRule Default = new MarkdownHorizontalRule();

        public override void Render(TextWriter writer)
        {
            writer.WriteLine("---");
            writer.WriteLine();
        }
    }
}
