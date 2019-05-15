using System;
using System.Linq.Expressions;

namespace Spinit.Expressions
{
    /// <summary>
    /// Utility class for defining predicate expressions.
    /// </summary>
    public static class Predicate
    {
        /// <summary>
        /// Utility method for defining predicate expression.
        /// </summary>
        /// <typeparam name="T">The type the predicate operates on.</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Of<T>(Expression<Func<T, bool>> expression)
        {
            return expression;
        }
    }
}
