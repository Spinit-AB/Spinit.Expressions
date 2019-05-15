using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Utility class for generating a predicate given a filter/dto. 
    /// <para>
    /// Handles simple properties, custom handlers could be added via <see cref="AddHandler{THandler}()"/>
    /// </para>
    /// </summary>
    /// <remarks>
    /// Intended use is to generate an expression for use in a ORM framework from a class normaly supplied by api/mvc.
    /// </remarks>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public class PredicateGenerator<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        private readonly IList<IPropertyPredicateHandler<TSource, TTarget>> _propertyPredicateHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateGenerator{TSource, TTarget}"/> class that
        /// contains the default simple propertytype handlers. 
        /// </summary>
        public PredicateGenerator()
        {
            _propertyPredicateHandlers = new List<IPropertyPredicateHandler<TSource, TTarget>>();
            AddHandler<SimplePropertyTypePredicateHandler<TSource, TTarget>>();
            AddHandler<SimpleEnumerablePropertyTypePredicateHandler<TSource, TTarget>>();
        }

        /// <summary>
        /// Adds a handler that can convert a property to an predicate.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public PredicateGenerator<TSource, TTarget> AddHandler<THandler>()
            where THandler : class, IPropertyPredicateHandler<TSource, TTarget>, new()
        {
            var handler = (IPropertyPredicateHandler<TSource, TTarget>)Activator.CreateInstance<THandler>();
            _propertyPredicateHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// Adds a handler that can convert a property to an predicate.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public PredicateGenerator<TSource, TTarget> AddHandler<THandler>(THandler handler)
            where THandler : class, IPropertyPredicateHandler<TSource, TTarget>
        {
            _propertyPredicateHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// Generates an predicate that operates on the target type. 
        /// </summary>
        /// <remarks>
        /// Handlers will be executed in reverse order for each property, if no handler handles the specific property an exception will be thrown.
        /// </remarks>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Expression<Func<TTarget, bool>> Generate(TSource source)
        {
            var predicates = new List<Expression<Func<TTarget, bool>>>();
            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                var predicate = GeneratePropertyPredicate(source, sourceProperty);
                if (predicate != null)
                    predicates.Add(predicate);
            }
            return predicates.Combine(CombineOperator.And);
        }

        private Expression<Func<TTarget, bool>> GeneratePropertyPredicate(TSource source, PropertyInfo sourceProperty)
        {
            foreach (var propertyPredicateHandler in _propertyPredicateHandlers.Reverse())
            {
                if (!propertyPredicateHandler.CanHandle(sourceProperty))
                    continue;
                return propertyPredicateHandler.Handle(source, sourceProperty);
            }
            throw new Exception($"No registered handler for property {source.GetType().Name}.{sourceProperty.Name}");
        }
    }
}
