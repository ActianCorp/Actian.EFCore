// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Actian.EFCore.Build
{
    public class LogConsole : IDisposable
    {
        private static readonly object _lock = new object();
        private readonly List<(bool stderr, string line)> _lines = new List<(bool stderr, string line)>();
        private readonly DateTime _start;
        private readonly bool _buffer;
        private readonly bool _quiet;
        private int _indent = 0;

        public LogConsole(string title, bool buffer = true, bool quiet = false)
        {
            _buffer = buffer;
            _quiet = quiet;
            WriteDoubleLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                WriteLine(title);
            }
            _start = DateTime.Now;
            WriteLine($"Started: {_start:yyyy-MM-dd HH:mm:ss.fff}");
            WriteSingleLine();
            WriteLine();
        }

        public void WriteSingleLine() => WriteLine(new string('-', 80));
        public void WriteDoubleLine() => WriteLine(new string('=', 80));

        public void WriteLine(string str = "", bool stderr = false)
        {
            if (_quiet)
                return;

            lock (_lock)
            {
                foreach (var line in str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Select(l => l.TrimEnd()))
                {
                    var indent = string.IsNullOrWhiteSpace(line) ? "" : "  ".Repeat(_indent);

                    if (_buffer)
                    {
                        _lines.Add((stderr, $"{indent}{line}"));
                    }
                    else if (stderr)
                    {
                        Console.Error.Write(indent);
                        Console.Error.WriteLine(line);
                    }
                    else
                    {
                        Console.Out.Write(indent);
                        Console.Out.WriteLine(line);
                    }
                }
            }
        }

        public void WriteCaption(string caption)
        {
            WriteLine();
            WriteSingleLine();
            WriteLine(caption);
            WriteSingleLine();
            WriteLine();
        }

        private class IndentationMarker : IDisposable
        {
            private readonly LogConsole _logConsole;

            public IndentationMarker(LogConsole logConsole)
            {
                _logConsole = logConsole ?? throw new ArgumentNullException(nameof(logConsole));
            }

            public void Dispose()
            {
                _logConsole.WriteLine();
                _logConsole._indent = Math.Max(0, _logConsole._indent - 1);
            }
        }

        public IDisposable Indent()
        {
            _indent += 1;
            return new IndentationMarker(this);
        }

        private void RenderToConsole()
        {
            if (!_buffer)
                return;

            lock (_lock)
            {
                foreach (var (stderr, line) in _lines)
                {
                    if (stderr)
                    {
                        Console.Error.WriteLine(line);
                    }
                    else
                    {
                        Console.Out.WriteLine(line);
                    }
                }
                _lines.Clear();
            }
        }

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    var end = DateTime.Now;
                    WriteLine();
                    WriteSingleLine();
                    WriteLine($"Finished: {end:yyyy-MM-dd HH:mm:ss.fff} - Elapsed: {end - _start}");
                    WriteDoubleLine();
                    WriteLine();
                    RenderToConsole();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
