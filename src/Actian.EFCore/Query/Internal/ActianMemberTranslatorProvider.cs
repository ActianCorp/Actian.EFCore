// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

// TODO: ActianMemberTranslatorProvider
namespace Actian.EFCore.Query.Internal
{
    public class ActianMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public ActianMemberTranslatorProvider(
            RelationalMemberTranslatorProviderDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource)
            : base(dependencies)
        {
            var sqlExpressionFactory = dependencies.SqlExpressionFactory;

            AddTranslators(
            [
                new ActianDateOnlyMemberTranslator(sqlExpressionFactory),
                new ActianDateTimeMemberTranslator(sqlExpressionFactory, typeMappingSource),
                new ActianStringMemberTranslator(sqlExpressionFactory),
                //new ActianTimeSpanMemberTranslator(sqlExpressionFactory),
                //new ActianTimeOnlyMemberTranslator(sqlExpressionFactory)
            ]);
        }
    }
}
