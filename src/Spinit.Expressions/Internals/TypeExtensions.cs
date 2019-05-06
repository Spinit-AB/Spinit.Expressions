using System;
using System.Collections.Generic;
using System.Linq;

namespace Spinit.Expressions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if a type is a <see cref="Nullable{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNullable(this Type type)
        {
            return type.IsNullableOf(x => true);
        }

        /// <summary>
        /// Checks if a type is a <see cref="Nullable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNullableOf<T>(this Type type)
        {
            return type.IsNullableOf(x => x == typeof(T));
        }

        /// <summary>
        /// Checks if a type is a <see cref="Nullable{T}"/> where T matches the <paramref name="underlyingTypePredicate"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="underlyingTypePredicate"></param>
        /// <returns></returns>
        internal static bool IsNullableOf(this Type type, Func<Type, bool> underlyingTypePredicate)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
                return false;
            if (underlyingTypePredicate == null)
                return true;
            return underlyingTypePredicate(underlyingType);
        }

        /// <summary>
        /// Checks if a type is a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEnumerable(this Type type)
        {
            return type.IsEnumerableOf(x => true);
        }

        /// <summary>
        /// Checks if a type is a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEnumerableOf<T>(this Type type)
        {
            return type.IsEnumerableOf(x => x == typeof(T));
        }

        /// <summary>
        /// Checks if a type is a <see cref="Nullable{T}"/> where T matches the <paramref name="underlyingTypePredicate"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="underlyingTypePredicate"></param>
        /// <returns></returns>
        internal static bool IsEnumerableOf(this Type type, Func<Type, bool> underlyingTypePredicate)
        {
            var underlyingType = type.GetEnumerableUnderlyingType();
            if (underlyingType == null)
                return false;
            if (underlyingTypePredicate == null)
                return true;
            return underlyingTypePredicate(underlyingType);
        }

        internal static Type GetEnumerableUnderlyingType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            var underlyingTypes = type.GetInterfaces()
                .Where(
                    t => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GenericTypeArguments[0]);
            if (!underlyingTypes.Any())
                return null;
            return underlyingTypes.Single(); // TODO: throw better exception if type implements multiple IEnumerable<T>
        }
    }
}
