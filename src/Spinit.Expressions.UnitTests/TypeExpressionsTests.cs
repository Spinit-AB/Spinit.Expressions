
namespace Spinit.Expressions.UnitTests.TypeExpressions
{
    using System;
    using System.Linq.Expressions;
    using Spinit.Expressions;
    using Xunit;

    public class GetPropertyExpressionT2
    {
        [Fact]
        public void ShouldReturnExpectedForIntProp()
        {
            var result = TypeExpressions.GetPropertyExpression<TestClass, int>(nameof(TestClass.IntProp));
            Expression<Func<TestClass, int>> expected = x => x.IntProp;
            Assert.Equal(expected.ToString(), result.ToString());
        }

        [Fact]
        public void ShouldReturnExpectedWhenCastingDecimalProp()
        {
            var result = TypeExpressions.GetPropertyExpression<TestClass, double>(nameof(TestClass.DecimalProp));
            Expression<Func<TestClass, double>> expected = x => (double)x.DecimalProp;
            Assert.Equal(expected.ToString(), result.ToString());
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenInvalidProperty()
        {
            Assert.Throws<ArgumentException>(() => TypeExpressions.GetPropertyExpression<TestClass, int>("ComputerSaysNo"));
        }

        [Fact]
        public void ShouldThrowInvalidOperationExceptionWhenInvalidCast()
        {
            Assert.Throws<InvalidOperationException>(() => TypeExpressions.GetPropertyExpression<TestClass, DateTime>(nameof(TestClass.StringProp)));
        }
    }

    public class GetPropertyExpressionT1
    {
        [Fact]
        public void ShouldReturnExpectedForIntProp()
        {
            var result = TypeExpressions.GetPropertyExpression<TestClass>(nameof(TestClass.IntProp));
            Expression<Func<TestClass, object>> expected = x => x.IntProp;
            Assert.Equal(expected.ToString(), result.ToString());
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenInvalidProperty()
        {
            Assert.Throws<ArgumentException>(() => TypeExpressions.GetPropertyExpression<TestClass>("ComputerSaysNo"));
        }
    }

    public class TestClass
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
        public decimal DecimalProp { get; set; }
    }
}
