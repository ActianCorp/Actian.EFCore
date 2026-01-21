// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Actian.TestLoggers.MarkdownRendering
{
    partial class MarkdownDocument
    {
        public static MarkdownTable<TItem> Table<TItem>(IEnumerable<TItem> items = null)
        {
            return MarkdownTable<TItem>.Default.WithItems(items);
        }
    }

    public class MarkdownTable<TItem> : MarkdownBase
    {
        public static readonly MarkdownTable<TItem> Default = new MarkdownTable<TItem>();

        private MarkdownTable() { }

        public MarkdownTable(
            MarkdownBase prefix,
            MarkdownBase postfix,
            MarkdownBase ifEmpty,
            IEnumerable<TItem> items,
            IEnumerable<MarkdownTableColumn<TItem>> columns
            )
        {
            Prefix = prefix;
            Postfix = postfix;
            IfEmpty = ifEmpty;
            Items = items?.ToImmutableList() ?? ImmutableList<TItem>.Empty;
            Columns = columns?.ToImmutableList() ?? ImmutableList<MarkdownTableColumn<TItem>>.Empty;
        }

        public MarkdownBase Prefix { get; } = null;
        public MarkdownBase Postfix { get; } = null;
        public MarkdownBase IfEmpty { get; } = null;
        public ImmutableList<TItem> Items { get; } = ImmutableList<TItem>.Empty;
        public ImmutableList<MarkdownTableColumn<TItem>> Columns { get; } = ImmutableList<MarkdownTableColumn<TItem>>.Empty;

        public MarkdownTable<TItem> With(
            MarkdownBase Prefix = null,
            MarkdownBase Postfix = null,
            MarkdownBase IfEmpty = null,
            IEnumerable<TItem> Items = null,
            IEnumerable<MarkdownTableColumn<TItem>> Columns = null
            )
        {
            return new MarkdownTable<TItem>(
                Prefix ?? this.Prefix,
                Postfix ?? this.Postfix,
                IfEmpty ?? this.IfEmpty,
                Items ?? this.Items,
                Columns ?? this.Columns
            );
        }

        public MarkdownTable<TItem> WithPrefix(MarkdownBase prefix)
            => new MarkdownTable<TItem>(prefix, Postfix, IfEmpty, Items, Columns);

        public MarkdownTable<TItem> WithPostfix(MarkdownBase postfix)
            => new MarkdownTable<TItem>(Prefix, postfix, IfEmpty, Items, Columns);

        public MarkdownTable<TItem> WithIfEmpty(MarkdownBase ifEmpty)
            => new MarkdownTable<TItem>(Prefix, Postfix, ifEmpty, Items, Columns);

        public MarkdownTable<TItem> WithItems(IEnumerable<TItem> items)
            => new MarkdownTable<TItem>(Prefix, Postfix, IfEmpty, items, Columns);

        public MarkdownTable<TItem> WithColumns(IEnumerable<MarkdownTableColumn<TItem>> columns)
            => new MarkdownTable<TItem>(Prefix, Postfix, IfEmpty, Items, columns);

        public MarkdownTable<TItem> AddItem(TItem item)
            => WithItems(Items.Add(item));

        public MarkdownTable<TItem> AddItems(params TItem[] items)
            => WithItems(Items.AddRange(items));

        public MarkdownTable<TItem> AddItems(IEnumerable<TItem> items)
            => WithItems(Items.AddRange(items));

        public MarkdownTable<TItem> Column(string title, Func<TItem, object> getValue, Func<MarkdownTableColumn<TItem>, MarkdownTableColumn<TItem>> configure = null)
            => WithColumns(Columns.Add(MarkdownTableColumn<TItem>.Create(title, getValue, configure)));

        public MarkdownTable<TItem> Column(string title, object value, Func<MarkdownTableColumn<TItem>, MarkdownTableColumn<TItem>> configure = null)
            => Column(title, _ => value, configure);

        public override void Render(TextWriter writer)
        {
            if (!Items.Any() || !Columns.Any())
            {
                if (IfEmpty != null)
                {
                    IfEmpty.Render(writer);
                    writer.WriteLine();
                }
                return;
            }

            if (Prefix != null)
            {
                Prefix.Render(writer);
                writer.WriteLine();
            }

            foreach (var column in Columns)
            {
                writer.Write("| ");
                writer.Write(column.Title);
                writer.Write(" ");
            }
            writer.WriteLine("|");

            foreach (var column in Columns)
            {
                writer.Write("| ");
                writer.Write(column.Marker);
                writer.Write(" ");
            }
            writer.WriteLine("|");

            foreach (var item in Items)
            {
                foreach (var column in Columns)
                {
                    writer.Write("| ");
                    var value = column.GetValue(item);
                    if (value is MarkdownBase markdown)
                    {
                        writer.Write(ConvertLineBreaks(markdown.Render()));
                    }
                    else if (value is string str)
                    {
                        writer.Write(ConvertLineBreaks(str));
                    }
                    else if (value != null)
                    {
                        writer.Write(ConvertLineBreaks(value.ToString()));
                    }
                    writer.Write(" ");
                }
                writer.WriteLine("|");
            }
            writer.WriteLine();
            writer.WriteLine();

            if (Postfix != null)
            {
                Postfix.Render(writer);
                writer.WriteLine();
            }
        }

        private static string ConvertLineBreaks(string str)
        {
            if (str is null)
                return "";

            return string.Join("<br>", str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
        }
    }
}
