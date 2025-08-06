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
using System.Text.RegularExpressions;
using Actian.EFCore.Parsing.Internal;
using Actian.EFCore.TestUtilities;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using static Actian.EFCore.Parsing.Internal.ActianSqlTokenizer;
using static Actian.EFCore.Text;

namespace Actian.EFCore.Tests.Parsing.ActianSqlTokenizer
{
    public class ActianSqlTokenizer_Tokenize
    {
        public ActianSqlTokenizer_Tokenize(ITestOutputHelper testOutputHelper)
        {
            TestEnvironment.Log(this, testOutputHelper);
        }

        [Fact]
        public void Empty_script_should_tokenize_to_zero_tokens() => Test();

        [Fact]
        public void A_string_should_tokenize_to_a_string_token() => Test();

        [Fact]
        public void A_string_with_embedded_quotes_should_tokenize_to_a_string_token() => Test();

        [Fact]
        public void A_block_comment_should_tokenize_to_a_block_comment_token() => Test();

        [Fact]
        public void Simple_script_should_tokenize_correctly() => Test();

        [Fact]
        public void Script_with_line_comments_should_tokenize_correctly() => Test();

        [Fact]
        public void Script_with_block_comments_should_tokenize_correctly() => Test();

        [Fact]
        public void Script_with_strings_should_tokenize_correctly() => Test();

        [Fact]
        public void Northwind_ascii_sql_should_tokenize_correctly() => Test();

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
                File.WriteAllText(scriptPath, "<script to be tokenized>", Encoding.UTF8);

            if (!File.Exists(expectedPath))
                File.WriteAllText(expectedPath, "", Encoding.UTF8);

            if (File.Exists(actualPath))
                File.Delete(actualPath);

            if (File.Exists(resultDiffPath))
                File.Delete(resultDiffPath);

            if (File.Exists(resultPath))
                File.Delete(resultPath);

            var script = string.Join("\n", File.ReadLines(scriptPath, Encoding.UTF8));

            var expected = File.ReadLines(expectedPath, Encoding.UTF8)
                .Select(Deserialize)
                .Where(t => t != null)
                .ToList();

            var timer = Stopwatch.StartNew();
            var actual = Tokenize(script).ToList();
            timer.Stop();

            result.WriteLine($"Time to tokenize: {timer.Elapsed.TotalSeconds:0.0#####} s");

            File.WriteAllText(actualPath, Serialize(actual, true), Encoding.UTF8);

            var diff = Diff(Serialize(expected, false), Serialize(actual, false));
            File.WriteAllText(resultDiffPath, diff.Format(), Encoding.UTF8);

            try
            {
                diff.Should().HaveNoChanges();
                File.WriteAllText(expectedPath, Serialize(expected, false), Encoding.UTF8);
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

        private static string Serialize(IEnumerable<ActianSqlToken> tokens, bool withPosition)
        {
            return string.Join("\n", tokens.Select(token => Serialize(token, withPosition)));
        }

        private static string Serialize(ActianSqlToken token, bool withPosition)
        {
            return withPosition
                ? $"[{token.Pos,7} {token.Line,5} {token.Column,3}] {token.TypeName,-13}: {JsonConvert.SerializeObject(token.Value)}"
                : $"{token.TypeName,-13}: {JsonConvert.SerializeObject(token.Value)}";
        }

        private static readonly Regex TokenWithPositionRe = new Regex(@"^\s*(?:(\d+)\s+(\d+)\s*:\s*(\d+)\s+)?(\w+)\s*:\s*("".*"")\s*$");

        private static ActianSqlToken Deserialize(string str)
        {
            var match = TokenWithPositionRe.Match(str);

            if (!match.Success)
                return null;

            if (!int.TryParse(match.Groups[1].Value, out var pos))
                pos = -1;

            if (!int.TryParse(match.Groups[2].Value, out var line))
                line = -1;

            if (!int.TryParse(match.Groups[3].Value, out var column))
                column = -1;

            if (!Enum.TryParse<ActianSqlTokenType>(match.Groups[4].Value, out var type))
                type = ActianSqlTokenType.Unknown;

            var typeName = type == ActianSqlTokenType.Unknown
                ? match.Groups[4].Value
                : type.ToString();

            var value = JsonConvert.DeserializeObject<string>(match.Groups[5].Value);

            return new ActianSqlToken(type, typeName, value, pos, line, column);
        }
    }
}
