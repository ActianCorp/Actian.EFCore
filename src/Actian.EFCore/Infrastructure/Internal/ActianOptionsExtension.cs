// Copyright (c) 2024 Actian Corporation. All Rights Reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Actian.EFCore.Infrastructure.Internal
{
    public class ActianOptionsExtension : RelationalOptionsExtension, IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;
        private int? _compatibilityLevel;
        private bool? _expandCollectionParameters;

        public static readonly int DefaultCompatibilityLevel = 160;
        public static readonly bool DefaultExpandCollectionParameters = true;

        public ActianOptionsExtension()
        {
        }

        // NB: When adding new options, make sure to update the copy ctor below.

        protected ActianOptionsExtension([NotNull] ActianOptionsExtension copyFrom)
            : base(copyFrom)
        {
            _compatibilityLevel = copyFrom._compatibilityLevel;
            _expandCollectionParameters = copyFrom._expandCollectionParameters;
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

        public virtual bool ExpandCollectionParameters
            => _expandCollectionParameters ?? DefaultExpandCollectionParameters;

        public virtual bool? ExpandCollectionParametersWithoutDefault
            => _expandCollectionParameters;

        public virtual ActianOptionsExtension WithCompatibilityLevel(int? compatibilityLevel)
        {
            var clone = (ActianOptionsExtension)Clone();

            clone._compatibilityLevel = compatibilityLevel;

            return clone;
        }

        public virtual ActianOptionsExtension WithExpandCollectionParameters(bool? expandCollectionParameters)
        {
            var clone = (ActianOptionsExtension)Clone();

            clone._expandCollectionParameters = expandCollectionParameters;

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
                                .Append(compatibilityLevel)
                                .Append(' ');
                        }

                        if (Extension._expandCollectionParameters is bool expandCollectionParameters)
                        {
                            builder
                                .Append("ExpandCollectionParameters=")
                                .Append(expandCollectionParameters);
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
                    var hashCode = base.GetServiceProviderHashCode();
                    hashCode = (hashCode * 397) ^ Extension._expandCollectionParameters.GetHashCode();
                    _serviceProviderHash = hashCode;
                }

                return (int)_serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (Extension.CompatibilityLevel is int compatibilityLevel)
                {
                    debugInfo["CompatibilityLevel"] = compatibilityLevel.ToString();
                }

                if (Extension._expandCollectionParameters is bool expandCollectionParameters)
                {
                    debugInfo["Actian:ExpandCollectionParameters"] = expandCollectionParameters.ToString();
                }
            }
        }
    }
}
