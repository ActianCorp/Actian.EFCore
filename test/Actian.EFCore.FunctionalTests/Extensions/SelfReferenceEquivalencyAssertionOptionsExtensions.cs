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
        public static TSelf UsingString<TSelf>(this TSelf options, string path, Action<IAssertionContext<string>> action)
            where TSelf : SelfReferenceEquivalencyAssertionOptions<TSelf>
            => options.Using(PathRe(path), action);

        public static TSelf Using<TSelf, TProperty>(this TSelf options, string path, Action<IAssertionContext<TProperty>> action)
            where TSelf : SelfReferenceEquivalencyAssertionOptions<TSelf>
            => options.Using(PathRe(path), action);

        public static TSelf Using<TSelf, TProperty>(this TSelf options, Regex pathRe, Action<IAssertionContext<TProperty>> action)
            where TSelf : SelfReferenceEquivalencyAssertionOptions<TSelf>
            => options
                .Using(action)
                .When(info => PathMatchesRe(info, pathRe));

        public static TSelf UsingDelimitedName<TSelf>(this TSelf options, DatabaseModel dbModel, string path)
            where TSelf : SelfReferenceEquivalencyAssertionOptions<TSelf>
            => options.UsingDelimitedName(dbModel, PathRe(path));

        public static TSelf UsingDelimitedName<TSelf>(this TSelf options, DatabaseModel dbModel, Regex pathRe)
            where TSelf : SelfReferenceEquivalencyAssertionOptions<TSelf>
            => options
                .Using<string>(ctx => ctx.Subject.Should().Be(NormalizeDelimitedName(dbModel, ctx.Expectation)))
                .When(info => PathMatchesRe(info, pathRe));

        private static Regex PathRe(string path)
            => new Regex($@"^{path.Replace(".", @"\.").Replace("[]", @"\[\d+\]")}$");

        private static bool PathMatchesRe(IMemberInfo info, Regex pathRe)
            => pathRe.IsMatch(info.SelectedMemberPath);

        public static string NormalizeDelimitedName(this DatabaseModel dbModel, string name)
            => dbModel.GetAnnotation<ActianCasing>(ActianAnnotationNames.DbDelimitedCase).Normalize(name);
    }
}
