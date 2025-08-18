// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianDecimalTypeMapping : DecimalTypeMapping
    {
        public ActianDecimalTypeMapping(
            [NotNull] string storeType,
            DbType? dbType = null,
            int? precision = null,
            int? scale = null,
            StoreTypePostfix storeTypePostfix = StoreTypePostfix.None)
            : base(new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(typeof(decimal)),
                storeType,
                storeTypePostfix,
                dbType
            ).WithPrecisionAndScale(precision, scale))
        {
        }

        protected ActianDecimalTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianDecimalTypeMapping(parameters);

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            if (Size.HasValue && Size.Value != -1)
            {
                parameter.Size = Size.Value;
            }
        }
    }
}
