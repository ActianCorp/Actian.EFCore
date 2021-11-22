﻿namespace Actian.EFCore.Parsing.Internal
{
    public class ActianSqlToken
    {
        public ActianSqlToken(ActianSqlTokenType type, string value, int pos, int line, int column)
            : this(type, type.ToString(), value, pos, line, column)
        {
        }

        public ActianSqlToken(ActianSqlTokenType type, string typeName, string value, int pos, int line, int column)
        {
            Type = type;
            TypeName = typeName;
            Value = value;
            Pos = pos;
            Line = line;
            Column = column;
        }

        public ActianSqlTokenType Type { get; }
        public string TypeName { get; }
        public string Value { get; }
        public int Pos { get; }
        public int Line { get; }
        public int Column { get; }
    }
}
