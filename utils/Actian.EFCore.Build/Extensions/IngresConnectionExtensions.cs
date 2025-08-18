// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data;
using System.Threading.Tasks;
using Ingres.Client;

namespace Actian.EFCore.Build
{
    public static class IngresConnectionExtensions
    {
        //public static async Task<int> ExecuteAsync(this IngresConnection connection, string sql, LogConsole console, bool ignoreErrors = false)
        //{
        //    try
        //    {
        //        console.WriteLine("Execute SQL");
        //        console.WriteLine();
        //        console.Indent();

        //        if (connection.State == ConnectionState.Closed)
        //            connection.Open();

        //        using var cmd = connection.CreateCommand();
        //        cmd.CommandText = Text.Normalize(sql);
        //        console.WriteLine(cmd.CommandText);
        //        var rows = await cmd.ExecuteNonQueryAsync();
        //        console.WriteLine(rows == -1 ? "ok" : $"{rows} rows");
        //        return rows;
        //    }
        //    catch (Exception ex) when (ignoreErrors)
        //    {
        //        console.WriteLine(ex.Message);
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        console.WriteLine(ex.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        console.WriteLine();
        //    }
        //}
    }
}
