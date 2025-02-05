// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Migrations.Internal;

public class ActianMigrationDatabaseLock(
    IRelationalCommand releaseLockCommand,
    RelationalCommandParameterObject relationalCommandParameters,
    IHistoryRepository historyRepository,
    CancellationToken cancellationToken = default)
    : IMigrationsDatabaseLock
{
    public virtual IHistoryRepository HistoryRepository => historyRepository;

    public void Dispose()
        => releaseLockCommand.ExecuteScalar(relationalCommandParameters);

    public async ValueTask DisposeAsync()
        => await releaseLockCommand.ExecuteScalarAsync(relationalCommandParameters, cancellationToken).ConfigureAwait(false);
}
