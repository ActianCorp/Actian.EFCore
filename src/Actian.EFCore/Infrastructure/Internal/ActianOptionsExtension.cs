// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

﻿using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Actian.EFCore.Infrastructure.Internal
{
    public class ActianOptionsExtension : RelationalOptionsExtension, IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;
        private int? _compatibilityLevel;

        public static readonly int DefaultCompatibilityLevel = 160;

        public ActianOptionsExtension()
        {
        }

        // NB: When adding new options, make sure to update the copy ctor below.

        protected ActianOptionsExtension([NotNull] ActianOptionsExtension copyFrom)
            : base(copyFrom)
        {
            _compatibilityLevel = copyFrom._compatibilityLevel;
        }

        /// <inheritdoc />
        public override DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        /// <inheritdoc />
        protected override RelationalOptionsExtension Clone()
            => new ActianOptionsExtension(this);

        public virtual int CompatibilityLevel
            => _compatibilityLevel ?? DefaultCompatibilityLevel;

        public virtual int? CompatibilityLevelWithoutDefault
            => _compatibilityLevel;

        public virtual ActianOptionsExtension WithCompatibilityLevel(int? compatibilityLevel)
        {
            var clone = (ActianOptionsExtension)Clone();

            clone._compatibilityLevel = compatibilityLevel;

            return clone;
        }

        /// <inheritdoc />
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkActian();

        private sealed class ExtensionInfo : RelationalExtensionInfo
        {
            private long? _serviceProviderHash;
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new ActianOptionsExtension Extension
                => (ActianOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append(base.LogFragment);

                        if (Extension._compatibilityLevel is int compatibilityLevel)
                        {
                            builder
                                .Append("CompatibilityLevel=")
                                .Append(compatibilityLevel);
                        }

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override int GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    _serviceProviderHash = base.GetServiceProviderHashCode();
                }

                return (int)_serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                //debugInfo[$"Actian:{nameof(ActianDbContextOptionsBuilder.UseRowNumberForPaging)}"]
                //    = (Extension._rowNumberPaging?.GetHashCode() ?? 0L).ToString(CultureInfo.InvariantCulture);
                if (Extension.CompatibilityLevel is int compatibilityLevel)
                {
                    debugInfo["CompatibilityLevel"] = compatibilityLevel.ToString();
                }
            }
        }
    }
}
