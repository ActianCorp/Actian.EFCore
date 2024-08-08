﻿using Actian.EFCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Query.Internal
{
    public class ActianQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly IActianSingletonOptions _actianSingletonOptions;

        public ActianQuerySqlGeneratorFactory(
            QuerySqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource,
            IActianSingletonOptions actianSingletonOptions)
        {
            Dependencies = dependencies;
            _typeMappingSource = typeMappingSource;
            _actianSingletonOptions = actianSingletonOptions;
        }

        /// <summary>
        ///     Relational provider-specific dependencies for this service.
        /// </summary>
        protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

        public virtual QuerySqlGenerator Create()
            => new ActianQuerySqlGenerator(Dependencies, _typeMappingSource, _actianSingletonOptions);
    }
}
