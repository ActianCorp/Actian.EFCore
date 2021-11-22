﻿using System;
using System.Collections.Generic;
using System.Data;
using Actian.EFCore.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Actian.EFCore.Storage.Internal
{
    public class ActianTypeMappingSource : RelationalTypeMappingSource
    {
        // Boolean
        private readonly ActianBooleanTypeMapping _boolean = new ActianBooleanTypeMapping("boolean", DbType.Boolean);

        // Uuid
        // TODO: Implement uuid
        //private readonly GuidTypeMapping _uuid = new GuidTypeMapping("uuid", DbType.Guid);

        // Numeric types
        private readonly ByteTypeMapping _byte = new ByteTypeMapping("smallint", DbType.Byte);
        private readonly ByteTypeMapping _tinyint = new ByteTypeMapping("tinyint", DbType.SByte);
        private readonly ShortTypeMapping _smallint = new ShortTypeMapping("smallint", DbType.Int16);
        private readonly IntTypeMapping _integer = new IntTypeMapping("integer", DbType.Int32);
        private readonly LongTypeMapping _bigint = new LongTypeMapping("bigint", DbType.Int64);

        private readonly FloatTypeMapping _float4 = new FloatTypeMapping("float4", DbType.Single);
        private readonly DoubleTypeMapping _float8 = new DoubleTypeMapping("float", DbType.Double);
        private readonly DecimalTypeMapping _decimal = new ActianDecimalTypeMapping("decimal(18, 2)", DbType.Decimal, precision: 18, scale: 2, storeTypePostfix: StoreTypePostfix.PrecisionAndScale);
        private readonly DecimalTypeMapping _money = new ActianDecimalTypeMapping("money", DbType.Decimal, precision: 14, scale: 2, storeTypePostfix: StoreTypePostfix.PrecisionAndScale);

        // String types
        private readonly ActianStringTypeMapping _char = new ActianStringTypeMapping("char", unicode: false, fixedLength: true, unbounded: false);
        private readonly ActianStringTypeMapping _varchar = new ActianStringTypeMapping("varchar", unicode: false, fixedLength: false, unbounded: false);
        private readonly ActianStringTypeMapping _longVarchar = new ActianStringTypeMapping("long varchar", unicode: false, fixedLength: false, unbounded: true);
        private readonly ActianStringTypeMapping _nchar = new ActianStringTypeMapping("nchar", unicode: true, fixedLength: true, unbounded: false);
        private readonly ActianStringTypeMapping _nvarchar = new ActianStringTypeMapping("nvarchar", unicode: true, fixedLength: false, unbounded: false);
        private readonly ActianStringTypeMapping _longNVarchar = new ActianStringTypeMapping("long nvarchar", unicode: true, fixedLength: false, unbounded: true);

        // Binary types
        private readonly ActianByteArrayTypeMapping _binary = new ActianByteArrayTypeMapping("byte", fixedLength: true, unbounded: false);
        private readonly ActianByteArrayTypeMapping _varbinary = new ActianByteArrayTypeMapping("varbyte", fixedLength: false, unbounded: false);
        private readonly ActianByteArrayTypeMapping _longbinary = new ActianByteArrayTypeMapping("long byte", fixedLength: false, unbounded: true);

        // Date/Time types
        private readonly ActianIngresDateTypeMapping _ingresdate = new ActianIngresDateTypeMapping(typeof(DateTime));
        private readonly ActianAnsiDateTypeMapping _ansidate = new ActianAnsiDateTypeMapping(typeof(DateTime));
        private readonly ActianTimeTypeMapping _time = new ActianTimeTypeMapping("time", typeof(TimeSpan), precision: 6);
        private readonly ActianTimeTypeMapping _timeWithoutTimeZone = new ActianTimeTypeMapping("time without time zone", typeof(TimeSpan));
        private readonly ActianTimeTypeMapping _timeWithLocalTimeZone = new ActianTimeTypeMapping("time with local time zone", typeof(TimeSpan));
        private readonly ActianTimeTypeMapping _timeWithTimeZone = new ActianTimeTypeMapping("time with time zone", typeof(DateTimeOffset), withTimeZone: true);
        private readonly ActianTimestampTypeMapping _timestamp = new ActianTimestampTypeMapping("timestamp", typeof(DateTime));
        private readonly ActianTimestampTypeMapping _timestampWithoutTimeZone = new ActianTimestampTypeMapping("timestamp without time zone", typeof(DateTime));
        private readonly ActianTimestampTypeMapping _timestampWithLocalTimeZone = new ActianTimestampTypeMapping("timestamp with local time zone", typeof(DateTime));
        private readonly ActianTimestampTypeMapping _timestampWithTimeZone = new ActianTimestampTypeMapping("timestamp with time zone", typeof(DateTimeOffset), withTimeZone: true);

        private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

        private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;

        // These are disallowed only if specified without any kind of length specified in parenthesis.
        private readonly HashSet<string> _disallowedMappings = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "binary",
            "binary varying",
            "varbinary",
            "byte",
            "byte varying",
            "varbyte",
            "char",
            "character",
            "char varying",
            "character varying",
            "varchar",
            "national char",
            "national character",
            "nchar",
            "national char varying",
            "national character varying",
            "nvarchar"
        };

        private readonly IReadOnlyDictionary<string, Func<Type, RelationalTypeMapping>> _namedClrMappings = new Dictionary<string, Func<Type, RelationalTypeMapping>>(StringComparer.Ordinal)
        {
        };

        public ActianTypeMappingSource(
            [NotNull] TypeMappingSourceDependencies dependencies,
            [NotNull] RelationalTypeMappingSourceDependencies relationalDependencies
            )
            : base(dependencies, relationalDependencies)
        {
            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(bool), _boolean },
                    // TODO: Implement uuid
                    //(typeof(Guid), _uuid),
                    { typeof(byte), _byte },
                    { typeof(short), _smallint },
                    { typeof(int), _integer },
                    { typeof(long), _bigint },
                    { typeof(float), _float4 },
                    { typeof(double), _float8 },
                    { typeof(decimal), _decimal },
                    { typeof(DateTime), _timestamp },
                    { typeof(DateTimeOffset), _timestampWithTimeZone },
                    { typeof(TimeSpan), _time }
                };

            _storeTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
            {
                { "boolean", _boolean },
                // TODO: Implement uuid
                //("uuid", _uuid),
                { "tinyint", _tinyint },
                { "smallint", _smallint },
                { "integer", _integer },
                { "bigint", _bigint },
                { "float4", _float4 },
                { "float", _float8 },
                { "decimal", _decimal },
                { "money", _money },
                { "char", _char },
                { "varchar", _varchar },
                { "long varchar", _longVarchar },
                { "nchar", _nchar },
                { "nvarchar", _nvarchar },
                { "long nvarchar", _longNVarchar },
                { "byte", _binary },
                { "varbyte", _varbinary },
                { "long byte", _longbinary },
                { "ingresdate", _ingresdate },
                { "ansidate", _ansidate },
                { "time", _time },
                { "time without time zone", _timeWithoutTimeZone },
                { "time with local time zone", _timeWithLocalTimeZone },
                { "time with time zone", _timeWithTimeZone },
                { "timestamp", _timestamp },
                { "timestamp without time zone", _timestampWithoutTimeZone },
                { "timestamp with local time zone", _timestampWithLocalTimeZone },
                { "timestamp with time zone", _timestampWithTimeZone }
            };
        }

        protected override void ValidateMapping(CoreTypeMapping mapping, IProperty property)
        {
            var relationalMapping = mapping as RelationalTypeMapping;

            if (_disallowedMappings.Contains(relationalMapping?.StoreType))
            {
                if (property == null)
                {
                    throw new ArgumentException(ActianStrings.UnqualifiedDataType(relationalMapping.StoreType));
                }

                throw new ArgumentException(ActianStrings.UnqualifiedDataTypeOnProperty(relationalMapping.StoreType, property.Name));
            }
        }

        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
            => FindRawMapping(mappingInfo)?.Clone(mappingInfo) ?? base.FindMapping(mappingInfo);

        private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                if (clrType == typeof(float)
                    && mappingInfo.Size != null
                    && mappingInfo.Size <= 24
                    && (storeTypeNameBase.Equals("float", StringComparison.OrdinalIgnoreCase))
                    )
                {
                    return _float4;
                }

                if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping) ||
                    _storeTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
                {
                    return clrType == null || mapping.ClrType == clrType
                        ? mapping
                        : null;
                }
            }

            if (clrType != null)
            {
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                    return mapping;

                if (_namedClrMappings.TryGetValue(clrType.FullName, out var mappingFunc))
                    return mappingFunc(clrType);

                if (clrType == typeof(string))
                {
                    var isUnicode = mappingInfo.IsUnicode ?? true;
                    var isFixedLength = mappingInfo.IsFixedLength ?? false;
                    var maxSize = isUnicode ? 16000 : 32000;

                    var size = mappingInfo.Size;
                    if (size > maxSize)
                    {
                        size = isFixedLength ? maxSize : (int?)null;
                    }

                    if (size is null)
                        return isUnicode ? _longNVarchar : _longVarchar;

                    return new ActianStringTypeMapping(unicode: isUnicode, size: size, fixedLength: isFixedLength);
                }

                if (clrType == typeof(byte[]))
                {
                    //if (mappingInfo.IsRowVersion == true)
                    //{
                    //    return _rowversion;
                    //}

                    var isFixedLength = mappingInfo.IsFixedLength ?? false;

                    var size = mappingInfo.Size ?? (mappingInfo.IsKeyOrIndex ? (int?)900 : null);
                    if (size > 8000)
                    {
                        size = isFixedLength ? 8000 : (int?)null;
                    }

                    return size == null
                        ? _longbinary
                        : new ActianByteArrayTypeMapping(size: size, fixedLength: isFixedLength);
                }
            }

            return null;
        }
    }
}
