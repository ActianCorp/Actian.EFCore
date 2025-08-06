// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using TestEnvironment = Actian.EFCore.TestUtilities.TestEnvironment;

namespace Actian.EFCore
{
    public abstract class TransactionInterceptionActianTest : TransactionInterceptionTestBase, IDisposable
    {
        protected TransactionInterceptionActianTest(InterceptionActianFixtureBase fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            TestEnvironment.Log(this, testOutputHelper);
            Helpers = new ActianSqlFixtureHelpers(fixture.ListLoggerFactory, testOutputHelper);
        }

        public ActianSqlFixtureHelpers Helpers { get; }
        public void AssertSql(params string[] expected) => Helpers.AssertSql(expected);
        public void Dispose() => Helpers.LogSql();

        public override Task BeginTransaction_without_interceptor(bool async)
        {
            return base.BeginTransaction_without_interceptor(async);
        }

        public override Task UseTransaction_without_interceptor(bool async)
        {
            return base.UseTransaction_without_interceptor(async);
        }

        public override Task Intercept_BeginTransaction(bool async)
        {
            return base.Intercept_BeginTransaction(async);
        }

        public override Task Intercept_BeginTransaction_with_isolation_level(bool async)
        {
            return base.Intercept_BeginTransaction_with_isolation_level(async);
        }

        public override Task Intercept_BeginTransaction_to_suppress(bool async)
        {
            return base.Intercept_BeginTransaction_to_suppress(async);
        }

        public override Task Intercept_BeginTransaction_to_wrap(bool async)
        {
            return base.Intercept_BeginTransaction_to_wrap(async);
        }

        public override Task Intercept_UseTransaction(bool async)
        {
            return base.Intercept_UseTransaction(async);
        }

        public override Task Intercept_UseTransaction_to_wrap(bool async)
        {
            return base.Intercept_UseTransaction_to_wrap(async);
        }

        public override Task Intercept_Commit(bool async)
        {
            return base.Intercept_Commit(async);
        }

        public override Task Intercept_Commit_to_suppress(bool async)
        {
            return base.Intercept_Commit_to_suppress(async);
        }

        public override Task Intercept_Rollback(bool async)
        {
            return base.Intercept_Rollback(async);
        }

        public override Task Intercept_Rollback_to_suppress(bool async)
        {
            return base.Intercept_Rollback_to_suppress(async);
        }

        public override Task Intercept_error_on_commit_or_rollback(bool async, bool commit)
        {
            return base.Intercept_error_on_commit_or_rollback(async, commit);
        }

        [ActianTodo]
        public override Task Intercept_ReleaseSavepoint(bool async)
        {
            return base.Intercept_ReleaseSavepoint(async);
        }

        public override Task Intercept_connection_with_multiple_interceptors(bool async)
        {
            return base.Intercept_connection_with_multiple_interceptors(async);
        }

        public abstract class InterceptionActianFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "TransactionInterception";
            protected override ITestStoreFactory TestStoreFactory => ActianTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkActian(), injectedInterceptors);
        }

        public class WithoutDiagnostics : TransactionInterceptionActianTest, IClassFixture<WithoutDiagnostics.InterceptionActianFixture>
        {
            public WithoutDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        public class WithDiagnostics : TransactionInterceptionActianTest, IClassFixture<WithDiagnostics.InterceptionActianFixture>
        {
            public WithDiagnostics(InterceptionActianFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            public class InterceptionActianFixture : InterceptionActianFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => true;
            }
        }
    }
}
