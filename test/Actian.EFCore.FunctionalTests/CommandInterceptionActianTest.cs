// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Actian.EFCore.Infrastructure;
using Actian.EFCore.Storage.Internal;
using Actian.EFCore.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Actian.EFCore
{
    public abstract class CommandInterceptionActianTest : CommandInterceptionTestBase
    {
        protected CommandInterceptionActianTest(InterceptionActianFixtureBase fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture.ListLoggerFactory, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void LogSql() => Helpers.LogSql();

        public override async Task<string> Intercept_query_passively(bool async, bool inject)
        {
            AssertSql(
                @"SELECT ""s"".""Id"", ""s"".""Type""
FROM ""Singularity"" AS ""s""",
                await base.Intercept_query_passively(async, inject)
            );
            return null;
        }

        public override Task Intercept_scalar_passively(bool async, bool inject)
        {
            return base.Intercept_scalar_passively(async, inject);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_passively(bool async, bool inject)
        {
            return base.Intercept_non_query_passively(async, inject);
        }

        public override Task<string> Intercept_query_to_suppress_execution(bool async, bool inject)
        {
            return base.Intercept_query_to_suppress_execution(async, inject);
        }

        public override Task Intercept_query_to_suppress_command_creation(bool async, bool inject)
        {
            return base.Intercept_query_to_suppress_command_creation(async, inject);
        }

        public override Task Intercept_scalar_to_suppress_execution(bool async, bool inject)
        {
            return base.Intercept_scalar_to_suppress_execution(async, inject);
        }

        public override Task Intercept_non_query_to_suppress_execution(bool async, bool inject)
        {
            return base.Intercept_non_query_to_suppress_execution(async, inject);
        }

        public override async Task<string> Intercept_query_to_mutate_command(bool async, bool inject)
        {
            AssertSql(
                @"SELECT ""s"".""Id"", ""s"".""Type""
FROM ""Brane"" AS ""s""",
                await base.Intercept_query_to_mutate_command(async, inject)
            );
            return null;
        }

        public override Task Intercept_scalar_to_mutate_command(bool async, bool inject)
        {
            return base.Intercept_scalar_to_mutate_command(async, inject);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_to_mutate_command(bool async, bool inject)
        {
            return base.Intercept_non_query_to_mutate_command(async, inject);
        }

        public override async Task<string> Intercept_query_to_replace_execution(bool async, bool inject)
        {
            AssertSql(
                @"SELECT ""s"".""Id"", ""s"".""Type""
FROM ""Singularity"" AS ""s""",
                await base.Intercept_query_to_replace_execution(async, inject)
            );
            return null;
        }

        public override Task Intercept_scalar_to_replace_execution(bool async, bool inject)
        {
            return base.Intercept_scalar_to_replace_execution(async, inject);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_to_replace_execution(bool async, bool inject)
        {
            return base.Intercept_non_query_to_replace_execution(async, inject);
        }

        public override Task<string> Intercept_query_to_replace_result(bool async, bool inject)
        {
            return base.Intercept_query_to_replace_result(async, inject);
        }

        public override Task Intercept_scalar_to_replace_result(bool async, bool inject)
        {
            return base.Intercept_scalar_to_replace_result(async, inject);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_to_replace_result(bool async, bool inject)
        {
            return base.Intercept_non_query_to_replace_result(async, inject);
        }

        public override Task Intercept_query_that_throws(bool async, bool inject)
        {
            return base.Intercept_query_that_throws(async, inject);
        }

        public override Task Intercept_scalar_that_throws(bool async, bool inject)
        {
            return base.Intercept_scalar_that_throws(async, inject);
        }

        public override Task Intercept_non_query_that_throws(bool async, bool inject)
        {
            return base.Intercept_non_query_that_throws(async, inject);
        }

        public override Task Intercept_query_to_throw(bool async, bool inject)
        {
            return base.Intercept_query_to_throw(async, inject);
        }

        public override Task Intercept_scalar_to_throw(bool async, bool inject)
        {
            return base.Intercept_scalar_to_throw(async, inject);
        }

        public override Task Intercept_non_query_to_throw(bool async, bool inject)
        {
            return base.Intercept_non_query_to_throw(async, inject);
        }

        public override Task Intercept_query_with_one_app_and_one_injected_interceptor(bool async)
        {
            return base.Intercept_query_with_one_app_and_one_injected_interceptor(async);
        }

        public override Task Intercept_scalar_with_one_app_and_one_injected_interceptor(bool async)
        {
            return base.Intercept_scalar_with_one_app_and_one_injected_interceptor(async);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_one_app_and_one_injected_interceptor(bool async)
        {
            return base.Intercept_non_query_one_app_and_one_injected_interceptor(async);
        }

        public override Task Intercept_query_with_two_injected_interceptors(bool async)
        {
            return base.Intercept_query_with_two_injected_interceptors(async);
        }

        public override Task Intercept_scalar_with_two_injected_interceptors(bool async)
        {
            return base.Intercept_scalar_with_two_injected_interceptors(async);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_with_two_injected_interceptors(bool async)
        {
            return base.Intercept_non_query_with_two_injected_interceptors(async);
        }

        public override Task Intercept_query_with_explicitly_composed_app_interceptor(bool async)
        {
            return base.Intercept_query_with_explicitly_composed_app_interceptor(async);
        }

        public override Task Intercept_scalar_with_explicitly_composed_app_interceptor(bool async)
        {
            return base.Intercept_scalar_with_explicitly_composed_app_interceptor(async);
        }

        [ActianSkipAnsi]
        public override Task Intercept_non_query_with_explicitly_composed_app_interceptor(bool async)
        {
            return base.Intercept_non_query_with_explicitly_composed_app_interceptor(async);
        }

        private static new void AssertSql(string expected, string actual)
        {
            actual.Should().NotDifferFrom(expected);
        }

        public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "CommandInterception";
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
        }

        public class WithoutDiagnostics : CommandInterceptionActianTest, IClassFixture<WithoutDiagnostics.InterceptionActianFixture>
        {
            public WithoutDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                {
                    new ActianDbContextOptionsBuilder(base.AddOptions(builder))
                        .ExecutionStrategy(d => new ActianExecutionStrategy(d));
                    return builder;
                }
            }
        }

        public class WithDiagnostics : CommandInterceptionActianTest, IClassFixture<WithDiagnostics.InterceptionActianFixture>
        {
            public WithDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => true;

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                {
                    new ActianDbContextOptionsBuilder(base.AddOptions(builder))
                        .ExecutionStrategy(d => new ActianExecutionStrategy(d));
                    return builder;
                }
            }
        }
    }
}
