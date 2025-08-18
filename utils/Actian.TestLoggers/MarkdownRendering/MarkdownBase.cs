// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.IO;

namespace Actian.TestLoggers.MarkdownRendering
{
    public abstract class MarkdownBase : IMarkdown
    {
        public abstract void Render(TextWriter writer);

        public override string ToString()
        {
            return this.Render();
        }
    }
}
