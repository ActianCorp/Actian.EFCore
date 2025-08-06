// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Design;

[assembly: DesignTimeProviderServices("Actian.EFCore.Design.Internal.ActianDesignTimeServices")]
[assembly: InternalsVisibleTo("Actian.EFCore.FunctionalTests")]
[assembly: InternalsVisibleTo("Actian.EFCore.Tests")]
