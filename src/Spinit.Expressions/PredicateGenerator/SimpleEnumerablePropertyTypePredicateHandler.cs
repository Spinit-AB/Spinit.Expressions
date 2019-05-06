using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Handles predicate for enumerables of simple types (eg value types and strings)
    /// </summary>
    public class SimpleEnumerablePropertyTypePredicateHandler<TSource, TTarget> : IPropertyPredicateHandler<TSource, TTarget>
    {
        /// <summary>
        /// Returns true if the property type is an enumerable of simple type (eg value types and strings)
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public bool CanHandle(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            return propertyType != typeof(string) && // string is an enumerable of char, should not be handled 
                propertyType.IsEnumerableOf(x => x.IsValueType || x == typeof(string));
        }

        /// <summary>
        /// Returns null is the enumerable property is empty or a predicate on <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo propertyInfo)
        {
            var propertyValue = propertyInfo.GetValue(source);
            if (propertyValue == null || ((IEnumerable)propertyValue).IsEmpty())
                return null;

            var propertyType = propertyInfo.PropertyType;
            var targetType = typeof(TTarget);
            var targetPropery = targetType.GetProperty(propertyInfo.Name); // TODO: add support for attribute for setting target property name
            if (targetPropery == null)
                throw new Exception($"{targetType.Name} does not contain a property named {propertyInfo.Name}");

            var genericEnumerableType = propertyType.GetEnumerableUnderlyingType();

            if (targetPropery.PropertyType != genericEnumerableType)
                throw new Exception($"{targetPropery.Name} must be of type {genericEnumerableType.Name}");

            var parameterExpression = Expression.Parameter(targetType, "x");
            var propertyExpression = Expression.Property(parameterExpression, targetPropery);
            var valuesExpression = Expression.Constant(propertyValue, propertyType);

            var containsMethodInfo = typeof(Enumerable).
                GetMethods().
                Where(x => x.Name == "Contains").
                Single(x => x.GetParameters().Length == 2).
                MakeGenericMethod(genericEnumerableType);

            var containsMethodExpression = Expression.Call(containsMethodInfo, valuesExpression, propertyExpression);
            var result = Expression.Lambda<Func<TTarget, bool>>(containsMethodExpression, parameterExpression);
            return result;
        }
    }
}
