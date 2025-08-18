// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Linq;
using System.Reflection;
using Ingres.Client;

namespace Actian.EFCore.TestUtilities
{
    public static class IngresExceptionFactory
    {
        public static IngresException CreateIngresException(int number, Guid? connectionId = null)
        {
            var errorCtors = typeof(IngresError)
                .GetTypeInfo()
                .DeclaredConstructors;

            var error = (IngresError)errorCtors.First(c => c.GetParameters().Length == 8)
                .Invoke(new object[] { number, (byte)0, (byte)0, "Server", "ErrorMessage", "Procedure", 0, null });
            var errors = (IngresErrorCollection)typeof(IngresErrorCollection)
                .GetTypeInfo()
                .DeclaredConstructors
                .Single()
                .Invoke(null);

            typeof(IngresErrorCollection).GetRuntimeMethods().Single(m => m.Name == "Add").Invoke(errors, new object[] { error });

            var exceptionCtors = typeof(IngresException)
                .GetTypeInfo()
                .DeclaredConstructors;

            return (IngresException)exceptionCtors.First(c => c.GetParameters().Length == 4)
                .Invoke(new object[] { "Bang!", errors, null, connectionId ?? Guid.NewGuid() });
        }
    }
}
