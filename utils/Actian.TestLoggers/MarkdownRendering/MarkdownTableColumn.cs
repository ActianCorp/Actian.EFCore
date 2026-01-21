// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;

namespace Actian.TestLoggers.MarkdownRendering
{
    public class MarkdownTableColumn<TItem>
    {
        public static MarkdownTableColumn<TItem> Create(string title, Func<TItem, object> getValue, Func<MarkdownTableColumn<TItem>, MarkdownTableColumn<TItem>> configure)
        {
            var column = new MarkdownTableColumn<TItem>(title, getValue, Alignment.Left);
            return configure is null ? column : configure(column);
        }

        public MarkdownTableColumn(string title, Func<TItem, object> getValue, Alignment alignment)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            Alignment = alignment;
        }

        public string Title { get; }
        public Func<TItem, object> GetValue { get; }
        public Alignment Alignment { get; } = Alignment.Left;

        public MarkdownTableColumn<TItem> With(
            string Title = null,
            Func<TItem, object> GetValue = null,
            Alignment? Alignment = null
            )
        {
            return new MarkdownTableColumn<TItem>(
                Title ?? this.Title,
                GetValue ?? this.GetValue,
                Alignment ?? this.Alignment
            );
        }

        public MarkdownTableColumn<TItem> WithTitle(string title)
            => new MarkdownTableColumn<TItem>(title, GetValue, Alignment);

        public MarkdownTableColumn<TItem> WithPostfix(Func<TItem, object> getValue)
            => new MarkdownTableColumn<TItem>(Title, getValue, Alignment);

        public MarkdownTableColumn<TItem> WithAlignment(Alignment alignment)
            => new MarkdownTableColumn<TItem>(Title, GetValue, alignment);

        public MarkdownTableColumn<TItem> AlignLeft()
            => WithAlignment(Alignment.Left);

        public MarkdownTableColumn<TItem> AlignCenter()
            => WithAlignment(Alignment.Center);

        public MarkdownTableColumn<TItem> AlignRight()
            => WithAlignment(Alignment.Right);

        internal string Marker => Alignment switch
        {
            Alignment.Left => ":---",
            Alignment.Center => ":---:",
            Alignment.Right => "---:",
            _ => ":---"
        };
    }
}
