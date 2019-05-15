using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Handles predicate for enumerables of simple types
    /// </summary>
    public class SimpleEnumerablePropertyTypePredicateHandler<TSource, TTarget> : BasePropertyPredicateHandler<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        /// Returns true if the property type is an enumerable of simple type
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        public override bool CanHandle(PropertyInfo sourceProperty)
        {
            var targetProperty = GetTargetProperty(sourceProperty);
            if (targetProperty == null)
                return false;

            var targetPropertyType = targetProperty.PropertyType;
            if (targetPropertyType.IsNullable())
                targetPropertyType = Nullable.GetUnderlyingType(targetPropertyType);
            if (targetPropertyType.IsEnum)
                targetPropertyType = targetPropertyType.GetEnumUnderlyingType();

            var sourcePropertyType = sourceProperty.PropertyType;
            return sourcePropertyType != typeof(string) // string is an enumerable of char, should not be handled 
                && sourcePropertyType.IsEnumerableOf(x =>
                    x.IsSimple() &&
                    targetPropertyType == (x.IsEnum
                        ? x.GetEnumUnderlyingType()
                        : x));
        }

        /// <summary>
        /// Returns null is the enumerable property is empty or a predicate on <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        public override Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo sourceProperty)
        {
            var sourcePropertyValue = sourceProperty.GetValue(source);
            if (sourcePropertyValue == null || ((IEnumerable)sourcePropertyValue).IsEmpty())
                return null;

            var targetProperty = GetTargetProperty(sourceProperty);
            var parameterExpression = Expression.Parameter(typeof(TTarget), "x");
            var propertyExpression = Expression.Property(parameterExpression, targetProperty);
            var valuesExpression = Expression.Constant(sourcePropertyValue);
            var genericEnumerableType = sourceProperty.PropertyType.GetEnumerableUnderlyingType();
            var containsMethodInfo = typeof(Enumerable).GetMethods()
                .Where(x => x.Name == "Contains")
                .Single(x => x.GetParameters().Length == 2)
                .MakeGenericMethod(genericEnumerableType);
            var containsMethodExpression = Expression.Call(containsMethodInfo, valuesExpression, propertyExpression);
            var result = Expression.Lambda<Func<TTarget, bool>>(containsMethodExpression, parameterExpression);
            return result;
        }
    }
}
