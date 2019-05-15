namespace Spinit.Expressions.UnitTests.ExpressionExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Spinit.Expressions;
    using Xunit;

    public class And
    {
        [Fact]
        public void ToStringShouldMatchExpectation()
        {
            Expression<Func<TestClass, bool>> first = x => x.Type == "Type";
            Expression<Func<TestClass, bool>> second = y => y.Title == "Title";
            Expression<Func<TestClass, bool>> expectedResult = x => x.Type == "Type" && x.Title == "Title";
            var result = first.And(second);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        public class TestClass
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
        }
    }

    public class Or
    {
        [Fact]
        public void ToStringShouldMatchExpectation()
        {
            Expression<Func<TestClass, bool>> first = x => x.Type == "Type";
            Expression<Func<TestClass, bool>> second = y => y.Title == "Title";
            Expression<Func<TestClass, bool>> expectedResult = x => x.Type == "Type" || x.Title == "Title";
            var result = first.Or(second);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        public class TestClass
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
        }
    }

    public class Combine
    {
        [Fact]
        public void WhenUsingAndToStringShouldMatchExpectation()
        {
            var predicates = new List<Expression<Func<TestClass, bool>>>
            {
                x => x.Id == 1,
                x => x.Id == 2,
                x => x.Id == 3,
            };
            Expression<Func<TestClass, bool>> expectedResult = x => x.Id == 1 && x.Id == 2 && x.Id == 3;
            var result = predicates.Combine(CombineOperator.And);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        [Fact]
        public void AndDocumentationExampleShouldWork()
        {
            var predicates = new List<Expression<Func<string, bool>>>
            {
                x => x.Contains("a"),
                y => y.Contains("b"),
                z => z.Contains("c")
            };
            Expression<Func<string, bool>> expectedResult = x => x.Contains("a") && x.Contains("b") && x.Contains("c");
            var result = predicates.Combine(CombineOperator.And);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        [Fact]
        public void WhenUsingOrToStringShouldMatchExpectation()
        {
            var predicates = new List<Expression<Func<TestClass, bool>>>
            {
                x => x.Id == 1,
                x => x.Id == 2,
                x => x.Id == 3,
            };
            Expression<Func<TestClass, bool>> expectedResult = x => x.Id == 1 || x.Id == 2 || x.Id == 3;
            var result = predicates.Combine(CombineOperator.Or);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        [Fact]
        public void OrDocumentationExampleShouldWork()
        {
            var predicates = new List<Expression<Func<string, bool>>>
            {
                x => x.Contains("a"),
                y => y.Contains("b"),
                z => z.Contains("c")
            };
            Expression<Func<string, bool>> expectedResult = x => x.Contains("a") || x.Contains("b") || x.Contains("c");
            var result = predicates.Combine(CombineOperator.Or);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        public class TestClass
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
        }
    }

    [Obsolete]
    public class RemapTo
    {
        [Fact]
        public void ExampleUsageShouldWork()
        {
            var predicate = Predicate.Of<int>(a => a > 10);
            var propertyExpression = TypeExpressions.GetPropertyExpression<TestClass, int>(nameof(TestClass.IntProperty2));
            var result = predicate.RemapTo(propertyExpression);
            Assert.Equal("x => (x.IntProperty2 > 10)", result.ToString());
        }

        [Fact]
        public void DocumentationExampleShouldWork()
        {
            Expression<Func<string, bool>> stringExpression = s => s == "123";
            var myClassExpression = stringExpression.RemapTo((MyClass x) => x.Id);
            var expectedResult = "x => (x.Id == \"123\")";
            Assert.Equal(expectedResult, myClassExpression.ToString());
        }

        public class MyClass
        {
            public string Id { get; set; }
        }

        public class TestClass
        {
            public int IntProperty1 { get; set; }
            public int IntProperty2 { get; set; }
            public int IntProperty3 { get; set; }
        }

        [Fact]
        public void ToStringShouldMatchExpectation()
        {
            Expression<Func<Entity, DateTime>> expression = x => x.CreatedDate;
            Expression<Func<EntityWrapper<Entity>, DateTime>> expectedResult = y => y.Entity.CreatedDate;
            var result = expression.RemapTo<EntityWrapper<Entity>, Entity, DateTime>(y => y.Entity);
            Assert.Equal(expectedResult.ToString(), result.ToString());
        }

        [Fact]
        public void UsingGetValueOrDefaultOnNullableTypesShouldWork()
        {
            Expression<Func<Entity, bool>> expression = x => x.ModifiedDate.GetValueOrDefault(DateTime.MinValue) >= DateTime.UtcNow;
            var result = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(y => y.Entity);
        }

        [Fact]
        public void UsingGetValueOrDefaultOnNullableTypesShouldReturnExpectedResult()
        {
            var now = DateTime.UtcNow;
            Expression<Func<Entity, bool>> expression = x => x.ModifiedDate.GetValueOrDefault(DateTime.MinValue) >= now.AddDays(-7);
            var predicate = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(y => y.Entity);
            var list = new List<EntityWrapper<Entity>>
                {
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldBeFound", ModifiedDate = now.AddDays(-1) }),
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldNotBeFound", ModifiedDate = null }),
                };
            var result = list.AsQueryable().Where(predicate);
            Assert.Contains(result, x => x.Entity.Title == "ShouldBeFound");
            Assert.DoesNotContain(result, x => x.Entity.Title == "ShouldNotBeFound");
        }

        [Fact]
        public void ParameterNamingShouldBeAgnostic()
        {
            var now = DateTime.UtcNow;
            Expression<Func<Entity, bool>> expression = a => a.ModifiedDate.GetValueOrDefault(DateTime.MinValue) >= now.AddDays(-7);
            var predicate = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(b => b.Entity);
            var list = new List<EntityWrapper<Entity>>
                {
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldBeFound", ModifiedDate = now.AddDays(-1) }),
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldNotBeFound", ModifiedDate = null }),
                };
            var result = list.AsQueryable().Where(predicate);
            Assert.Contains(result, c => c.Entity.Title == "ShouldBeFound");
            Assert.DoesNotContain(result, d => d.Entity.Title == "ShouldNotBeFound");
        }

        [Fact]
        public void InterfaceConversionShouldReturnExpectedResult()
        {
            var now = DateTime.UtcNow;
            var expression = new Filterer<Entity>(now).Filter();
            var predicate = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(x => x.Entity);
            var list = new List<EntityWrapper<Entity>>
                {
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldBeFound", CreatedDate = now.AddMonths(-1) }),
                    new EntityWrapper<Entity>(new Entity { Title = "ShouldNotBeFound", CreatedDate = now.AddDays(1) })
                };

            var result = list.AsQueryable().Where(predicate);
            Assert.Contains(result, x => x.Entity.Title == "ShouldBeFound");
            Assert.DoesNotContain(result, x => x.Entity.Title == "ShouldNotBeFound");
        }

        [Fact]
        public void MultipleLevelsShouldReturnExpectedResult()
        {
            var now = DateTime.UtcNow;
            var expression = new Filterer<Entity>(now).Filter();
            var predicate = expression.RemapTo<NestingEntityWrapper<Entity>, Entity, bool>(x => x.InnerWrapper.Entity);
            var list = new List<NestingEntityWrapper<Entity>>
                {
                    new NestingEntityWrapper<Entity>(new Entity { Title = "ShouldBeFound", CreatedDate = now.AddMonths(-1) }),
                    new NestingEntityWrapper<Entity>(new Entity { Title = "ShouldNotBeFound", CreatedDate = now.AddDays(1) })
                };

            var result = list.AsQueryable().Where(predicate);
            Assert.Contains(result, x => x.InnerWrapper.Entity.Title == "ShouldBeFound");
            Assert.DoesNotContain(result, x => x.InnerWrapper.Entity.Title == "ShouldNotBeFound");
        }

        [Fact]
        public void UsingInterfacesShouldReturnExpectedResult()
        {
            Expression<Func<IEntity, bool>> expression = a => a.Id == 1;
            var predicate = expression.RemapTo<EntityWrapper<IEntity>, IEntity, bool>(b => b.Entity);
            var list = new List<EntityWrapper<IEntity>>
                {
                    new EntityWrapper<IEntity>(new Entity { Id = 1 }),
                    new EntityWrapper<IEntity>(new Entity { Id = 2 })
                };

            var result = list.AsQueryable().Where(predicate);
            Assert.Single(result);
            Assert.Equal(1, result.Single().Entity.Id);
        }

        public interface IEntity
        {
            int Id { get; set; }
        }

        public interface IEntityWithCreatedDate : IEntity
        {
            DateTime CreatedDate { get; set; }
        }

        public class Entity : IEntity, IEntityWithCreatedDate
        {
            public int Id { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class EntityWrapper<TEntity>
            where TEntity : class, IEntity
        {
            public EntityWrapper(TEntity entity)
            {
                Entity = entity;
            }

            public TEntity Entity { get; set; }
        }

        public class NestingEntityWrapper<TEntity>
            where TEntity : class, IEntity
        {
            public NestingEntityWrapper(TEntity entity)
            {
                InnerWrapper = new EntityWrapper<TEntity>(entity);
            }

            public EntityWrapper<TEntity> InnerWrapper { get; set; }
        }

        public class Filterer<T>
            where T : IEntityWithCreatedDate
        {
            private readonly DateTime _maxDateToInclude;

            public Filterer(DateTime maxDateToInclude)
            {
                _maxDateToInclude = maxDateToInclude;
            }

            public Expression<Func<T, bool>> Filter() => t => t.CreatedDate < _maxDateToInclude;
        }
    }
}
