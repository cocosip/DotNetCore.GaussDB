using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using GaussDB.Internal.Postgres;
using GaussDB.PostgresTypes;

namespace GaussDB.Internal;

[RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
public abstract class DynamicTypeInfoResolver : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (dataTypeName is null)
            return null;

        var context = GetMappings(type, dataTypeName.GetValueOrDefault(), options);
        return context?.Find(type, dataTypeName.GetValueOrDefault(), options);
    }

    protected static DynamicMappingCollection CreateCollection(TypeInfoMappingCollection? baseCollection = null) => new(baseCollection);

    protected static bool IsTypeOrNullableOfType(Type type, Func<Type, bool> predicate, out Type matchedType)
    {
        matchedType = Nullable.GetUnderlyingType(type) ?? type;
        return predicate(matchedType);
    }

    protected static bool IsArrayLikeType(Type type, [NotNullWhen(true)] out Type? elementType) => TypeInfoMappingCollection.IsArrayLikeType(type, out elementType);

    protected static bool IsArrayDataTypeName(DataTypeName dataTypeName, PgSerializerOptions options, out DataTypeName elementDataTypeName)
    {
        if (options.DatabaseInfo.GetPostgresType(dataTypeName) is PostgresArrayType arrayType)
        {
            elementDataTypeName = arrayType.Element.DataTypeName;
            return true;
        }

        elementDataTypeName = default;
        return false;
    }

    protected abstract DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options);

    [RequiresDynamicCode("A dynamic type info resolver may need to construct a generic converter for a statically unknown type.")]
    protected class DynamicMappingCollection
    {
        TypeInfoMappingCollection? _mappings;

        static readonly MethodInfo AddTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddStructType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(
            nameof(TypeInfoMappingCollection.AddResolverType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddResolverArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(
            nameof(TypeInfoMappingCollection.AddResolverStructType),
            new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();

        static readonly MethodInfo AddResolverStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
            .GetMethod(nameof(TypeInfoMappingCollection.AddResolverStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();

        internal DynamicMappingCollection(TypeInfoMappingCollection? baseCollection = null)
        {
            if (baseCollection is not null)
                _mappings = new(baseCollection);
        }

#if NET9_0_OR_GREATER
        public DynamicMappingCollection AddMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported, map its underlying type instead to get both.");

            // Call appropriate method based on whether it's a value type or reference type
            if (type.IsValueType)
            {
                AddStructTypeMapping(type, dataTypeName, factory, configureMapping);
            }
            else
            {
                AddReferenceTypeMapping(type, dataTypeName, factory, configureMapping);
            }

            return this;
        }

        private void AddStructTypeMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping)
        {
            // Call the method directly for structs (value types)
            AddStructTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName, factory, configureMapping });
        }

        private void AddReferenceTypeMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping)
        {
            // Call the method directly for reference types
            AddTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName, factory, configureMapping });
        }

        public DynamicMappingCollection AddArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the appropriate method for arrays based on whether it's a value type or reference type
            if (elementType.IsValueType)
            {
                AddStructArrayMapping(elementType, dataTypeName);
            }
            else
            {
                AddReferenceArrayMapping(elementType, dataTypeName);
            }

            return this;
        }

        private void AddStructArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the method directly for value type arrays (structs)
            AddStructArrayTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName });
        }

        private void AddReferenceArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the method directly for reference type arrays
            AddArrayTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName });
        }

        public DynamicMappingCollection AddResolverMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported");

            // Call the appropriate resolver method based on whether it's a value type or reference type
            if (type.IsValueType)
            {
                AddResolverStructTypeMapping(type, dataTypeName, factory, configureMapping);
            }
            else
            {
                AddResolverReferenceTypeMapping(type, dataTypeName, factory, configureMapping);
            }

            return this;
        }

        private void AddResolverStructTypeMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping)
        {
            // Call the resolver method directly for structs (value types)
            AddResolverStructTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName, factory, configureMapping });
        }

        private void AddResolverReferenceTypeMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping)
        {
            // Call the resolver method directly for reference types
            AddResolverTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName, factory, configureMapping });
        }

        public DynamicMappingCollection AddResolverArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the appropriate resolver method for arrays based on whether it's a value type or reference type
            if (elementType.IsValueType)
            {
                AddResolverStructArrayMapping(elementType, dataTypeName);
            }
            else
            {
                AddResolverReferenceArrayMapping(elementType, dataTypeName);
            }

            return this;
        }

        private void AddResolverStructArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the resolver method directly for value type arrays (structs)
            AddResolverStructArrayTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName });
        }

        private void AddResolverReferenceArrayMapping(Type elementType, string dataTypeName)
        {
            // Call the resolver method directly for reference type arrays
            AddResolverArrayTypeMethodInfo.Invoke(_mappings ??= new(), new object?[] { dataTypeName });
        }

#else
        public DynamicMappingCollection AddMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported, map its underlying type instead to get both.");

            (type.IsValueType ? AddStructTypeMethodInfo : AddTypeMethodInfo)
                .MakeGenericMethod(type).Invoke(_mappings ??= new(), new object?[]
                {
                    dataTypeName,
                    factory,
                    configureMapping
                });
            return this;
        }

        public DynamicMappingCollection AddArrayMapping(Type elementType, string dataTypeName)
        {
            (elementType.IsValueType ? AddStructArrayTypeMethodInfo : AddArrayTypeMethodInfo)
                .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), new object?[] { dataTypeName });
            return this;
        }

        public DynamicMappingCollection AddResolverMapping(Type type, string dataTypeName, TypeInfoFactory factory, Func<TypeInfoMapping, TypeInfoMapping>? configureMapping = null)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is not null)
                throw new NotSupportedException("Mapping nullable types is not supported");

            (type.IsValueType ? AddResolverStructTypeMethodInfo : AddResolverTypeMethodInfo)
                .MakeGenericMethod(type).Invoke(_mappings ??= new(), new object?[]
                {
                    dataTypeName,
                    factory,
                    configureMapping
                });
            return this;
        }

        public DynamicMappingCollection AddResolverArrayMapping(Type elementType, string dataTypeName)
        {
            (elementType.IsValueType ? AddResolverStructArrayTypeMethodInfo : AddResolverArrayTypeMethodInfo)
                .MakeGenericMethod(elementType).Invoke(_mappings ??= new(), new object?[] { dataTypeName });
            return this;
        }

#endif

        internal PgTypeInfo? Find(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => _mappings?.Find(type, dataTypeName, options);

        public TypeInfoMappingCollection ToTypeInfoMappingCollection()
            => new(_mappings?.Items ?? Array.Empty<TypeInfoMapping>());
    }
}
