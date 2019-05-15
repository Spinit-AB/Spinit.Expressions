using System;
using System.Linq.Expressions;

namespace Spinit.Expressions
{
    /// <summary>
    /// Contains type expression utilities
    /// </summary>
    public static class TypeExpressions
    {
        /// <summary>
        /// Returns an expression for the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Expression<Func<T, TPropertyType>> GetPropertyExpression<T, TPropertyType>(string propertyName)
            where T : class
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            Expression propertyExpression = Expression.Property(parameterExpression, propertyName);
            if (propertyExpression.Type != typeof(TPropertyType))
                propertyExpression = Expression.Convert(propertyExpression, typeof(TPropertyType));
            return Expression.Lambda<Func<T, TPropertyType>>(propertyExpression, parameterExpression);
        }

        /// <summary>
        /// Returns an expression for the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Expression<Func<T, object>> GetPropertyExpression<T>(string propertyName)
            where T : class
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            Expression propertyExpression = Expression.Property(parameterExpression, propertyName);
            if (propertyExpression.Type.IsValueType)
                propertyExpression = Expression.Convert(propertyExpression, typeof(object));
            return Expression.Lambda<Func<T, object>>(propertyExpression, parameterExpression);
        }
    }
}
