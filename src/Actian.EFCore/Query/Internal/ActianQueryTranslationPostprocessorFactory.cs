using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Query.Internal
{
    public class ActianQueryTranslationPostprocessorFactory : IQueryTranslationPostprocessorFactory
    {
        public ActianQueryTranslationPostprocessorFactory(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies)
        {
            Dependencies = dependencies;
            RelationalDependencies = relationalDependencies;
        }

        /// <summary>
        ///     Dependencies for this service.
        /// </summary>
        protected virtual QueryTranslationPostprocessorDependencies Dependencies { get; }

        /// <summary>
        ///     Relational provider-specific dependencies for this service.
        /// </summary>
        protected virtual RelationalQueryTranslationPostprocessorDependencies RelationalDependencies { get; }

        public virtual QueryTranslationPostprocessor Create(QueryCompilationContext queryCompilationContext)
            => new ActianQueryTranslationPostprocessor(Dependencies, RelationalDependencies, (ActianQueryCompilationContext)queryCompilationContext);
    }
}
