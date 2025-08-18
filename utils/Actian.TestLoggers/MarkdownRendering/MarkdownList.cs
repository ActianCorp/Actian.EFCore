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
        public static MarkdownList<TItem> List<TItem>(IEnumerable<TItem> items = null, Func<TItem, object> getValue = null)
        {
            return MarkdownList<TItem>.Default.With(
                Items: items,
                GetValue: getValue
            );
        }

        public static MarkdownList<TItem> List<TItem>(params TItem[] items)
        {
            return MarkdownList<TItem>.Default.With(
                Items: items
            );
        }
    }

    public enum MarkdownListType
    {
        List,
        NumberedList,
        Paragraphs
    }

    public class MarkdownList<TItem> : MarkdownBase
    {
        public static readonly MarkdownList<TItem> Default = new MarkdownList<TItem>();

        private MarkdownList() { }

        private MarkdownList(
            MarkdownBase prefix,
            MarkdownBase postfix,
            MarkdownBase ifEmpty,
            MarkdownListType listType,
            IEnumerable<TItem> items,
            Func<TItem, object> getValue
            )
        {
            Prefix = prefix;
            Postfix = postfix;
            IfEmpty = ifEmpty;
            ListType = listType;
            Items = items?.ToImmutableList() ?? ImmutableList<TItem>.Empty;
            GetValue = getValue ?? (item => item);
        }

        public MarkdownBase Prefix { get; } = null;
        public MarkdownBase Postfix { get; } = null;
        public MarkdownBase IfEmpty { get; } = null;
        public MarkdownListType ListType { get; } = MarkdownListType.List;
        public ImmutableList<TItem> Items { get; } = ImmutableList<TItem>.Empty;
        public Func<TItem, object> GetValue { get; } = item => item;

        public MarkdownList<TItem> With(
            MarkdownBase Prefix = null,
            MarkdownBase Postfix = null,
            MarkdownBase IfEmpty = null,
            MarkdownListType? ListType = null,
            IEnumerable<TItem> Items = null,
            Func<TItem, object> GetValue = null
            )
        {
            return new MarkdownList<TItem>(
                Prefix ?? this.Prefix,
                Postfix ?? this.Postfix,
                IfEmpty ?? this.IfEmpty,
                ListType ?? this.ListType,
                Items ?? this.Items,
                GetValue ?? this.GetValue
            );
        }

        public MarkdownList<TItem> WithPrefix(MarkdownBase prefix)
            => new MarkdownList<TItem>(prefix, Postfix, IfEmpty, ListType, Items, GetValue);

        public MarkdownList<TItem> WithPostfix(MarkdownBase postfix)
            => new MarkdownList<TItem>(Prefix, postfix, IfEmpty, ListType, Items, GetValue);

        public MarkdownList<TItem> WithIfEmpty(MarkdownBase ifEmpty)
            => new MarkdownList<TItem>(Prefix, Postfix, ifEmpty, ListType, Items, GetValue);

        public MarkdownList<TItem> WithListType(MarkdownListType listType)
            => new MarkdownList<TItem>(Prefix, Postfix, IfEmpty, listType, Items, GetValue);

        public MarkdownList<TItem> WithItems(IEnumerable<TItem> items)
            => new MarkdownList<TItem>(Prefix, Postfix, IfEmpty, ListType, items, GetValue);

        public MarkdownList<TItem> WithGetValue(Func<TItem, object> getValue)
            => new MarkdownList<TItem>(Prefix, Postfix, IfEmpty, ListType, Items, getValue);

        public MarkdownList<TItem> AddItem(TItem item)
            => WithItems(Items.Add(item));

        public MarkdownList<TItem> AddItems(params TItem[] items)
            => WithItems(Items.AddRange(items));

        public MarkdownList<TItem> AddItems(IEnumerable<TItem> items)
            => WithItems(Items.AddRange(items));

        public override void Render(TextWriter writer)
        {
            if (!Items.Any())
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

            foreach (var (item, no) in Items.Select((i, index) => (item: i, no: index + 1)))
            {
                switch (ListType)
                {
                    case MarkdownListType.List:
                        writer.Write("- ");
                        break;
                    case MarkdownListType.NumberedList:
                        writer.Write($"{no}. ");
                        break;
                }
                var value = GetValue(item);
                if (value is MarkdownBase markdown)
                {
                    markdown.Render(writer);
                }
                else if (value is string str)
                {
                    writer.Write(str);
                }
                else if (value != null)
                {
                    writer.Write(value.ToString());
                }
                writer.WriteLine();
                switch (ListType)
                {
                    case MarkdownListType.Paragraphs:
                        writer.WriteLine();
                        break;
                }
            }
            writer.WriteLine();
            writer.WriteLine();

            if (Postfix != null)
            {
                Postfix.Render(writer);
                writer.WriteLine();
            }
        }
    }
}
