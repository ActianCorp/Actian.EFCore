// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System;
using System.Text.RegularExpressions;
using Actian.EFCore.Metadata.Internal;
using Actian.EFCore.Scaffolding.Internal;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Actian.EFCore
{
    public static class SelfReferenceEquivalencyAssertionOptionsExtensions
    {
        public static EquivalencyOptions<TSelf> UsingString<TSelf>(this EquivalencyOptions<TSelf> options, string path, Action<IAssertionContext<string>> action)
            => options.Using(PathRe(path), action);

        public static EquivalencyOptions<TSelf> Using<TSelf, TProperty>(this EquivalencyOptions<TSelf> options, string path, Action<IAssertionContext<TProperty>> action)
            => options.Using(PathRe(path), action);

        public static EquivalencyOptions<TSelf> Using<TSelf, TProperty>(this EquivalencyOptions<TSelf> options, Regex pathRe, Action<IAssertionContext<TProperty>> action)
            => options
                .Using(action)
                .When(info => PathMatchesRe(info, pathRe));

        public static EquivalencyOptions<TSelf> UsingDelimitedName<TSelf>(this EquivalencyOptions<TSelf> options, DatabaseModel dbModel, string path)
            => options.UsingDelimitedName(dbModel, PathRe(path));

        public static EquivalencyOptions<TSelf> UsingDelimitedName<TSelf>(this EquivalencyOptions<TSelf> options, DatabaseModel dbModel, Regex pathRe)
            => options
                .Using<string>(ctx => ctx.Subject.Should().Be(NormalizeDelimitedName(dbModel, ctx.Expectation)))
                .When(info => PathMatchesRe(info, pathRe));

        private static Regex PathRe(string path)
            => new Regex($@"^{path.Replace(".", @"\.").Replace("[]", @"\[\d+\]")}$");

        private static bool PathMatchesRe(IObjectInfo info, Regex pathRe)
            => pathRe.IsMatch(info.Path);

        public static string NormalizeDelimitedName(this DatabaseModel dbModel, string name)
            => dbModel.GetAnnotation<ActianCasing>(ActianAnnotationNames.DbDelimitedCase).Normalize(name);
    }
}
