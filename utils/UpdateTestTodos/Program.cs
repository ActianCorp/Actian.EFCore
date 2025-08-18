// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using UpdateTestTodos;

var paths = new Paths();
var testResults = TestResults.GetResults(paths);
var changeCount = 0;

foreach (var testFile in paths.GetTestFiles())
{
    UpdateActianTodoRewriter.Rewrite(testFile, testResults, ref changeCount);
}

