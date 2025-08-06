// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Actian.EFCore.TestUtilities
{
    public class ActianTestAttribute : Attribute
    {
        public ActianTestAttribute(string sourceFilePath, int sourceLineNumber)
        {
            Id = $"{GetType().Name}@{sourceFilePath}#{sourceLineNumber}";
            _member = new Lazy<MemberInfo>(GetMember);
        }

        public MemberInfo Member => _member.Value;

        public string Id { get; }

        private readonly Lazy<MemberInfo> _member;
        private MemberInfo GetMember()
        {
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (HasThisAttribute(type))
                {
                    return type;
                }

                foreach (var method in type.GetMethods())
                {
                    if (HasThisAttribute(method))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        private bool HasThisAttribute(MemberInfo member)
            => member.GetCustomAttributes<ActianTestAttribute>().Any(attr => attr.Id == Id);
    }
}
