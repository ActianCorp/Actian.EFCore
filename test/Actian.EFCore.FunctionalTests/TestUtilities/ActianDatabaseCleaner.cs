﻿using System.Diagnostics;
using Actian.EFCore.Design.Internal;
using Actian.EFCore.Diagnostics.Internal;
using Actian.EFCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Actian.EFCore.TestUtilities
{
    public class ActianDatabaseCleaner : RelationalDatabaseCleaner
    {
        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
        {
            var services = new ServiceCollection();
            services.AddEntityFrameworkActian();

            new ActianDesignTimeServices().ConfigureDesignTimeServices(services);

            return services
                .BuildServiceProvider() // No scope validation; cleaner violates scopes, but only resolve services once.
                .GetRequiredService<IDatabaseModelFactory>();
        }
    }
}
