using System.Linq.Expressions;

namespace Spinit.Expressions
{
    /// <summary>
    /// Enumeration for defining operator used when combining predicates.
    /// </summary>
    /// <seealso cref="ExpressionExtensions.Combine{T}(System.Collections.Generic.IEnumerable{Expression{System.Func{T, bool}}}, CombineOperator)"/>
    public enum CombineOperator
    {
        /// <summary>
        /// Combine the predicates using &amp;&amp;, eg using <see cref="Expression.AndAlso(Expression, Expression)"/>
        /// </summary>
        And = 1,

        /// <summary>
        /// Combine the predicates using ||, eg using <see cref="Expression.OrElse(Expression, Expression)"/>
        /// </summary>
        Or = 2
    }
}
