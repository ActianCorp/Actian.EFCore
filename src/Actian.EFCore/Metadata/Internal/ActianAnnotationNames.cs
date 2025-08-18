// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿namespace Actian.EFCore.Metadata.Internal
{
    // TODO: Which annotations are needed?
    public static class ActianAnnotationNames
    {
        public const string Prefix = "Actian:";

        public const string ValueGenerationStrategy = Prefix + "ValueGenerationStrategy";
        public const string HiLoSequenceName = Prefix + "HiLoSequenceName";
        public const string HiLoSequenceSchema = Prefix + "HiLoSequenceSchema";
        public const string IdentityOptions = Prefix + "IdentityOptions";
        public const string EditionOptions = Prefix + "EditionOptions";
        public const string MaxDatabaseSize = Prefix + "DatabaseMaxSize";
        public const string ServiceTierSql = Prefix + "ServiceTierSql";
        public const string PerformanceLevelSql = Prefix + "PerformanceLevelSql";
        public const string Locations = Prefix + "Location";
        public const string Journaling = Prefix + "Journaling";
        public const string Duplicates = Prefix + "Duplicates";
        public const string Persistence = Prefix + "Persistence";
        public const string Include = Prefix + "Include";

        public const string DbNameCase = Prefix + "DbNameCase";
        public const string DbDelimitedCase = Prefix + "DbDelimitedCase";
        public const string IdentityIncrement = Prefix + "IdentityIncrement";
        public const string IdentitySeed = Prefix + "IdentitySeed";
        public const string SequenceName = Prefix + "SequenceName";
        public const string SequenceNameSuffix = Prefix + "SequenceNameSuffix";
        public const string SequenceSchema = Prefix + "SequenceSchema";
        public const string Sparse = Prefix + "Sparse";
        public const string IsTemporal = Prefix + "IsTemporal";
        public const string TemporalHistoryTableName = Prefix + "TemporalHistoryTableName";
        public const string TemporalHistoryTableSchema = Prefix + "TemporalHistoryTableSchema";
        public const string TemporalPeriodStartPropertyName = Prefix + "TemporalPeriodStartPropertyName";
        public const string TemporalPeriodStartColumnName = Prefix + "TemporalPeriodStartColumnName";
        public const string TemporalPeriodEndPropertyName = Prefix + "TemporalPeriodEndPropertyName";
        public const string TemporalPeriodEndColumnName = Prefix + "TemporalPeriodEndColumnName";
        public const string MemoryOptimized = Prefix + "MemoryOptimized";
        public const string UseSqlOutputClause = Prefix + "UseSqlOutputClause";
        public const string CreatedOnline = Prefix + "Online";
        public const string SortInTempDb = Prefix + "SortInTempDb";
        public const string DataCompression = Prefix + "DataCompression";
        public const string Clustered = Prefix + "Clustered";
        public const string FillFactor = Prefix + "FillFactor";
        public const string Identity = Prefix + "Identity";
        public const string TemporalOperationType = Prefix + "TemporalOperationType";
        public const string TemporalAsOfPointInTime = Prefix + "TemporalAsOfPointInTime";
        public const string TemporalRangeOperationFrom = Prefix + "TemporalRangeOperationFrom";
        public const string TemporalRangeOperationTo = Prefix + "TemporalRangeOperationTo";
    }
}
