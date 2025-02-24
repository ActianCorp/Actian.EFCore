using System.Threading;
using System.Threading.Tasks;
using Actian.EFCore.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Extensions
{
    public static class RelationalCommandExtensions
    {
        public static T ExecuteScalar<T>(this IRelationalCommand command, RelationalCommandParameterObject parameterObject)
        {
            return command.ExecuteScalar(parameterObject).ChangeType<T>();
        }

        public static async Task<T> ExecuteScalarAsync<T>(this IRelationalCommand command, RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken = default)
        {
            var value = await command.ExecuteScalarAsync(parameterObject, cancellationToken).ConfigureAwait(false);
            if (value == null)
                value = 0;
            return value.ChangeType<T>();
        }
    }
}
