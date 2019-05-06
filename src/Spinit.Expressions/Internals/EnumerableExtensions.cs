using System.Collections;

namespace Spinit.Expressions
{
    internal static class EnumerableExtensions
    {
        internal static bool IsEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
