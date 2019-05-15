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
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        /// Returns true if the property can be handled by this handler, regardless of the actual value.
        /// </summary>
        /// <param name="sourceProperty">The property to check</param>
        /// <returns>True if the handler can handle this property.</returns>
        bool CanHandle(PropertyInfo sourceProperty);

        /// <summary>
        /// Builds a predicate for the specified source property.
        /// <para>Is only called if <see cref="CanHandle(PropertyInfo)"/> returns <see langword="true"/>.</para>
        /// </summary>
        /// <param name="source">The source filter</param>
        /// <param name="sourceProperty">The property</param>
        /// <returns>Returns a predicate or null if value not set.</returns>
        Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo sourceProperty);
    }
}
