using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Handles predicate for simple types
    /// </summary>
    public class SimplePropertyTypePredicateHandler<TSource, TTarget> : BasePropertyPredicateHandler<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        /// Returns true if the property type is a simple type
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        public override bool CanHandle(PropertyInfo sourceProperty)
        {
            var targetProperty = GetTargetProperty(sourceProperty);
            if (targetProperty == null)
                return false;

            var sourcePropertyType = sourceProperty.PropertyType;
            if (sourcePropertyType.IsNullable())
                sourcePropertyType = Nullable.GetUnderlyingType(sourcePropertyType);
            if (sourcePropertyType.IsEnum)
                sourcePropertyType = sourcePropertyType.GetEnumUnderlyingType();

            var targetPropertyType = targetProperty.PropertyType;
            if (targetPropertyType.IsNullable())
                targetPropertyType = Nullable.GetUnderlyingType(targetPropertyType);
            if (targetPropertyType.IsEnum)
                targetPropertyType = targetPropertyType.GetEnumUnderlyingType();

            return sourcePropertyType.IsSimple()
                && targetPropertyType.IsSimple()
                && targetPropertyType.IsEquatableOf(sourcePropertyType);
        }

        /// <summary>
        /// Returns null if the property is null or a predicate on <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        public override Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo sourceProperty)
        {
            var sourcePropertyValue = sourceProperty.GetValue(source);
            if (sourcePropertyValue == null)
                return null;

            var targetProperty = GetTargetProperty(sourceProperty);
            var parameterExpression = Expression.Parameter(typeof(TTarget), "x");
            var propertyExpression = Expression.Property(parameterExpression, targetProperty);
            Expression valueExpression = Expression.Constant(sourcePropertyValue);
            if (sourcePropertyValue.GetType() != targetProperty.PropertyType)
                valueExpression = Expression.Convert(valueExpression, targetProperty.PropertyType);
            var equalExpression = Expression.Equal(propertyExpression, valueExpression);
            var result = Expression.Lambda<Func<TTarget, bool>>(equalExpression, parameterExpression);
            return result;
        }
    }
}
