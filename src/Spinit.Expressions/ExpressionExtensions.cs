using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Spinit.Expressions
{
    /// <summary>
    /// Contains expression extensions
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two predicates using AndAlso (&amp;&amp;) and uses parameters from the first expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines two predicates using OrElse (||) and uses parameters from the first expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.OrElse);
        }

        /// <summary>
        /// Combines a collection of predicates using the supplied operator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicates"></param>
        /// <param name="combineOperator"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Combine<T>(this IEnumerable<Expression<Func<T, bool>>> predicates, CombineOperator combineOperator)
        {
            if (predicates == null || !predicates.Any())
                return null;

            Func<Expression, Expression, BinaryExpression> operatorFunc;
            switch (combineOperator)
            {
                case CombineOperator.And:
                    operatorFunc = Expression.AndAlso;
                    break;
                case CombineOperator.Or:
                    operatorFunc = Expression.OrElse;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(combineOperator));
            }
            return predicates.Aggregate((Expression<Func<T, bool>>)null, (result, next) => result.Combine(next, operatorFunc));
        }

        /// <summary>
        /// Remaps an expression on a type to another type that have the original type as a property.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        [Obsolete("Use Replace()")]
        public static Expression<Func<TTarget, TResult>> RemapTo<TTarget, TSource, TResult>(this Expression<Func<TSource, TResult>> source, Expression<Func<TTarget, TSource>> selector)
        {
            return source.Replace(selector);
        }

        /// <summary>
        /// Replaces parameters in a lambda expression using a replacement expression.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static Expression<Func<TTarget, TResult>> Replace<TTarget, TSource, TResult>(this Expression<Func<TSource, TResult>> source, Expression<Func<TTarget, TSource>> replacement)
        {
            var map = new Dictionary<ParameterExpression, Expression>
            {
                [source.Parameters.Single()] = replacement.Body
            };
            var resultBody = new ParameterReplacerVisitor(map).Visit(source.Body);
            return Expression.Lambda<Func<TTarget, TResult>>(resultBody, replacement.Parameters);
        }

        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, BinaryExpression> operatorFunc)
        {
            if (first == null || second == null)
                return first ?? second;

            var map = second.Parameters
                .Zip(first.Parameters, (key, value) => new { key, value })
                .ToDictionary(x => x.key, x => (Expression)x.value);
            var secondBody = new ParameterReplacerVisitor(map).Visit(second.Body);
            return Expression.Lambda<T>(operatorFunc(first.Body, secondBody), first.Parameters);
        }

        private class ParameterReplacerVisitor : ExpressionVisitor
        {
            private readonly IDictionary<ParameterExpression, Expression> _parametersMap;

            public ParameterReplacerVisitor(IDictionary<ParameterExpression, Expression> parametersMap)
            {
                _parametersMap = parametersMap ?? new Dictionary<ParameterExpression, Expression>();
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                if (_parametersMap.TryGetValue(parameter, out var replacement))
                {
                    return replacement;
                }

                return parameter;
            }
        }
    }
}
