using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Abstract class for implementing a range/between filter handler.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TRange"></typeparam>
    public abstract class RangePropertyPredicateHandler<TSource, TTarget, TRange> : BasePropertyPredicateHandler<TSource, TTarget>
        where TSource : class
        where TTarget : class
        where TRange : struct, IComparable<TRange>
    {
         /// <summary>
        /// Builds a predicate for the specified source property.
        /// <para>Is only called if <see cref="IPropertyPredicateHandler{TSource, TTarget}.CanHandle(PropertyInfo)"/> returns <see langword="true"/>.</para>
        /// </summary>
        /// <param name="source">The source filter</param>
        /// <param name="sourceProperty">The property</param>
        /// <returns>Returns a predicate or null if value not set.</returns>
        public override sealed Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo sourceProperty)
        {
            (TRange? min, TRange? max) = GetRange(source, sourceProperty);
            if (min == null && max == null)
                return null;

            var targetProperty = GetTargetProperty(sourceProperty);
            if (targetProperty == null)
                throw new ArgumentException($"No target property found for {source.GetType().Name}.{sourceProperty.Name}.");
            var targetPropertyType = targetProperty.PropertyType;

            Expression<Func<TRange, bool>> predicate = null;
            if (min.HasValue)
                predicate = predicate.And(CreateGreaterThenOrEqualPredicate(min.Value));
            if (max.HasValue)
                predicate = predicate.And(CreateLessThanOrEqualPredicate(max.Value));

            if (targetPropertyType == typeof(TRange?))
            {
                var nullCheck = Predicate.Of<TRange?>(x => x.HasValue);
                Expression<Func<TRange?, TRange>> nullableReplacement = x => x.Value;
                var nullablePredicate = predicate.Replace(nullableReplacement);
                nullablePredicate = nullCheck.And(nullablePredicate);
                return nullablePredicate.Replace(TypeExpressions.GetPropertyExpression<TTarget, TRange?>(targetProperty.Name));
            }
            else
            {
                return predicate.Replace(TypeExpressions.GetPropertyExpression<TTarget, TRange>(targetProperty.Name));
            }
        }

        /// <summary>
        /// Should return the range that should be applied.
        /// <para>If min and max is null no predicate is applied.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        protected abstract (TRange? min, TRange? max) GetRange(TSource source, PropertyInfo sourceProperty);

        internal static Expression<Func<TRange, bool>> CreateGreaterThenOrEqualPredicate(TRange min)
        {
            //return Predicate.Of<TRange>(x => x >= min);
            return CreateBetweenPredicate(min, Expression.GreaterThanOrEqual);
        }

        internal static Expression<Func<TRange, bool>> CreateLessThanOrEqualPredicate(TRange max)
        {
            //return Predicate.Of<TRange>(x => x <= max);
            return CreateBetweenPredicate(max, Expression.LessThanOrEqual);
        }

        internal static Expression<Func<TRange, bool>> CreateBetweenPredicate(TRange maxValue, Func<Expression, Expression, BinaryExpression> comparisonFunc)
        {
            var parameter = Expression.Parameter(typeof(TRange));
            return Expression.Lambda<Func<TRange, bool>>(comparisonFunc(parameter, Expression.Constant(maxValue)), parameter);
        }

        
    }
}
