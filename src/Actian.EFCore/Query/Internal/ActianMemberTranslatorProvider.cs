using JetBrains.Annotations;
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
