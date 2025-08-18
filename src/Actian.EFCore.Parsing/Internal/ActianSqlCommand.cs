// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Text;

namespace Actian.EFCore.Parsing.Internal
{
    public abstract class ActianSqlCommand : ActianSqlStatement
    {
        protected ActianSqlCommand(StringBuilder sql)
            : base(null, sql, true)
        {
        }

        public class Ansistamp : ActianSqlCommand
        {
            internal Ansistamp(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Append : ActianSqlCommand
        {
            internal Append(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Bell : ActianSqlCommand
        {
            internal Bell(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\bell";
            }
        }

        public class NoBell : ActianSqlCommand
        {
            internal NoBell(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\nobell";
            }
        }

        public class ChDir : ActianSqlCommand
        {
            internal ChDir(StringBuilder sql, string dirName) : base(sql)
            {
                DirName = dirName;
            }

            public string DirName { get; }

            public override string ToString()
            {
                return $"\\chdir {DirName}";
            }
        }

        public class Continue : ActianSqlCommand
        {
            internal Continue(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\continue";
            }
        }

        public class NoContinue : ActianSqlCommand
        {
            internal NoContinue(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\nocontinue";
            }
        }

        public class Date : ActianSqlCommand
        {
            internal Date(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\date";
            }
        }

        public class Editor : ActianSqlCommand
        {
            internal Editor(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class Eval : ActianSqlCommand
        {
            internal Eval(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Go : ActianSqlCommand
        {
            internal Go(StringBuilder sql) : base(sql)
            {
            }

            public override string ToString()
            {
                return $"\\g";
            }
        }

        public class Include : ActianSqlCommand
        {
            internal Include(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }

            public override string ToString()
            {
                return $"\\include {FileName}";
            }
        }

        public class List : ActianSqlCommand
        {
            internal List(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Macro : ActianSqlCommand
        {
            internal Macro(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoMacro : ActianSqlCommand
        {
            internal NoMacro(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Mark : ActianSqlCommand
        {
            internal Mark(StringBuilder sql, string label) : base(sql)
            {
                Label = label;
            }

            public string Label { get; }
        }

        public class Padding : ActianSqlCommand
        {
            internal Padding(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoPadding : ActianSqlCommand
        {
            internal NoPadding(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Print : ActianSqlCommand
        {
            internal Print(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Quit : ActianSqlCommand
        {
            internal Quit(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Read : ActianSqlCommand
        {
            internal Read(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class Redirect : ActianSqlCommand
        {
            internal Redirect(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class NoRedirect : ActianSqlCommand
        {
            internal NoRedirect(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class Reset : ActianSqlCommand
        {
            internal Reset(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Runtime : ActianSqlCommand
        {
            internal Runtime(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoRuntime : ActianSqlCommand
        {
            internal NoRuntime(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Script : ActianSqlCommand
        {
            internal Script(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }

        public class Shell : ActianSqlCommand
        {
            internal Shell(StringBuilder sql, string command) : base(sql)
            {
                Command = command;
            }

            public string Command { get; }
        }

        public class Silent : ActianSqlCommand
        {
            internal Silent(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoSilent : ActianSqlCommand
        {
            internal NoSilent(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Suppress : ActianSqlCommand
        {
            internal Suppress(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoSuppress : ActianSqlCommand
        {
            internal NoSuppress(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Time : ActianSqlCommand
        {
            internal Time(StringBuilder sql) : base(sql)
            {
            }
        }

        public class TimeStamp : ActianSqlCommand
        {
            internal TimeStamp(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Titles : ActianSqlCommand
        {
            internal Titles(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoTitles : ActianSqlCommand
        {
            internal NoTitles(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Trim : ActianSqlCommand
        {
            internal Trim(StringBuilder sql) : base(sql)
            {
            }
        }

        public class NoTrim : ActianSqlCommand
        {
            internal NoTrim(StringBuilder sql) : base(sql)
            {
            }
        }

        public class VDelimiter : ActianSqlCommand
        {
            internal VDelimiter(StringBuilder sql, string chr = null) : base(sql)
            {
                Char = chr;
            }

            public string Char { get; }
        }

        public class Vertical : ActianSqlCommand
        {
            internal Vertical(StringBuilder sql) : base(sql)
            {
            }
        }

        public class Write : ActianSqlCommand
        {
            internal Write(StringBuilder sql, string fileName) : base(sql)
            {
                FileName = fileName;
            }

            public string FileName { get; }
        }
    }
}
