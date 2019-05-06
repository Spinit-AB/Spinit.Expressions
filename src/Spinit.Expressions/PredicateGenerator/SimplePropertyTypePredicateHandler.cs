using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Handles predicate for simple types (eg value types and strings)
    /// </summary>
    public class SimplePropertyTypePredicateHandler<TSource, TTarget> : IPropertyPredicateHandler<TSource, TTarget>
    {
        /// <summary>
        /// Returns true if the property type is a simple type (eg value types and strings)
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public bool CanHandle(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            bool isSimpleType(Type x) => x.IsValueType || x == typeof(string);
            return
                isSimpleType(propertyType) ||
                propertyType.IsNullableOf(isSimpleType);
        }

        /// <summary>
        /// Returns null is the property is null or a predicate on <typeparamref name="TSource"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo propertyInfo)
        {
            var propertyValue = propertyInfo.GetValue(source);
            if (propertyValue == null)
                return null;

            var propertyType = propertyInfo.PropertyType;
            var targetType = typeof(TTarget);
            var targetPropery = targetType.GetProperty(propertyInfo.Name); // TODO: add support for attribute for setting target property name
            if (targetPropery == null)
                throw new Exception($"{targetType.Name} does not contain a property named {propertyInfo.Name}");

            if (propertyType.IsNullable())
                propertyType = Nullable.GetUnderlyingType(propertyType);

            if (targetPropery.PropertyType != propertyType)
                throw new Exception($"{targetPropery.Name} must be of type {propertyType.Name}");

            var parameterExpression = Expression.Parameter(targetType, "x");
            var propertyExpression = Expression.Property(parameterExpression, targetPropery);
            var valueExpression = Expression.Constant(propertyValue, propertyType);
            var equalExpression = Expression.Equal(propertyExpression, valueExpression);
            var result = Expression.Lambda<Func<TTarget, bool>>(equalExpression, parameterExpression);
            return result;
        }
    }
}
