// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using JetBrains.Annotations;

namespace Actian.EFCore.Scaffolding.Internal
{
    public static class DbDataReaderExtension
    {
        public static T GetValueOrDefault<T>([NotNull] this DbDataReader reader, [NotNull] string name, ActianCasing casing)
        {
            return reader.GetValueOrDefault<T>(reader.GetOrdinal(casing.Normalize(name)));
        }

        public static T GetValueOrDefault<T>([NotNull] this DbDataReader reader, int idx)
        {
            return reader.IsDBNull(idx)
                ? default
                : reader.GetFieldValue<T>(idx);
        }

        public static T GetValueOrDefault<T>([NotNull] this DbDataRecord record, [NotNull] string name, ActianCasing casing)
        {
            return record.GetValueOrDefault<T>(record.GetOrdinal(casing.Normalize(name)));
        }

        public static T GetValueOrDefault<T>([NotNull] this DbDataRecord record, int idx)
        {
            return record.IsDBNull(idx)
                ? default
                : (T)record.GetValue(idx);
        }

        public static string GetTrimmedChar([NotNull] this DbDataReader reader, [NotNull] string name, ActianCasing casing)
        {
            return reader.GetTrimmedChar(reader.GetOrdinal(casing.Normalize(name)));
        }

        public static string GetTrimmedChar([NotNull] this DbDataReader reader, int idx)
        {
            return reader.IsDBNull(idx)
                ? default
                : reader.GetFieldValue<string>(idx).TrimEnd(' ');
        }

        public static T ExecuteReader<T>([NotNull] this DbConnection connection, string sql, [NotNull] Func<DbDataReader, T> get)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            return get(reader);
        }

        public static IEnumerable<T> Select<T>([NotNull] this DbConnection connection, string sql, [NotNull] Func<DbDataReader, T> get)
        {
            var wasOpen = connection.State == ConnectionState.Open;
            if (!wasOpen)
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                connection.Open();
            }
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                using var reader = command.ExecuteReader();
                return reader.Select(get).ToList();
            }
            finally
            {
                if (!wasOpen && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public static IEnumerable<T> Select<T>([NotNull] this DbDataReader reader, [NotNull] Func<DbDataReader, T> get)
        {
            while (reader.Read())
            {
                yield return get(reader);
            }
        }
    }
}
