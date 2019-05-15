
namespace Spinit.Expressions.UnitTests.TypeExpressions
{
    using System;
    using System.Linq;
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

        [Fact]
        public void DocumentationExampleShouldWork()
        {
            var data = new[]
            {
                new MyEntity
                {
                    Name = "b"
                },
                new MyEntity
                {
                    Name = "c"
                },
                new MyEntity
                {
                    Name = "a"
                }
            };
            var orderBy = nameof(MyEntity.Name);
            var _entities = data.AsQueryable();
            var orderByExpression = TypeExpressions.GetPropertyExpression<MyEntity>(orderBy);
            var orderedData = _entities.OrderBy(orderByExpression);
            Assert.Equal(new[] { "a", "b", "c" }, orderedData.Select(x => x.Name).ToArray());
        }

        public class MyEntity
        {
            public string Name { get; set; }
        }
    }

    public class TestClass
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
        public decimal DecimalProp { get; set; }
    }
}
