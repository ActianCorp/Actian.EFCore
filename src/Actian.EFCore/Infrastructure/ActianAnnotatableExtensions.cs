// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="IAnnotatable" />.
    /// </summary>
    public static class ActianAnnotatableExtensions
    {
        /// <summary>
        ///     Gets the annotation with the given name, throwing if it does not exist.
        /// </summary>
        /// <param name="annotatable"> The object to find the annotation on. </param>
        /// <param name="annotationName"> The key of the annotation to find. </param>
        /// <returns> The annotation with the specified name. </returns>
        public static T GetAnnotation<T>([NotNull] this IAnnotatable annotatable, [NotNull] string annotationName)
        {
            return (T)annotatable.GetAnnotation(annotationName).Value;
        }
    }
}
