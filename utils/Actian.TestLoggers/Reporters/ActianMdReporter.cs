// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Actian.TestLoggers.MarkdownRendering;
using static System.Web.HttpUtility;
using static Actian.TestLoggers.MarkdownRendering.MarkdownDocument;

namespace Actian.TestLoggers
{
    public enum ActianMdReporterDepth
    {
        Installation = 0,
        Project = 1,
        Namespace = 2,
        Class = 3,
        Test = 4
    }

    internal static class ActianMdReporterDepthExtensions
    {
        public static ActianMdReporterDepth ToActianMdReporterDepth(this string depth) => depth?.ToLowerInvariant() switch
        {
            "installation" => ActianMdReporterDepth.Installation,
            "project" => ActianMdReporterDepth.Project,
            "namespace" => ActianMdReporterDepth.Namespace,
            "class" => ActianMdReporterDepth.Class,
            "test" => ActianMdReporterDepth.Test,
            _ => throw new Exception($"Unknown depth: {depth ?? "<null>"}")
        };
    }

    public class ActianMdReporter : IActianTestReporter
    {
        public ActianMdReporter(string logFilePath, string title, string branch, string pullRequest, string detailsUrl, ActianMdReporterDepth depth = ActianMdReporterDepth.Test)
        {
            LogFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
            Title = title;
            Branch = branch;
            PullRequest = pullRequest;
            DetailsUrl = detailsUrl;
            Depth = depth;
        }

        public string LogFilePath { get; }
        public string Title { get; }
        public string Branch { get; }
        public string PullRequest { get; }
        public string DetailsUrl { get; }
        public ActianMdReporterDepth Depth { get; }

        public void CreateReport(IEnumerable<ActianTestProject> testProjects)
        {
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

            using var writer = new StreamWriter(LogFilePath, false, new UTF8Encoding(false)) { NewLine = "\n" };
            Render(writer, testProjects
                .GroupBy(p => (p.ActianServer, p.ActianServerPort))
                .Select(ActianTestInstallation.Create)
                .OrderByDescending(p => p.ActianServerVersion)
                .ThenByDescending(p => p.ActianServerCompatibilty)
                .ThenBy(p => p.ActianServer)
                .ThenBy(p => p.ActianServerPort)
                .ToList()
            );
        }

        private void Render(TextWriter writer, IEnumerable<ActianTestInstallation> installations)
        {
            if (!string.IsNullOrWhiteSpace(Title))
                writer.WriteMD(Heading(1, Title));

            RenderParameterTable(writer,
                (Bold("Branch:"), Branch),
                (Bold("Pull Request:"), PullRequest),
                (Bold("Started:"), installations.SelectMany(i => i.Results).StartTime().Format()),
                (Bold("Finished:"), installations.SelectMany(i => i.Results).EndTime().Format()),
                (Bold("Duration:"), installations.SelectMany(i => i.Results).Duration().Format()),
                (Bold("Outcome:"), installations.SelectMany(i => i.Results).Outcome().ToHtml()),
                (Bold("Detailed report:"), Depth < ActianMdReporterDepth.Test ? Link("Detailed report", DetailsUrl) : null)
            );

            writer.RenderResultTable("Installation", false, installations
                .Select(installation => ResultRow.Create(installation, GetAnchor(AnchorText(installation), ActianMdReporterDepth.Installation)))
            );

            foreach (var installation in installations)
            {
                Render(writer, installation);
            }
        }

        private void Render(TextWriter writer, ActianTestInstallation installation)
        {
            if (Depth < ActianMdReporterDepth.Project)
                return;

            writer.WriteMD(HorizontalRule);
            writer.WriteMD(Heading(2, $"Installation: {installation}", AnchorText(installation)));

            RenderParameterTable(writer,
                (Bold("Server version:"), installation.ActianServerVersion),
                (Bold("Compatibilty:"), installation.ActianServerCompatibilty),
                (Bold("Started:"), installation.Results.StartTime().Format()),
                (Bold("Finished:"), installation.Results.EndTime().Format()),
                (Bold("Duration:"), installation.Results.Duration().Format()),
                (Bold("Outcome:"), installation.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Project", true, installation.TestProjects.OrderBy(p => p.ProjectName).Select(project => ResultRow.Create(project, GetAnchor(AnchorText(installation, project), ActianMdReporterDepth.Project))));

            foreach (var project in installation.TestProjects.OrderBy(p => p.ProjectName))
            {
                Render(writer, installation, project);
            }

            foreach (var project in installation.TestProjects.OrderBy(p => p.ProjectName))
            {
                foreach (var ns in project.TestNamespaces.OrderBy(p => p.Namespace))
                {
                    Render(writer, installation, project, ns);
                }
            }

            foreach (var project in installation.TestProjects.OrderBy(p => p.ProjectName))
            {
                foreach (var ns in project.TestNamespaces.OrderBy(p => p.Namespace))
                {
                    foreach (var testClass in ns.TestClasses.OrderBy(p => p.ClassDisplayName))
                    {
                        Render(writer, installation, project, ns, testClass);
                    }
                }
            }
        }

        private void Render(TextWriter writer, ActianTestInstallation installation, ActianTestProject project)
        {
            if (Depth < ActianMdReporterDepth.Project)
                return;

            writer.WriteMD(HorizontalRule);
            writer.WriteMD(Heading(3, $"Project: {project}", AnchorText(installation, project)));

            RenderParameterTable(writer,
                (Bold("Installation:"), Link($"{installation}", GetAnchor(AnchorText(installation), ActianMdReporterDepth.Installation))),
                (Bold("Started:"), project.Results.StartTime().Format()),
                (Bold("Finished:"), project.Results.EndTime().Format()),
                (Bold("Duration:"), project.Results.Duration().Format()),
                (Bold("Outcome:"), project.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Namespace", true, project.TestNamespaces.OrderBy(p => p.Namespace)
                .Select(ns => ResultRow.Create(ns, GetAnchor(AnchorText(installation, project, ns), ActianMdReporterDepth.Namespace)))
            );
        }

        private void Render(TextWriter writer, ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns)
        {
            if (Depth < ActianMdReporterDepth.Namespace)
                return;

            writer.WriteMD(HorizontalRule);
            writer.WriteMD(Heading(4, $"Namespace: {ns.Namespace}", AnchorText(installation, project, ns)));

            RenderParameterTable(writer,
                (Bold("Installation:"), Link($"{installation}", GetAnchor(AnchorText(installation), ActianMdReporterDepth.Installation))),
                (Bold("Project:"), Link($"{project}", GetAnchor(AnchorText(installation, project), ActianMdReporterDepth.Project))),
                (Bold("Started:"), ns.Results.StartTime().Format()),
                (Bold("Finished:"), ns.Results.EndTime().Format()),
                (Bold("Duration:"), ns.Results.Duration().Format()),
                (Bold("Outcome:"), ns.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Class", true, ns.TestClasses.OrderBy(p => p.ClassDisplayName)
                .Select(testClass => ResultRow.Create(testClass, GetAnchor(AnchorText(installation, project, ns, testClass), ActianMdReporterDepth.Class)))
            );
        }

        private void Render(TextWriter writer, ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass)
        {
            if (Depth < ActianMdReporterDepth.Test)
                return;

            writer.WriteMD(HorizontalRule);
            writer.WriteMD(Heading(4, $"Class: {testClass.ClassDisplayName}", AnchorText(installation, project, ns, testClass)));

            RenderParameterTable(writer,
                (Bold("Installation:"), Link($"{installation}", GetAnchor(AnchorText(installation), ActianMdReporterDepth.Installation))),
                (Bold("Project:"), Link($"{project}", GetAnchor(AnchorText(installation, project), ActianMdReporterDepth.Project))),
                (Bold("Namespace:"), Link($"{ns}", GetAnchor(AnchorText(installation, project, ns), ActianMdReporterDepth.Namespace))),
                (Bold("Full name:"), testClass.ClassName),
                (Bold("Started:"), testClass.Results.StartTime().Format()),
                (Bold("Finished:"), testClass.Results.EndTime().Format()),
                (Bold("Duration:"), testClass.Results.Duration().Format()),
                (Bold("Outcome:"), testClass.Results.Outcome().ToHtml())
            );

            RenderTestResults(writer, ActianTestOutcome.Failed, testClass);
            RenderTestResults(writer, ActianTestOutcome.Skipped, testClass);
            RenderTestResults(writer, ActianTestOutcome.Passed, testClass);
            RenderTestResults(writer, ActianTestOutcome.Todo, testClass);
        }

        private void RenderTestResults(TextWriter writer, ActianTestOutcome outcome, ActianTestClass testClass)
        {
            var results = testClass.Results.Where(r => r.Outcome == outcome).ToList();
            var singular = results.Count == 1;

            writer.WriteLine();
            writer.WriteLine("<details>");
            writer.Write("<summary>");
            writer.Write(outcome switch
            {
                ActianTestOutcome.Passed => "✔️",
                ActianTestOutcome.Failed => "❌",
                ActianTestOutcome.Skipped => "⚠️",
                ActianTestOutcome.Todo => "⚠️",
                _ => ""
            });
            writer.Write(" ");
            writer.Write(results.Count);
            writer.Write(singular ? " test " : " tests ");
            writer.Write(outcome switch
            {
                ActianTestOutcome.Passed => "passed",
                ActianTestOutcome.Failed => "failed",
                ActianTestOutcome.Skipped => singular ? "was skipped" : "where skipped",
                ActianTestOutcome.Todo => singular ? "has not been processed" : "have not been processed",
                _ => ""
            });
            writer.WriteLine("</summary>");
            if (results.Any())
            {
                writer.WriteLine("<br>");
                writer.WriteLine("<blockquote>");

                foreach (var testResult in results.OrderByDescending(r => r.Outcome).ThenBy(r => r.DisplayName))
                {
                    RenderTestResult(writer, testResult);
                }

                writer.WriteLine("</blockquote>");
            }
            writer.WriteLine("</details>");
            writer.WriteLine();
        }

        private void RenderTestResult(TextWriter writer, ActianTestResult testResult)
        {
            writer.WriteLine();
            writer.WriteLine("<details>");
            writer.Write("<summary>");
            writer.Write(testResult.Outcome switch
            {
                ActianTestOutcome.Passed => "✔️",
                ActianTestOutcome.Failed => "❌",
                ActianTestOutcome.Skipped => "⚠️",
                ActianTestOutcome.Todo => "⚠️",
                _ => ""
            });
            writer.Write(" ");
            writer.Write(testResult.DisplayName);
            writer.WriteLine("</summary>");
            writer.WriteLine("<br>");
            writer.WriteLine("<blockquote>");

            if (testResult.Outcome == ActianTestOutcome.Failed)
            {
                writer.WriteLine("<strong>Error:</strong><br><br>");
                writer.Write($"<pre><code>");
                var first = true;
                foreach (var line in testResult.ErrorMessage.ToLines())
                {
                    if (!first)
                        writer.WriteLine();
                    writer.Write(HtmlEncode(line.TrimEnd()));
                    first = false;
                }
                writer.WriteLine($"</code></pre>");

                writer.WriteLine("<details>");
                writer.WriteLine("<summary><strong>Stack trace:</strong></summary><br>");
                writer.Write($"<pre><code>");
                first = true;
                foreach (var line in testResult.ErrorStackTrace.ToLines())
                {
                    if (!first)
                        writer.WriteLine();
                    writer.Write(HtmlEncode(line.TrimEnd()));
                    first = false;
                }
                writer.WriteLine($"</code></pre>");
                writer.WriteLine("</details>");
            }

            var messageLines = testResult.Messages
                .SelectMany(m => Text.Normalize(m.Text).ToLines())
                .Where(line => !ActianTestMessages.IsTestMessage(line))
                .Trim(line => string.IsNullOrEmpty(line))
                .ToList();

            if (testResult.Outcome == ActianTestOutcome.Skipped && messageLines.Any())
            {
                foreach (var line in messageLines)
                {
                    writer.Write(HtmlEncode(line.TrimEnd()));
                    writer.WriteLine("<br>");
                }
            }
            else if (testResult.Outcome == ActianTestOutcome.Todo)
            {
                if (messageLines.Any())
                {
                    messageLines[0] = Regex.Replace(messageLines[0], @"^todo\s*:?", "", RegexOptions.IgnoreCase).TrimStart();
                    if (string.IsNullOrWhiteSpace(messageLines[0]))
                    {
                        messageLines.RemoveAt(0);
                    }
                }

                if (messageLines.Any())
                {
                    writer.WriteMD(Heading(2, $"This test still needs development:"));
                    writer.WriteMD(Paragraph(Markdown.Add(string.Join("   \n", messageLines))));
                }
                else
                {
                    writer.WriteMD(Heading(2, $"This test still needs development"));
                }
            }
            else if (messageLines.Any())
            {
                writer.WriteLine("<details>");
                writer.WriteLine("<summary><strong>Output</strong></summary><br>");
                writer.Write($"<pre><code>");
                var first = true;
                foreach (var line in messageLines)
                {
                    if (!first)
                        writer.WriteLine();
                    writer.Write(HtmlEncode(line.TrimEnd()));
                    first = false;
                }
                writer.WriteLine($"</code></pre>");
                writer.WriteLine("</details>");
            }

            writer.WriteLine("</blockquote>");
            writer.WriteLine("</details>");
            writer.WriteLine();
            writer.WriteLine();
        }

        private void RenderParameterTable(TextWriter writer, params (object title, object value)[] parameters)
        {
            RenderParameterTable(writer, parameters.AsEnumerable());
        }

        private void RenderParameterTable(TextWriter writer, IEnumerable<(object title, object value)> parameters)
        {
            parameters = parameters.Where(p => p.value is string str ? !string.IsNullOrWhiteSpace(str) : p.value != null);

            if (!parameters.Any())
                return;

            writer.WriteLine("<table>");
            writer.WriteLine("<tbody>");

            foreach (var (title, value) in parameters)
            {
                writer.WriteLine("<tr>");

                writer.Write($"<td>");
                writer.Write(title.ToHtml(removeParagraph: true));
                writer.WriteLine($"</td>");

                writer.Write($"<td>");
                writer.Write(value.ToHtml(removeParagraph: true));
                writer.WriteLine($"</td>");

                writer.WriteLine("</tr>");
            }

            writer.WriteLine("</tbody>");
            writer.WriteLine("</table>");
            writer.WriteLine("");
        }

        private string AnchorText(ActianTestInstallation installation)
            => AnchorText($"{installation.AnchorText}");

        private string AnchorText(ActianTestInstallation installation, ActianTestProject project)
            => AnchorText($"{AnchorText(installation)}-{project.AnchorText}");

        private string AnchorText(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns)
            => AnchorText($"{AnchorText(installation, project)}-{ns.AnchorText}");

        private string AnchorText(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass)
            => AnchorText($"{AnchorText(installation, project, ns)}-{testClass.AnchorText}");

        private string AnchorText(string anchor) => anchor
            .Replace(" ", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(".", "-");

        private string GetAnchor(string anchor, ActianMdReporterDepth depth)
        {
            if (string.IsNullOrWhiteSpace(anchor))
                return "";
            if (Depth > depth)
                return $"#{anchor}";
            return "";
        }
    }
}
