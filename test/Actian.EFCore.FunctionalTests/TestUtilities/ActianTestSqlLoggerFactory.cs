// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;

namespace Actian.EFCore.TestUtilities
{
    internal class ActianTestSqlLoggerFactory : TestSqlLoggerFactory
    {
        public ActianTestSqlLoggerFactory()
            : this(_ => true)
        {
        }

        public ActianTestSqlLoggerFactory(Func<string, bool> shouldLogCategory)
            : base(c => shouldLogCategory(c) || c == DbLoggerCategory.Database.Command.Name)
        {
            Logger = new ActianTestSqlLogger(shouldLogCategory(DbLoggerCategory.Database.Command.Name));
        }

        protected ActianTestSqlLogger ActianLogger => (ActianTestSqlLogger)Logger;

        public void AddLogger(ILogger logger)
        {
            ActianLogger.ExtraLoggers.Add(logger);
        }

        protected class ActianTestSqlLogger : TestSqlLogger
        {
            public ActianTestSqlLogger(bool shouldLogCommands) : base(shouldLogCommands)
            {
            }

            public readonly HashSet<ILogger> ExtraLoggers = new HashSet<ILogger>();

            protected override void UnsafeLog<TState>(LogLevel logLevel, EventId eventId, string message, TState state, Exception exception)
            {
                base.UnsafeLog(logLevel, eventId, message, state, exception);
                foreach (var logger in ExtraLoggers)
                {
                    logger.Log(logLevel, eventId, exception, message);
                }
            }
        }
    }
}
