// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Actian.EFCore.TestUtilities
{
    public class ActianTestLogger : ILogger
    {
        public ActianTestLogger(ITestOutputHelper output = null)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; private set; }

        public void SetOutput(ITestOutputHelper output)
        {
            Output = output;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = GetMessage(state, exception, formatter);

            Output?.WriteLine($"[{logLevel} {DateTime.Now:HH:mm:ss.fff}]");
            foreach (var line in message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                Output?.WriteLine(line);
            }
            Output?.WriteLine("");


            //var prefix = $"[{FormatLogLevel(logLevel)} {DateTime.Now:HH:mm:ss.fff}] ";
            //foreach (var line in message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            //{
            //    Output?.WriteLine($"{prefix}{line}");
            //    prefix = new string(' ', prefix.Length);
            //}
            //Output?.WriteLine("");
        }

        private string GetMessage<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            if (exception is null)
                return message;

            using var writer = new StringWriter();

            writer.WriteLine(message);
            var first = true;
            while (exception != null)
            {
                if (!first || exception.Message != message)
                {
                    writer.WriteLine(exception.Message);
                }
                writer.WriteLine(exception.StackTrace);
                exception = exception.InnerException;
                first = false;
            }
            return writer.ToString();
        }

        private string FormatLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            LogLevel.None => "NON",
            _ => ""
        };
    }
}
