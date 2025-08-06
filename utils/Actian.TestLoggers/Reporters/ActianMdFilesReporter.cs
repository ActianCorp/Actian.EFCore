// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Actian.TestLoggers.MarkdownRendering;
using static Actian.TestLoggers.MarkdownRendering.MarkdownDocument;
using static System.Web.HttpUtility;

namespace Actian.TestLoggers
{
    public class ActianMdFilesReporter : IActianTestReporter
    {
        public ActianMdFilesReporter(string testReportDirectory, string title, string branch, string pullRequest)
        {
            TestReportDirectory = testReportDirectory ?? throw new ArgumentNullException(nameof(testReportDirectory));
            Title = title;
            Branch = branch;
            PullRequest = pullRequest;
        }

        public string TestReportDirectory { get; }
        public string Title { get; }
        public string Branch { get; }
        public string PullRequest { get; }

        public void CreateReport(IEnumerable<ActianTestProject> testProjects)
        {
            if (Directory.Exists(TestReportDirectory))
            {
                Directory.Delete(TestReportDirectory, true);
            }

            Render(testProjects
                .GroupBy(p => (p.ActianServer, p.ActianServerPort))
                .Select(ActianTestInstallation.Create)
                .OrderByDescending(p => p.ActianServerVersion)
                .ThenByDescending(p => p.ActianServerCompatibilty)
                .ThenBy(p => p.ActianServer)
                .ThenBy(p => p.ActianServerPort)
                .ToList()
            );
        }

        private TextWriter CreateWriter(IEnumerable<string> pathSegments)
        {
            var path = Path.Combine(new[] { TestReportDirectory }.Concat(pathSegments).ToArray());
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            return new StreamWriter(path, false, new UTF8Encoding(false)) { NewLine = "\n" };
        }

        private static readonly string[] TestReportPath = new[] { "Index.md" };

        private void Render(IEnumerable<ActianTestInstallation> installations)
        {
            using var writer = CreateWriter(TestReportPath);

            if (!string.IsNullOrWhiteSpace(Title))
                writer.WriteMD(Heading(1, Title));

            RenderParameterTable(writer,
                (Bold("Branch:"), Branch),
                (Bold("Pull Request:"), PullRequest),
                (Bold("Started:"), installations.SelectMany(i => i.Results).StartTime().Format()),
                (Bold("Finished:"), installations.SelectMany(i => i.Results).EndTime().Format()),
                (Bold("Duration:"), installations.SelectMany(i => i.Results).Duration().Format()),
                (Bold("Outcome:"), installations.SelectMany(i => i.Results).Outcome().ToHtml())
            );

            writer.RenderResultTable("Installation", false, installations
                .Select(installation => ResultRow.Create(installation, FileUrl(TestReportPath, installation)))
            );

            foreach (var installation in installations)
            {
                Render(installation);
            }
        }

        private void Render(ActianTestInstallation installation)
        {
            var path = FilePath(installation);

            using var writer = CreateWriter(path);

            writer.WriteMD(Heading(1, $"Installation: {installation}"));

            RenderParameterTable(writer,
                (Bold("Test Run:"), Link(Title, FileUrl(path, TestReportPath))),
                (Bold("Server version:"), installation.ActianServerVersion),
                (Bold("Compatibilty:"), installation.ActianServerCompatibilty),
                (Bold("Started:"), installation.Results.StartTime().Format()),
                (Bold("Finished:"), installation.Results.EndTime().Format()),
                (Bold("Duration:"), installation.Results.Duration().Format()),
                (Bold("Outcome:"), installation.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Project", true, installation.TestProjects
                .OrderBy(p => p.ProjectName)
                .Select(project => ResultRow.Create(project, FileUrl(path, installation, project)))
            );

            foreach (var project in installation.TestProjects.OrderBy(p => p.ProjectName))
            {
                Render(installation, project);
            }
        }

        private void Render(ActianTestInstallation installation, ActianTestProject project)
        {
            var path = FilePath(installation, project);

            using var writer = CreateWriter(path);

            writer.WriteMD(Heading(1, $"Project: {project}"));

            RenderParameterTable(writer,
                (Bold("Test Run:"), Link(Title, FileUrl(path, TestReportPath))),
                (Bold("Installation:"), Link($"{installation}", FileUrl(path, installation))),
                (Bold("Started:"), project.Results.StartTime().Format()),
                (Bold("Finished:"), project.Results.EndTime().Format()),
                (Bold("Duration:"), project.Results.Duration().Format()),
                (Bold("Outcome:"), project.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Namespace", true, project.TestNamespaces.OrderBy(p => p.Namespace)
                .Select(testNamespace => ResultRow.Create(testNamespace, FileUrl(path, installation, project, testNamespace)))
            );

            foreach (var ns in project.TestNamespaces)
            {
                Render(installation, project, ns);
            }
        }

        private void Render(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns)
        {
            var path = FilePath(installation, project, ns);

            using var writer = CreateWriter(path);

            writer.WriteMD(Heading(1, $"Project: {project}"));

            RenderParameterTable(writer,
                (Bold("Test Run:"), Link(Title, FileUrl(path, TestReportPath))),
                (Bold("Installation:"), Link($"{installation}", FileUrl(path, installation))),
                (Bold("Project:"), Link($"{project}", FileUrl(path, installation, project))),
                (Bold("Started:"), project.Results.StartTime().Format()),
                (Bold("Finished:"), project.Results.EndTime().Format()),
                (Bold("Duration:"), project.Results.Duration().Format()),
                (Bold("Outcome:"), project.Results.Outcome().ToHtml())
            );

            writer.RenderResultTable("Class", true, ns.TestClasses.OrderBy(p => p.ClassDisplayName)
                .Select(testClass => ResultRow.Create(testClass, FileUrl(path, installation, project, ns, testClass)))
            );

            foreach (var testClass in ns.TestClasses)
            {
                Render(installation, project, ns, testClass);
            }
        }

        private void Render(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass)
        {
            var path = FilePath(installation, project, ns, testClass);

            using var writer = CreateWriter(path);

            writer.WriteMD(Heading(1, $"Test Class: {testClass.ClassDisplayName}"));

            RenderParameterTable(writer,
                (Bold("Test Run:"), Link(Title, FileUrl(path, TestReportPath))),
                (Bold("Installation:"), Link($"{installation}", FileUrl(path, installation))),
                (Bold("Project:"), Link($"{project}", FileUrl(path, installation, project))),
                (Bold("Namespace:"), Link($"{ns}", FileUrl(path, installation, project, ns))),
                (Bold("Full name:"), testClass.ClassName),
                (Bold("Implements:"), HtmlEncode(testClass.Implements)),
                (Bold("Started:"), testClass.Results.StartTime().Format()),
                (Bold("Finished:"), testClass.Results.EndTime().Format()),
                (Bold("Duration:"), testClass.Results.Duration().Format()),
                (Bold("Outcome:"), testClass.Results.Outcome().ToHtml())
            );

            writer.WriteMD(Table(testClass.Results.OrderBy(p => p.DisplayName))
                .Column("Outcome", testResult => testResult.Outcome.ToHtml())
                .Column("Test", testResult => Link(testResult.DisplayName, FileUrl(path, installation, project, ns, testClass, testResult)))
            );

            foreach (var result in testClass.Results)
            {
                RenderTestResult(installation, project, ns, testClass, result);
            }
        }

        private void RenderTestResult(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass, ActianTestResult testResult)
        {
            var path = FilePath(installation, project, ns, testClass, testResult);

            using var writer = CreateWriter(path);

            writer.WriteMD(Heading(1, $"Test: {testResult.DisplayName}"));

            RenderParameterTable(writer,
                (Bold("Test Run:"), Link(Title, FileUrl(path, TestReportPath))),
                (Bold("Installation:"), Link($"{installation}", FileUrl(path, installation))),
                (Bold("Project:"), Link($"{project}", FileUrl(path, installation, project))),
                (Bold("Namespace:"), Link($"{ns}", FileUrl(path, installation, project, ns))),
                (Bold("Test Class:"), Link(testClass.ClassDisplayName, FileUrl(path, installation, project, ns, testClass))),
                (Bold("Full Name:"), testResult.FullyQualifiedName),
                (Bold("Implements:"), HtmlEncode(testClass.Implements)),
                (Bold("Started:"), testResult.StartTime.Format()),
                (Bold("Finished:"), testResult.EndTime.Format()),
                (Bold("Duration:"), testResult.Duration.Format()),
                (Bold("Outcome:"), testResult.Outcome.ToHtml())
            );

            if (testResult.Outcome == ActianTestOutcome.Failed)
            {
                writer.WriteMD(Heading(2, $"Error:"));
                writer.WriteMD(CodeBlock(string.Join("\n",
                    testResult.ErrorMessage.ToLines(trim: false)
                    .Concat("")
                    .Concat(testResult.ErrorStackTrace.ToLines(trim: false))
                )));
            }

            var messageLines = testResult.Messages
                .SelectMany(m => Text.Normalize(m.Text).ToLines())
                .Select(line => line.TrimEnd())
                .Where(line => !ActianTestMessages.IsTestMessage(line))
                .Trim(line => string.IsNullOrEmpty(line))
                .ToList();

            if (testResult.Outcome == ActianTestOutcome.Skipped && messageLines.Any())
            {
                writer.WriteMD(Heading(2, $"Reason the test was skipped:"));
                writer.WriteMD(Paragraph(Markdown.Add(string.Join("   \n", messageLines))));
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
                writer.WriteMD(Heading(2, $"Output:"));
                writer.WriteMD(CodeBlock(Markdown.Add(string.Join("\n", messageLines))));
            }
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

        public static IEnumerable<string> FilePath(ActianTestInstallation installation)
            => FilePath($"{installation.ActianServer}-{installation.ActianServerPort}", "Index.md");

        public static IEnumerable<string> FilePath(ActianTestInstallation installation, ActianTestProject project)
            => FilePath($"{installation.ActianServer}-{installation.ActianServerPort}", project.ProjectName, "Index.md");

        public static IEnumerable<string> FilePath(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns)
            => FilePath($"{installation.ActianServer}-{installation.ActianServerPort}", project.ProjectName, ns.Namespace, "Index.md");

        public static IEnumerable<string> FilePath(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass)
            => FilePath($"{installation.ActianServer}-{installation.ActianServerPort}", project.ProjectName, ns.Namespace, testClass.ClassDisplayName, "Index.md");

        public static IEnumerable<string> FilePath(ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass, ActianTestResult testResult)
            => FilePath($"{installation.ActianServer}-{installation.ActianServerPort}", project.ProjectName, ns.Namespace, testClass.ClassDisplayName, $"{testClass.GetTestName(testResult)}.md");

        public static IEnumerable<string> FilePath(params string[] segments)
            => segments.Select(segment => segment.ToFileName()).ToList();

        public static string FileUrl(IEnumerable<string> from, ActianTestInstallation installation)
            => FileUrl(from, FilePath(installation));

        public static string FileUrl(IEnumerable<string> from, ActianTestInstallation installation, ActianTestProject project)
            => FileUrl(from, FilePath(installation, project));

        public static string FileUrl(IEnumerable<string> from, ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns)
            => FileUrl(from, FilePath(installation, project, ns));

        public static string FileUrl(IEnumerable<string> from, ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass)
            => FileUrl(from, FilePath(installation, project, ns, testClass));

        public static string FileUrl(IEnumerable<string> from, ActianTestInstallation installation, ActianTestProject project, ActianTestNamespace ns, ActianTestClass testClass, ActianTestResult testResult)
            => FileUrl(from, FilePath(installation, project, ns, testClass, testResult));

        private static string FileUrl(IEnumerable<string> from, IEnumerable<string> to)
        {
            var relativePath = new List<string>();

            var fromSegments = from.SkipLast(1).ToArray();
            var toSegments = to.ToArray();

            var length = Math.Min(fromSegments.Length, toSegments.Length);

            // find common root
            var lastCommonRoot = -1;
            for (var index = 0; index < length; index++)
            {
                if (string.Compare(fromSegments[index], toSegments[index], true) != 0)
                    break;

                lastCommonRoot = index;
            }

            // add relative folders in from path
            for (var index = lastCommonRoot + 1; index < fromSegments.Length; index++)
            {
                if (fromSegments[index].Length > 0)
                    relativePath.Add("..");
            }

            // add to folders to path
            for (var index = lastCommonRoot + 1; index < toSegments.Length; index++)
            {
                relativePath.Add(toSegments[index]);
            }

            return string.Join("/", relativePath);
        }
    }
}
