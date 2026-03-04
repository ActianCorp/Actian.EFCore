// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace Actian.EFCore.Query.Internal;

public class ActianParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
{
    private readonly IActianSingletonOptions _actianSingletonOptions;

    public ActianParameterBasedSqlProcessorFactory(
        RelationalParameterBasedSqlProcessorDependencies dependencies,
        IActianSingletonOptions actianSingletonOptions)
    {
        Dependencies = dependencies;
        _actianSingletonOptions = actianSingletonOptions;
    }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual RelationalParameterBasedSqlProcessorDependencies Dependencies { get; }

    public virtual RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new ActianParameterBasedSqlProcessor(Dependencies, parameters, _actianSingletonOptions);
}
