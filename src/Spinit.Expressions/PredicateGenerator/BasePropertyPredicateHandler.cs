using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Spinit.Expressions
{
    /// <summary>
    /// Basic <see langword="abstract"/> implementation of <see cref="IPropertyPredicateHandler{TSource, TTarget}"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public abstract class BasePropertyPredicateHandler<TSource, TTarget> : IPropertyPredicateHandler<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        /// Should true if the property can be handled by this handler.
        /// </summary>
        /// <param name="sourceProperty">The property to check</param>
        /// <returns>True if the handler can handle this property</returns>
        public abstract bool CanHandle(PropertyInfo sourceProperty);
        
        /// <summary>
        /// Should builds a predicate for the specified source property.
        /// </summary>
        /// <param name="source">The source filter</param>
        /// <param name="sourceProperty">The property</param>
        /// <returns>Returns a predicate or null if filter not set.</returns>
        public abstract Expression<Func<TTarget, bool>> Handle(TSource source, PropertyInfo sourceProperty);

        /// <summary>
        /// Returns the target property name, via <see cref="TargetPropertyNameAttribute"/> or convention.
        /// </summary>
        /// <param name="sourceProperty"></param>
        /// <returns>The target property or <see langword="null"/> if not found</returns>
        protected internal static PropertyInfo GetTargetProperty(PropertyInfo sourceProperty)
        {
            var propertyName = sourceProperty.GetCustomAttribute<TargetPropertyNameAttribute>()?.Name ?? sourceProperty.Name;
            return typeof(TTarget).GetProperty(propertyName);
        }
    }
}
