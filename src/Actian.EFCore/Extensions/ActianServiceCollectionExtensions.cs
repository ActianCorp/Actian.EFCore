using System;
using Actian.EFCore.Diagnostics.Internal;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Infrastructure.Internal;
using Actian.EFCore.Internal;
using Actian.EFCore.Metadata.Conventions;
using Actian.EFCore.Migrations;
using Actian.EFCore.Migrations.Internal;
using Actian.EFCore.Query.Internal;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.Update.Internal;
using Actian.EFCore.Utilities;
using Actian.EFCore.ValueGeneration.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Actian.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Actian.EFCore.Metadata.Internal;

#nullable enable

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Actian specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class ActianServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers the given Entity Framework <see cref="DbContext" /> as a service in the <see cref="IServiceCollection" />
        ///     and configures it to connect to an Actian database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is a shortcut for configuring a <see cref="DbContext" /> to use Actian. It does not support all options.
        ///         Use <see cref="O:EntityFrameworkServiceCollectionExtensions.AddDbContext" /> and related methods for full control of
        ///         this process.
        ///     </para>
        ///     <para>
        ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
        ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
        ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
        ///         overridden to configure the Actian provider and connection string.
        ///     </para>
        ///     <para>
        ///         To configure the <see cref="DbContextOptions{TContext}" /> for the context, either override the
        ///         <see cref="DbContext.OnConfiguring" /> method in your derived context, or supply
        ///         an optional action to configure the <see cref="DbContextOptions" /> for the context.
        ///     </para>
        ///     <para>
        ///         See <see href="https://aka.ms/efcore-docs-di">Using DbContext with dependency injection</see> for more information and examples.
        ///     </para>
        ///     <para>
        ///         See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>
        ///     </para>
        /// </remarks>
        /// <typeparam name="TContext">The type of context to be registered.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="connectionString">The connection string of the database to connect to.</param>
        /// <param name="actianOptionsAction">An optional action to allow additional Actian specific configuration.</param>
        /// <param name="optionsAction">An optional action to configure the <see cref="DbContextOptions" /> for the context.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddActian<TContext>(
            this IServiceCollection serviceCollection,
            string? connectionString,
            Action<ActianDbContextOptionsBuilder>? actianOptionsAction = null,
            Action<DbContextOptionsBuilder>? optionsAction = null)
            where TContext : DbContext
            => serviceCollection.AddDbContext<TContext>(
                (_, options) =>
                {
                    optionsAction?.Invoke(options);
                    options.UseActian(connectionString, actianOptionsAction);
                });

        public static IServiceCollection AddEntityFrameworkActian([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, ActianLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<ActianOptionsExtension>>()
                .TryAdd<IValueGeneratorCache>(p => p.GetService<IActianValueGeneratorCache>()!)
                .TryAdd<IRelationalTypeMappingSource, ActianTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, ActianSqlGenerationHelper>()
                .TryAdd<IRelationalAnnotationProvider, ActianAnnotationProvider>()
                .TryAdd<IMigrationsAnnotationProvider, ActianMigrationsAnnotationProvider>()
                .TryAdd<IModelValidator, ActianModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, ActianConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IActianUpdateSqlGenerator>()!)
                .TryAdd<IModificationCommandBatchFactory, ActianModificationCommandBatchFactory>()
                .TryAdd<IModificationCommandFactory, ActianModificationCommandFactory>()
                .TryAdd<IValueGeneratorSelector, ActianValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IActianConnection>()!)
                .TryAdd<IMigrationsSqlGenerator, ActianMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, ActianDatabaseCreator>()
                .TryAdd<IHistoryRepository, ActianHistoryRepository>()
                .TryAdd<IExecutionStrategyFactory, ActianExecutionStrategyFactory>()
                .TryAdd<ICompiledQueryCacheKeyGenerator, ActianCompiledQueryCacheKeyGenerator>()
                .TryAdd<IQueryCompilationContextFactory, ActianQueryCompilationContextFactory>()
                .TryAdd<IMethodCallTranslatorProvider, ActianMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, ActianMemberTranslatorProvider>()
                .TryAdd<IQuerySqlGeneratorFactory, ActianQuerySqlGeneratorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, ActianSqlTranslatingExpressionVisitorFactory>()
                .TryAdd<IQueryTranslationPostprocessorFactory, ActianQueryTranslationPostprocessorFactory>()
                .TryAdd<IRelationalParameterBasedSqlProcessorFactory, ActianParameterBasedSqlProcessorFactory>()
                .TryAdd<ISingletonOptions, IActianSingletonOptions>(p => p.GetRequiredService<IActianSingletonOptions>())
                .TryAddProviderSpecificServices(b => b
                    .TryAddSingleton<IActianSingletonOptions, ActianSingletonOptions>()
                    .TryAddSingleton<IActianValueGeneratorCache, ActianValueGeneratorCache>()
                    .TryAddSingleton<IActianUpdateSqlGenerator, ActianUpdateSqlGenerator>()
                    .TryAddSingleton<IActianSequenceValueGeneratorFactory, ActianSequenceValueGeneratorFactory>()
                    .TryAddScoped<IActianConnection, ActianConnection>()
                );

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
