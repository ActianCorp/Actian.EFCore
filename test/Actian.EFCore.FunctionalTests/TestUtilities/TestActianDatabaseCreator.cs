using System.Threading;
using System.Threading.Tasks;
using Actian.EFCore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.TestUtilities
{
    internal class TestActianDatabaseCreator : ActianDatabaseCreator
    {
        public TestActianDatabaseCreator(
            [NotNull] RelationalDatabaseCreatorDependencies dependencies,
            [NotNull] IActianConnection connection,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder)
            : base(dependencies, connection, rawSqlCommandBuilder)
        {
        }

        public override void Delete()
        {
            // Actian.Client can not create or delete databases. So clean instead.
            _connection.Context.Database.EnsureClean();
        }

        public override Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            Delete();
            return Task.CompletedTask;
        }
    }
}
