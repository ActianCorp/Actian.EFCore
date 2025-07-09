using System.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianBooleanTypeMapping : BoolTypeMapping
    {
        public ActianBooleanTypeMapping(
            [NotNull] string storeType,
            DbType? dbType = null)
            : base(storeType, dbType)
        {
        }

        protected ActianBooleanTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        /// <inheritdoc />
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new ActianBooleanTypeMapping(parameters);

        /// <inheritdoc />
        protected override string GenerateNonNullSqlLiteral(object value)
            => $"CAST({base.GenerateNonNullSqlLiteral(value)} AS {StoreType})";
    }
}
