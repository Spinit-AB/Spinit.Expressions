using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Base interface for all handlers that can construct a predicate for a property.
    /// </summary>
    /// <typeparam name="TSource">The source filter type</typeparam>
    /// <typeparam name="TTarget">The type that the predicate should operate on</typeparam>
    /// <see cref="PredicateGenerator{TSource, TTarget}"/>
    public interface IPropertyPredicateHandler<TSource, TTarget>
    {
        /// <summary>
        /// Returns true if the property can be handled by this handler.
        /// </summary>
        /// <param name="propertyInfo">The property to check</param>
        /// <returns>True if the handler can handle this property</returns>
        bool CanHandle(PropertyInfo propertyInfo);

        /// <summary>
        /// Builds a predicate for the specified property.
        /// </summary>
        /// <param name="source">The source filter</param>
        /// <param name="propertyInfo">The property</param>
        /// <returns>Returns a predicate or null if filter not set.</returns>
        Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo propertyInfo);
    }
}
