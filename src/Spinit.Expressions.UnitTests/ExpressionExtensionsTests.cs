using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Spinit.Expressions.UnitTests
{
    public class ExpressionExtensionsTests
    {
        public class And
        {
            [Fact]
            public void ToStringShouldMatchExpectation()
            {
                Expression<Func<TestClass, bool>> firsts = x => x.Type == "Type";
                Expression<Func<TestClass, bool>> second = y => y.Title == "Title";
                Expression<Func<TestClass, bool>> expectedResult = x => x.Type == "Type" && x.Title == "Title";
                var result = firsts.And(second);
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
                Expression<Func<TestClass, bool>> firsts = x => x.Type == "Type";
                Expression<Func<TestClass, bool>> second = y => y.Title == "Title";
                Expression<Func<TestClass, bool>> expectedResult = x => x.Type == "Type" || x.Title == "Title";
                var result = firsts.Or(second);
                Assert.Equal(expectedResult.ToString(), result.ToString());
            }

            public class TestClass
            {
                public int Id { get; set; }
                public string Type { get; set; }
                public string Title { get; set; }
            }
        }

        public class RemapTo
        {
            [Fact]
            public void ToStringShouldMatchExpectation()
            {
                Expression<Func<Entity, DateTime>> expression = x => x.CreatedDate;
                Expression<Func<EntityWrapper<Entity>, DateTime>> expectedResult = x => x.Entity.CreatedDate;
                var result = expression.RemapTo<EntityWrapper<Entity>, Entity, DateTime>(x => x.Entity);
                Assert.Equal(expectedResult.ToString(), result.ToString());
            }

            [Fact]
            public void UsingGetValueOrDefaultOnNullableTypesShouldWork()
            {
                Expression<Func<Entity, bool>> expression = x => x.ModifiedDate.GetValueOrDefault(DateTime.MinValue) >= DateTime.UtcNow;
                var result = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(x => x.Entity);
            }

            [Fact]
            public void UsingGetValueOrDefaultOnNullableTypesShouldReturnExpectedResult()
            {
                var now = DateTime.UtcNow;
                Expression<Func<Entity, bool>> expression = x => x.ModifiedDate.GetValueOrDefault(DateTime.MinValue) >= now.AddDays(-7);
                var predicate = expression.RemapTo<EntityWrapper<Entity>, Entity, bool>(x => x.Entity);
                var list = new List<EntityWrapper<Entity>>
                {
                    new EntityWrapper<Entity>(new Entity{ Title = "ShouldBeFound", ModifiedDate = now.AddDays(-1) }),
                    new EntityWrapper<Entity>(new Entity{ Title = "ShouldNotBeFound", ModifiedDate = null }),
                };
                var result = list.AsQueryable().Where(predicate);
                Assert.Contains(result, x => x.Entity.Title == "ShouldBeFound");
                Assert.DoesNotContain(result, x => x.Entity.Title == "ShouldNotBeFound");
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
                Expression<Func<IEntity, bool>> expression = x => x.Id == 1;
                var predicate = expression.RemapTo<EntityWrapper<IEntity>, IEntity, bool>(x => x.Entity);
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

                public Expression<Func<T, bool>> Filter() => x => x.CreatedDate < _maxDateToInclude;
            }
        }
    }
}
