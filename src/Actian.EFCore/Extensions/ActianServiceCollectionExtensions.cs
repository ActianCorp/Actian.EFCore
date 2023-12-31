﻿using Actian.EFCore.Diagnostics.Internal;
using Actian.EFCore.Infrastructure.Internal;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Conventions;
using Actian.EFCore.Migrations;
using Actian.EFCore.Migrations.Internal;
using Actian.EFCore.Query;
using Actian.EFCore.Query.Internal;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.Update.Internal;
using Actian.EFCore.Utilities;
using Actian.EFCore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Actian specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class ActianServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkActian([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, ActianLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<ActianOptionsExtension>>()
                .TryAdd<IValueGeneratorCache>(p => p.GetService<IActianValueGeneratorCache>())
                .TryAdd<IRelationalTypeMappingSource, ActianTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, ActianSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, ActianMigrationsAnnotationProvider>()
                .TryAdd<IModelValidator, ActianModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, ActianConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IActianUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, ActianModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, ActianValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IActianConnection>())
                .TryAdd<IMigrationsSqlGenerator, ActianMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, ActianDatabaseCreator>()
                .TryAdd<IHistoryRepository, ActianHistoryRepository>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, ActianCompiledQueryCacheKeyGenerator>()
                .TryAdd<IExecutionStrategyFactory, ActianExecutionStrategyFactory>()
                .TryAdd<ISingletonOptions, IActianOptions>(p => p.GetService<IActianOptions>())
                .TryAdd<IValueConverterSelector, ActianValueConverterSelector>()
                .TryAdd<ISqlExpressionFactory, ActianSqlExpressionFactory>()
                .TryAdd<IMethodCallTranslatorProvider, ActianMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, ActianMemberTranslatorProvider>()
                .TryAdd<IQuerySqlGeneratorFactory, ActianQuerySqlGeneratorFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, ActianQueryTranslationPostprocessorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, ActianSqlTranslatingExpressionVisitorFactory>()
                .TryAddProviderSpecificServices(b => b
                    .TryAddSingleton<IActianValueGeneratorCache, ActianValueGeneratorCache>()
                    .TryAddSingleton<IActianOptions, ActianOptions>()
                    .TryAddSingleton<IActianUpdateSqlGenerator, ActianUpdateSqlGenerator>()
                    .TryAddSingleton<IActianSequenceValueGeneratorFactory, ActianSequenceValueGeneratorFactory>()
                    .TryAddScoped<IActianConnection, ActianConnection>()
                );

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
