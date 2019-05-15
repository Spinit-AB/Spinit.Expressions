using System;
using System.Collections.Generic;
using System.Linq;

namespace Spinit.Expressions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks if a type is "simple" eg holds a single value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsSimple(this Type type)
        {
            if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type);
            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }

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
        internal static bool IsNullableOf(this Type type, Predicate<Type> underlyingTypePredicate)
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
        internal static bool IsEnumerableOf(this Type type, Predicate<Type> underlyingTypePredicate)
        {
            var underlyingType = type.GetEnumerableUnderlyingType();
            if (underlyingType == null)
                return false;
            if (underlyingTypePredicate == null)
                return true;
            return underlyingTypePredicate(underlyingType);
        }

        /// <summary>
        /// Checks if a type is equatable to it self, eg implements <see cref="IEquatable{T}"/> where T is <paramref name="type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEquatable(this Type type)
        {
            return type.IsEquatableOf(x => x == type);
        }

        /// <summary>
        /// Checks if a type implements <see cref="IEquatable{T}"/> where T is <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEquatableOf<T>(this Type type)
        {
            return type.IsEquatableOf(typeof(T));
        }

        /// <summary>
        /// Checks if a type implements <see cref="IEquatable{T}"/> where T is <paramref name="underlyingType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="underlyingType"></param>
        /// <returns></returns>
        internal static bool IsEquatableOf(this Type type, Type underlyingType)
        {
            return type.IsEquatableOf(x => x == underlyingType);
        }

        /// <summary>
        /// Checks if a type implements <see cref="IEquatable{T}"/> where T matches the <paramref name="underlyingTypePredicate"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="underlyingTypePredicate"></param>
        /// <returns></returns>
        internal static bool IsEquatableOf(this Type type, Predicate<Type> underlyingTypePredicate)
        {
            var SimpleTypes = type.GetInterfaces()
                .Where(
                    t => t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(IEquatable<>))
                .Select(t => t.GenericTypeArguments[0]);
            if (!SimpleTypes.Any())
                return false;
            return SimpleTypes.Any(x => underlyingTypePredicate(x));
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
                .Select(t => t.GenericTypeArguments[0])
                .ToList();
            if (!underlyingTypes.Any())
                return null;
            if (underlyingTypes.Count > 1)
                throw new ArgumentException($"{type.Name} implements multiple enumerables");
            return underlyingTypes.First();
        }
    }
}
