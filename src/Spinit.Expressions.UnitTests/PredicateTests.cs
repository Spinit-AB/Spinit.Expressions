using System;
using System.Linq.Expressions;
using Xunit;

namespace Spinit.Expressions.UnitTests
{
    public class PredicateTests
    {
        [Fact]
        public void DocumentationExampleShouldWork()
        {
            Expression<Func<string, bool>> expectedPredicate = s => s == "123";
            var predicate = Predicate.Of<string>(s => s == "123");
            Assert.Equal(expectedPredicate.ToString(), predicate.ToString());
        }
    }
}
