// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobExpressions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Actian.TestLoggers
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var debug = false;
            var format = "";
            var output = "";
            var title = "";
            var branch = "";
            var pullRequest = "";
            var detailsUrl = "";
            var depth = ActianMdReporterDepth.Test;
            var files = new HashSet<string>();

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-f":
                    case "--format":
                        i += 1;
                        format = args[i];
                        break;
                    case "-o":
                    case "--output":
                        i += 1;
                        output = args[i];
                        break;
                    case "-t":
                    case "--title":
                        i += 1;
                        title = args[i];
                        break;
                    case "-d":
                    case "--depth":
                        i += 1;
                        depth = args[i].ToActianMdReporterDepth();
                        break;
                    case "-b":
                    case "--branch":
                        i += 1;
                        branch = args[i];
                        break;
                    case "-p":
                    case "--pr":
                    case "--pull-request":
                        i += 1;
                        pullRequest = args[i];
                        break;
                    case "--details-url":
                        i += 1;
                        detailsUrl = args[i];
                        break;
                    case "--debug":
                        debug = true;
                        break;
                    default:
                        foreach (var file in Glob.Files(Environment.CurrentDirectory, args[i]))
                        {
                            files.Add(file);
                        }
                        break;
                }
            }

            if (debug)
            {
                Debug(files);
                return;
            }

            var testProjects = ReadTestProjects(files);
            var reporter = GetReporter(format, output, title, branch, pullRequest, detailsUrl, depth);

            Console.WriteLine($"Creating test report: {output}");
            reporter.CreateReport(testProjects);
            Console.WriteLine($"Test report created");
        }

        static IActianTestReporter GetReporter(string format, string output, string title, string branch, string pullrequest, string detailsUrl, ActianMdReporterDepth depth)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new Exception($"No format specified");

            switch (format)
            {
                case "md":
                    if (string.IsNullOrWhiteSpace(output))
                        throw new Exception($"No output specified");
                    return new ActianMdReporter(output, title, branch, pullrequest, detailsUrl, depth);
                case "md-files":
                    if (string.IsNullOrWhiteSpace(output))
                        throw new Exception($"No output specified");
                    return new ActianMdFilesReporter(output, title, branch, pullrequest);
                case "json":
                    if (string.IsNullOrWhiteSpace(output))
                        throw new Exception($"No output specified");
                    return new ActianJsonReporter(output);
                default:
                    throw new Exception($"No reporter for format {format} found");
            }
        }

        static IEnumerable<ActianTestProject> ReadTestProjects(IEnumerable<string> paths)
        {
            return paths.SelectMany(ReadTestProjects).ToList();
        }

        public static IEnumerable<ActianTestProject> ReadTestProjects(string path)
        {
            Console.WriteLine($"Reading test results from {path}");
            using var reader = File.OpenText(path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            return serializer.Deserialize<IEnumerable<ActianTestProject>>(jsonReader);
        }

        static void Debug(IEnumerable<string> paths)
        {
            var testrun = new ActianTestRun();
            var results = paths.SelectMany(ReadTestResults).ToList();

            foreach (var result in results)
            {
                testrun.AddTestResult(result);
            }
        }

        static IEnumerable<TestResult> ReadTestResults(string path)
        {
            Console.WriteLine($"Reading test results from {path}");
            using var reader = File.OpenText(path);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            return serializer.Deserialize<IEnumerable<TestResult>>(jsonReader);
        }
    }
}
