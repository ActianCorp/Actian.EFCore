// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Actian.EFCore.Parsing.Internal
{
    public class ActianSqlParser : IEnumerator<ActianSqlStatement>
    {
        public static ActianSqlParser Parse(string script)
        {
            return new ActianSqlParser(ActianSqlTokenizer.Tokenize(script));
        }

        public static ActianSqlParser Parse(IEnumerable<string> statements)
        {
            return new ActianSqlParser(ActianSqlTokenizer.Tokenize(string.Join("\n;\n", statements)));
        }

        public static ActianSqlParser Parse(TextReader reader, bool disposeReader = false)
        {
            return new ActianSqlParser(ActianSqlTokenizer.Tokenize(reader, disposeReader));
        }

        private readonly IEnumerator<ActianSqlToken> _tokenizer;

        private ActianSqlParser(IEnumerator<ActianSqlToken> tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _tokenizer.MoveNext();
        }

        public ActianSqlStatement Current { get; private set; }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (EndOfText)
            {
                Current = null;
                return false;
            }

            var statement = ReadNextStatement();

            if (statement is null)
                return MoveNext();

            Current = statement;
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            _tokenizer.Dispose();
        }

        private StringBuilder commandText;
        private bool commandTextEndsWithWhiteSpace = false;
        private StringBuilder sql;
        private bool inStatement = false;
        private bool hasNonWhiteSpace = false;
        private bool hasStatement = false;

        private ActianSqlStatement ReadNextStatement()
        {
            commandText = new StringBuilder();
            commandTextEndsWithWhiteSpace = false;
            sql = new StringBuilder();
            inStatement = true;
            hasNonWhiteSpace = false;
            hasStatement = false;

            Until(() => HasToken(ActianSqlTokenType.Semicolon, ActianSqlTokenType.Command), ReadStatementToken);

            if (!hasStatement && HasToken(ActianSqlTokenType.Command))
            {
                return ReadCommand();
            }
            else if (HasToken(ActianSqlTokenType.Semicolon))
            {
                inStatement = false;
                Advance();
                while (HasToken(ActianSqlTokenType.WhiteSpace, ActianSqlTokenType.LineComment, ActianSqlTokenType.BlockComment))
                {
                    Advance();
                }

                if (HasToken(ActianSqlTokenType.NewLine))
                {
                    Advance();
                }
            }

            return !hasNonWhiteSpace ? null : new ActianSqlStatement(commandText, sql, hasStatement);
        }

        private void ReadStatementToken()
        {
            if (HasToken(ActianSqlTokenType.Command))
            {
                ReadCommand();
            }
            if (HasKeyWord("create"))
            {
                ReadCreateStatement();
            }
            else if (HasKeyWord("begin"))
            {
                ReadStatementBlock();
            }
            else
            {
                Advance();
            }
        }

        private ActianSqlCommand ReadCommand()
        {
            var commandTxt = Token.Value;
            Advance();
            switch (commandTxt.Substring(1).ToLowerInvariant())
            {
                case "ansistamp":
                    return new ActianSqlCommand.Ansistamp(sql);
                case "a":
                case "append":
                    return new ActianSqlCommand.Append(sql);
                case "bell":
                    return new ActianSqlCommand.Bell(sql);
                case "nobell":
                    return new ActianSqlCommand.NoBell(sql);
                case "branch":
                    throw new NotSupportedException("The branch command is not supported");
                case "cd":
                case "chdir":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var chdirDir = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.ChDir(sql, chdirDir);
                case "colformat":
                    throw new NotSupportedException("The colformat command is not supported");
                case "co":
                case "continue":
                    return new ActianSqlCommand.Continue(sql);
                case "noco":
                case "nocontinue":
                    return new ActianSqlCommand.NoContinue(sql);
                case "date":
                    return new ActianSqlCommand.Date(sql);
                case "e":
                case "ed":
                case "edit":
                case "editor":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var editorFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Editor(sql, editorFilename);
                case "v":
                case "eval":
                    return new ActianSqlCommand.Eval(sql);
                case "g":
                case "go":
                    return new ActianSqlCommand.Go(sql);
                case "i":
                case "include":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var includeFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Include(sql, includeFilename);
                case "l":
                case "list":
                    return new ActianSqlCommand.List(sql);
                case "macro":
                    return new ActianSqlCommand.Macro(sql);
                case "nomacro":
                    return new ActianSqlCommand.NoMacro(sql);
                case "mark":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var markLabel = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Mark(sql, markLabel);
                case "padding":
                    return new ActianSqlCommand.Padding(sql);
                case "nopadding":
                    return new ActianSqlCommand.NoPadding(sql);
                case "p":
                case "print":
                    return new ActianSqlCommand.Print(sql);
                case "q":
                case "quit":
                    return new ActianSqlCommand.Quit(sql);
                case "read":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var readFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Read(sql, readFilename);
                case "redir":
                case "redirect":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var redirectFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Redirect(sql, redirectFilename);
                case "noredir":
                case "noredirect":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var noredirectFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.NoRedirect(sql, noredirectFilename);
                case "r":
                case "reset":
                    return new ActianSqlCommand.Reset(sql);
                case "rt":
                case "runtime":
                    return new ActianSqlCommand.Runtime(sql);
                case "nort":
                case "noruntime":
                    return new ActianSqlCommand.NoRuntime(sql);
                case "script":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var scriptFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Script(sql, scriptFilename);
                case "s":
                case "sh":
                case "shell":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var shellCommand = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Shell(sql, shellCommand);
                case "sil":
                case "silent":
                    return new ActianSqlCommand.Silent(sql);
                case "nosil":
                case "nosilent":
                    return new ActianSqlCommand.NoSilent(sql);
                case "suppress":
                    return new ActianSqlCommand.Suppress(sql);
                case "nosuppress":
                    return new ActianSqlCommand.NoSuppress(sql);
                case "time":
                    return new ActianSqlCommand.Time(sql);
                case "ts":
                case "timestamp":
                    return new ActianSqlCommand.TimeStamp(sql);
                case "titles":
                    return new ActianSqlCommand.Titles(sql);
                case "notitles":
                    return new ActianSqlCommand.NoTitles(sql);
                case "trim":
                    return new ActianSqlCommand.Trim(sql);
                case "notrim":
                    return new ActianSqlCommand.NoTrim(sql);
                case "vdelim":
                case "vdelimiter":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    if (HasKeyWord("space") || HasKeyWord("tab") || HasKeyWord("none"))
                    {
                        var vdelimiterChar = Token.Value.ToUpperInvariant();
                        Advance();
                        return new ActianSqlCommand.VDelimiter(sql, vdelimiterChar);
                    }
                    return new ActianSqlCommand.VDelimiter(sql);
                case "vert":
                case "vertical":
                    return new ActianSqlCommand.Vertical(sql);
                case "write":
                    While(() => HasWhiteSpaceOrComment, Advance);
                    var writeFilename = TextUntil(() => HasWhiteSpaceOrComment || HasToken(ActianSqlTokenType.Command));
                    return new ActianSqlCommand.Write(sql, writeFilename);
                default:
                    throw new Exception($"Unknown command: {commandText}");
            }
        }

        private void ReadCreateStatement()
        {
            Advance();
            While(() => HasWhiteSpaceOrComment, Advance);

            if (HasKeyWord("procedure"))
            {
                ReadCreateProcedureStatement();
            }
        }

        private void ReadCreateProcedureStatement()
        {
            Until(() => HasKeyWord("begin"), Advance, inclusive: true);
            ReadStatementBlock();
        }

        private void ReadStatementBlock()
        {
            Until(() => HasKeyWord("end"), ReadStatementToken, inclusive: false);
            if (HasKeyWord("end"))
            {
                Advance();
            }
        }

        private ActianSqlToken Token => _tokenizer.Current;
        private bool EndOfText => Token is null;
        private bool HasToken(ActianSqlTokenType type) => !EndOfText && Token.Type == type;
        private bool HasToken(params ActianSqlTokenType[] types) => !EndOfText && types.Contains(Token.Type);
        private bool HasKeyWord(string keyWord) => HasToken(ActianSqlTokenType.Word) && string.Equals(Token.Value, keyWord, StringComparison.InvariantCultureIgnoreCase);
        private bool HasWhiteSpaceOrComment => HasToken(ActianSqlTokenType.WhiteSpace, ActianSqlTokenType.NewLine, ActianSqlTokenType.LineComment, ActianSqlTokenType.BlockComment);

        private void Until(Func<bool> until, Action action, bool inclusive = false)
        {
            while (!EndOfText && !until())
            {
                action();
            }
            if (!EndOfText && until() && inclusive)
            {
                action();
            }
        }

        private void While(Func<bool> @while, Action action)
        {
            while (!EndOfText && @while())
            {
                action();
            }
        }

        private string TextUntil(Func<bool> until, bool inclusive = false)
        {
            var text = new StringBuilder();
            Until(until, () =>
            {
                text.Append(Token.Value);
                Advance();
            }, inclusive);
            return text.ToString();
        }

        private string TextWhile(Func<bool> @while)
        {
            var text = new StringBuilder();
            While(@while, () =>
            {
                text.Append(Token.Value);
                Advance();
            });
            return text.ToString();
        }

        private void Advance()
        {
            if (EndOfText)
                return;

            sql.Append(Token.Value);

            if (!HasToken(ActianSqlTokenType.WhiteSpace, ActianSqlTokenType.NewLine))
                hasNonWhiteSpace = true;

            if (inStatement)
            {
                if (!HasToken(ActianSqlTokenType.WhiteSpace, ActianSqlTokenType.NewLine, ActianSqlTokenType.LineComment, ActianSqlTokenType.BlockComment))
                    hasStatement = true;

                if (HasToken(ActianSqlTokenType.LineComment, ActianSqlTokenType.BlockComment))
                {
                    if (!commandTextEndsWithWhiteSpace)
                    {
                        AppendToCommandText(" ", true);
                    }
                }
                else if (HasToken(ActianSqlTokenType.WhiteSpace, ActianSqlTokenType.NewLine))
                {
                    AppendToCommandText(Token.Value, true);
                }
                else
                {
                    AppendToCommandText(Token.Value, false);
                }
            }

            _tokenizer.MoveNext();
        }

        private void AppendToCommandText(string str, bool endsWithWhiteSpace)
        {
            commandText.Append(str);
            if (str.Length > 0)
            {
                commandTextEndsWithWhiteSpace = endsWithWhiteSpace;
            }
        }
    }
}
