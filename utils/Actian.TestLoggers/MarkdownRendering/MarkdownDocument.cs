// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Actian.TestLoggers.MarkdownRendering
{
    public partial class MarkdownDocument : MarkdownBase
    {
        public static readonly MarkdownDocument Markdown = new MarkdownDocument();

        public MarkdownDocument(IEnumerable<object> items)
        {
            Items = items.ToImmutableList();
        }

        public MarkdownDocument(params object[] items)
            : this(items.AsEnumerable())
        {
        }

        public ImmutableList<object> Items { get; } = ImmutableList<object>.Empty;

        public MarkdownDocument With(
            IEnumerable<object> Items = null
            )
        {
            return new MarkdownDocument(
                Items ?? this.Items
            );
        }

        public MarkdownDocument WithItems(IEnumerable<object> Items)
            => new MarkdownDocument(Items);

        public MarkdownDocument Add(IEnumerable<object> items)
            => WithItems(Items.AddRange(items));

        public MarkdownDocument Add(params object[] items)
            => WithItems(Items.AddRange(items));

        public override void Render(TextWriter writer)
        {
            foreach (var item in Items)
            {
                if (item is MarkdownBase markdown)
                {
                    markdown.Render(writer);
                }
                else if (item is string str)
                {
                    writer.Write(str);
                }
                else if (item != null)
                {
                    writer.Write(item.ToString());
                }
            }
        }
    }
}
