// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Actian.TestLoggers.MarkdownRendering;
using static Actian.TestLoggers.Constants;
using static Actian.TestLoggers.MarkdownRendering.MarkdownDocument;

namespace Actian.TestLoggers
{
    internal static class ResultRowExtensions
    {
        public static void RenderResultTable(this TextWriter writer, string itemName, bool withTotals, IEnumerable<ResultRow> results, Func<MarkdownTable<ResultRow>, MarkdownTable<ResultRow>> configure = null)
        {
            var table = Table(results);

            if (withTotals)
            {
                table = table.AddItem(ResultRow.CreateTotal(results));
            }

            table = table
                .Column("Outcome", item => item.Outcome.ToHtml())
                .Column(itemName, item => item.Formatted);

            if (configure != null)
                table = configure(table);

            table = table
                .Column("❌&nbsp;Failed", item => item.Failed.ToString("N0", US), c => c.AlignRight())
                .Column("⚠️&nbsp;Skipped", item => item.Skipped.ToString("N0", US), c => c.AlignRight())
                .Column("⚠️&nbsp;Todo", item => item.Todo.ToString("N0", US), c => c.AlignRight())
                .Column("✔️&nbsp;Passed", item => item.Passed.ToString("N0", US), c => c.AlignRight())
                .Column("Total", item => item.Total.ToString("N0", US), c => c.AlignRight());

            writer.WriteMD(table);
        }
    }

    internal class ResultRow
    {
        public static ResultRow CreateTotal(IEnumerable<ResultRow> results) => new ResultRow(
            "Total",
            results.Select(r => r.Outcome).Outcome(),
            results.Select(r => r.Total).DefaultIfEmpty(0).Sum(),
            results.Select(r => r.Passed).DefaultIfEmpty(0).Sum(),
            results.Select(r => r.Failed).DefaultIfEmpty(0).Sum(),
            results.Select(r => r.Skipped).DefaultIfEmpty(0).Sum(),
            results.Select(r => r.Todo).DefaultIfEmpty(0).Sum(),
            null,
            null,
            null
        );

        public static ResultRow Create(IEnumerable<ActianTestInstallation> installations)
            => new ResultRow("", installations.SelectMany(i => i.Results), null);

        public static ResultRow Create(ActianTestInstallation installation, string url)
            => new ResultRow(installation.ToString(), installation.Results, url)
                .WithExtra($"<br>_<small>Server Version:&nbsp;{installation.ActianServerVersion}</small>_<br>_<small>Compatibilty:&nbsp;{installation.ActianServerCompatibilty}</small>_");

        public static ResultRow Create(ActianTestProject project, string url)
            => new ResultRow(project.ProjectName, project.Results, url);

        public static ResultRow Create(ActianTestNamespace ns, string url)
            => new ResultRow(ns.Namespace, ns.Results, url);

        public static ResultRow Create(ActianTestClass testClass, string url)
        {
            var row = new ResultRow(testClass.ClassDisplayName, testClass.Results, url);
            return row;
        }

        private ResultRow(string title, IEnumerable<ActianTestResult> results, string url)
            : this(title, results.Outcome(), results.Total(), results.Passed(), results.Failed(), results.Skipped(), results.Todo(), url, "", null)
        {
        }

        private ResultRow(string title, ActianTestOutcome outcome, long total, long passed, long failed, long skipped, long todo, string url, string extra, ImmutableDictionary<string, object> parameters)
        {
            Title = title;
            Outcome = outcome;
            Total = total;
            Passed = passed;
            Failed = failed;
            Skipped = skipped;
            Todo = todo;
            Url = url;
            Extra = extra;
            Parameters = parameters ?? ImmutableDictionary<string, object>.Empty;
        }

        public string Title { get; }
        public ActianTestOutcome Outcome { get; }
        public long Total { get; }
        public long Passed { get; }
        public long Failed { get; }
        public long Skipped { get; }
        public long Todo { get; }
        public string Url { get; }
        public string Extra { get; }
        public ImmutableDictionary<string, object> Parameters { get; }

        public ResultRow WithTitle(string title)
            => new ResultRow(title, Outcome, Total, Passed, Failed, Skipped, Todo, Url, Extra, Parameters);

        public ResultRow WithExtra(string extra)
            => new ResultRow(Title, Outcome, Total, Passed, Failed, Skipped, Todo, Url, extra, Parameters);

        public ResultRow WithParameter(string key, object value)
            => new ResultRow(Title, Outcome, Total, Passed, Failed, Skipped, Todo, Url, Extra, Parameters.SetItem(key, value));

        public object Parameter(string key)
        {
            return Parameters.TryGetValue(key, out var value) ? value : "";
        }

        public IMarkdown Formatted
        {
            get
            {
                var formatted = Markdown
                    .Add(Link(Title, Url));

                if (!string.IsNullOrWhiteSpace(Extra))
                    formatted = formatted.Add(Extra);

                return formatted;
            }
        }
    }
}
