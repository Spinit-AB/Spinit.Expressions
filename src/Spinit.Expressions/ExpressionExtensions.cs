﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Spinit.Expressions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two expressions using AndAlso (&amp;&amp;) and uses parameters from the first expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines two expressions using OrElse (||) and uses parameters from the first expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;
            return first.Compose(second, Expression.OrElse);
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
        public static Expression<Func<TTarget, TResult>> RemapTo<TTarget, TSource, TResult>(this Expression<Func<TSource, TResult>> source, Expression<Func<TTarget, TSource>> selector)
        {
            var expression = RemapperExpressionVisitor<TTarget, TSource, TResult>.Map(source, selector) as LambdaExpression;
            return Expression.Lambda<Func<TTarget, TResult>>(expression.Body, selector.Parameters);
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> mergeFunc)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(mergeFunc(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly IDictionary<ParameterExpression, ParameterExpression> _parametersMap;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> parametersMap)
            {
                _parametersMap = parametersMap ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> parametersMap, Expression expression)
            {
                return new ParameterRebinder(parametersMap).Visit(expression);
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                if (_parametersMap.TryGetValue(parameter, out var replacement))
                {
                    parameter = replacement;
                }

                return base.VisitParameter(parameter);
            }
        }

        private class RemapperExpressionVisitor<TTarget, TSource, TResult> : ExpressionVisitor
        {
            private readonly Expression<Func<TTarget, TSource>> _selector;

            RemapperExpressionVisitor(Expression<Func<TTarget, TSource>> selector)
            {
                _selector = selector;
            }

            public static Expression Map(Expression<Func<TSource, TResult>> expression, Expression<Func<TTarget, TSource>> selector)
            {
                return new RemapperExpressionVisitor<TTarget, TSource, TResult>(selector).Visit(expression);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (
                        node.Expression != null && 
                        (node.Expression.NodeType == ExpressionType.Parameter || node.Expression.NodeType == ExpressionType.Convert) && 
                        (node.Expression.Type == typeof(TSource) || node.Expression.Type.IsAssignableFrom(typeof(TSource))))
                    return Expression.PropertyOrField(Visit(_selector.Body), node.Member.Name);
                else
                    return base.VisitMember(node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return Expression.Lambda<Func<TTarget, TResult>>(Visit(node.Body), _selector.Parameters);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                node = _selector.Parameters.Single(x => x.Name == node.Name);
                return base.VisitParameter(node);
            }
        }
    }
}
