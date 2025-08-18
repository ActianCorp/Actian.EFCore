// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Data;
using Ingres.Client;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore
{
    internal static class IngresTypeExtensions
    {
        public static string GetStoreName(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.Boolean => "boolean",

                IngresType.TinyInt => "tinyint",
                IngresType.SmallInt => "smallint",
                IngresType.Int => "int",
                IngresType.BigInt => "bigint",

                IngresType.Real => "float4",
                IngresType.Double => "float",
                IngresType.Decimal => "decimal",

                IngresType.Char => "char",
                IngresType.VarChar => "varchar",
                IngresType.LongVarChar => "long varchar",

                IngresType.NChar => "nchar",
                IngresType.NVarChar => "nvarchar",
                IngresType.LongNVarChar => "long nvarchar",

                IngresType.Binary => "byte",
                IngresType.VarBinary => "varbyte",
                IngresType.LongVarBinary => "long byte",

                IngresType.Date => "ansidate",
                IngresType.Time => "time",
                IngresType.DateTime => "timestamp",
                IngresType.IngresDate => "ingresdate",

                IngresType.IntervalYearToMonth => "intervalyeartomonth",
                IngresType.IntervalDayToSecond => "intervaldaytosecond",
                _ => throw new InvalidOperationException($"Unknown Ingres type {ingresType}"),
            };
        }

        public static StoreTypePostfix GetStoreTypePostfix(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.Boolean => StoreTypePostfix.None,

                IngresType.TinyInt => StoreTypePostfix.None,
                IngresType.SmallInt => StoreTypePostfix.None,
                IngresType.Int => StoreTypePostfix.None,
                IngresType.BigInt => StoreTypePostfix.None,

                IngresType.Real => StoreTypePostfix.None,
                IngresType.Double => StoreTypePostfix.None,
                IngresType.Decimal => StoreTypePostfix.PrecisionAndScale,

                IngresType.Char => StoreTypePostfix.Size,
                IngresType.VarChar => StoreTypePostfix.Size,
                IngresType.LongVarChar => StoreTypePostfix.None,

                IngresType.NChar => StoreTypePostfix.Size,
                IngresType.NVarChar => StoreTypePostfix.Size,
                IngresType.LongNVarChar => StoreTypePostfix.None,

                IngresType.Binary => StoreTypePostfix.Size,
                IngresType.VarBinary => StoreTypePostfix.Size,
                IngresType.LongVarBinary => StoreTypePostfix.None,

                IngresType.Date => StoreTypePostfix.None,
                IngresType.Time => StoreTypePostfix.None,
                IngresType.DateTime => StoreTypePostfix.None,
                IngresType.IngresDate => StoreTypePostfix.None,

                IngresType.IntervalYearToMonth => StoreTypePostfix.None,
                IngresType.IntervalDayToSecond => StoreTypePostfix.None,
                _ => StoreTypePostfix.None,
            };
        }

        public static DbType GetDbType(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.Boolean => DbType.Boolean,

                IngresType.TinyInt => DbType.SByte,
                IngresType.SmallInt => DbType.Int16,
                IngresType.Int => DbType.Int32,
                IngresType.BigInt => DbType.Int64,

                IngresType.Real => DbType.Single,
                IngresType.Double => DbType.Double,
                IngresType.Decimal => DbType.Decimal,

                IngresType.Char => DbType.AnsiStringFixedLength,
                IngresType.VarChar => DbType.AnsiString,
                IngresType.LongVarChar => DbType.AnsiString,

                IngresType.NChar => DbType.StringFixedLength,
                IngresType.NVarChar => DbType.String,
                IngresType.LongNVarChar => DbType.String,

                IngresType.Binary => DbType.Binary,
                IngresType.VarBinary => DbType.Binary,
                IngresType.LongVarBinary => DbType.Binary,

                IngresType.Date => DbType.Date,
                IngresType.Time => DbType.Time,
                IngresType.DateTime => DbType.DateTime,
                IngresType.IngresDate => DbType.DateTime,

                IngresType.IntervalYearToMonth => DbType.Object,
                IngresType.IntervalDayToSecond => DbType.Object,
                _ => DbType.Object,
            };
        }

        public static bool IsUnicode(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.NChar => true,
                IngresType.NVarChar => true,
                IngresType.LongNVarChar => true,
                _ => false,
            };
        }

        public static bool IsFixedLength(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.Char => true,
                IngresType.NChar => true,
                _ => false,
            };
        }

        private const int ByteMax = 32000;
        private const int AnsiMax = ByteMax;
        private const int UnicodeMax = ByteMax / 2;

        public static int? GetMaxLength(this IngresType ingresType)
        {
            return ingresType switch
            {
                IngresType.Char => AnsiMax,
                IngresType.VarChar => AnsiMax,

                IngresType.NChar => UnicodeMax,
                IngresType.NVarChar => UnicodeMax,

                IngresType.Binary => ByteMax,
                IngresType.VarBinary => ByteMax,

                _ => null,
            };
        }
    }
}
