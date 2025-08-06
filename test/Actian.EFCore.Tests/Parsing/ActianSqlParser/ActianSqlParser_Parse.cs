// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Actian.EFCore.Parsing.Internal;
using Actian.EFCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using static Actian.EFCore.Parsing.Internal.ActianSqlParser;
using static Actian.EFCore.Text;

namespace Actian.EFCore.Tests.Parsing.ActianSqlParser
{
    public class ActianSqlParser_Parse
    {
        public ActianSqlParser_Parse(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Fact]
        public void Empty_script_should_parse_to_zero_statements() => Test();

        [Fact]
        public void Simple_script_should_parse_correctly() => Test();

        [Fact]
        public void Can_parse_create_procedure() => Test();

        [Fact]
        public void Script_with_line_comments_should_parse_correctly() => Test();

        [Fact]
        public void Script_with_block_comments_should_parse_correctly() => Test();

        [Fact]
        public void Script_with_strings_should_parse_correctly() => Test();

        [Fact]
        public void Northwind_ascii_sql_should_parse_correctly() => Test();

        private void Test([CallerMemberName] string testcase = "", [CallerFilePath] string path = "")
        {
            using var result = new StringWriter();

            var dataDir = Path.Combine(Path.GetDirectoryName(path), "TestData", testcase);
            var scriptPath = Path.Combine(dataDir, "script.sql");
            var expectedPath = Path.Combine(dataDir, "expected.txt");
            var actualPath = Path.Combine(dataDir, "actual.txt");
            var resultDiffPath = Path.Combine(dataDir, "result.diff");
            var resultPath = Path.Combine(dataDir, "result.txt");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            if (!File.Exists(scriptPath))
                File.WriteAllText(scriptPath, "<script to be parsed>", Encoding.UTF8);

            if (!File.Exists(expectedPath))
                File.WriteAllText(expectedPath, "", Encoding.UTF8);

            if (File.Exists(actualPath))
                File.Delete(actualPath);

            if (File.Exists(resultDiffPath))
                File.Delete(resultDiffPath);

            if (File.Exists(resultPath))
                File.Delete(resultPath);

            var script = File.ReadAllText(scriptPath, Encoding.UTF8);

            var expectedTxt = File.ReadAllText(expectedPath, Encoding.UTF8);

            var timer = Stopwatch.StartNew();
            var actual = Parse(script).ToList();
            timer.Stop();

            result.WriteLine($"Time to parse: {timer.Elapsed.TotalSeconds:0.0#####} s");

            var actualTxt = Serialize(actual);

            File.WriteAllText(actualPath, actualTxt, Encoding.UTF8);

            var diff = Diff(expectedTxt, actualTxt);

            File.WriteAllText(resultDiffPath, diff.Format(), Encoding.UTF8);

            try
            {
                diff.Should().HaveNoChanges(maxChanges: 10);
                result.WriteLine("Test succeeded");
            }
            catch (Exception ex)
            {
                result.WriteLine("Test failed");
                result.WriteLine();
                result.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                File.WriteAllText(resultPath, result.ToString(), Encoding.UTF8);
            }
        }

        private static string Serialize(IEnumerable<ActianSqlStatement> statements)
        {
            using var writer = new StringWriter();
            foreach (var statement in statements)
            {
                Serialize(writer, statement);
            }
            return writer.ToString();
        }

        private static void Serialize(TextWriter writer, ActianSqlStatement statement)
        {
            WriteCaption(writer, "SQL", top: '=');
            WriteLine(writer, $"{statement.Sql}");
            WriteCaption(writer, "Command text");
            WriteLine(writer, $"{statement.CommandText}");
            WriteLine(writer, Line('='));
            WriteLine(writer);
        }

        public static void WriteCaption(TextWriter writer, string caption, char? top = '-', char? bottom = '-')
        {
            if (writer is null)
                return;

            WriteLine(writer, Line(top));
            WriteLine(writer, Lines(caption, line => $"-- {line,-74} --"));
            WriteLine(writer, Line(bottom));
        }

        private static void WriteLine(TextWriter writer)
        {
            writer.WriteLine();
        }

        private static void WriteLine(TextWriter writer, string str)
        {
            if (str is null)
                return;

            IEnumerable<string> lines = str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (lines.Last() == "")
            {
                lines = lines.Take(lines.Count() - 1);
            }

            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        private static string Line(char? c)
        {
            return c.HasValue ? new string(c.Value, 80) : null;
        }

        private static string Lines(string str, Func<string, string> modifyLine) => string.Join("\n",
            str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
               .Select(modifyLine)
        );
    }
}
