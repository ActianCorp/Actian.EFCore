// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Actian.TestLoggers
{
    public class ActianTestMessage
    {
        public ActianTestMessage() { }
        public ActianTestMessage(TestMessageLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public TestMessageLevel Level { get; set; }
        public string Message { get; set; }
    }
}
