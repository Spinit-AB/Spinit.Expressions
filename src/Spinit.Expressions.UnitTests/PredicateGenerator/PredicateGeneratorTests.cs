using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Spinit.Expressions.UnitTests
{
    public class PredicateGeneratorTests
    {
        [Fact]
        public void TestIdPredicate()
        {
            var filter = new TodoFilter { Id = 123 };
            var predicateGenerator = new PredicateGenerator<TodoFilter, Todo>();
            var predicate = predicateGenerator.Generate(filter);
            //var expectedPredicate = ExpressionExtensions.Expression(x => x.Id == 123);
            Expression<Func<Todo, bool>> expectedPredicate = x => x.Id == 123;
            Assert.Equal(expectedPredicate.ToString(), predicate.ToString());
        }

        [Fact]
        public void TestTypePredicate()
        {
            var filter = new TodoFilter { Type = new[] { "a", "b" } };
            var predicateGenerator = new PredicateGenerator<TodoFilter, Todo>();
            var predicate = predicateGenerator.Generate(filter);

            var items = new List<Todo>
            {
                new Todo { Type = "a" },
                new Todo { Type = "b" },
                new Todo { Type = "c" },
            };
            var filteredItems = items.AsQueryable().Where(predicate).ToList();
            Assert.DoesNotContain(filteredItems, x => x.Type == "c");
        }

        //[Fact]
        //public void TestTagsPredicate()
        //{
        //    var filter = new TodoFilter { Tags = new[] { "a", "b" } };
        //    var predicateGenerator = new PredicateGenerator<TodoFilter, Todo>();
        //    var predicate = predicateGenerator.Generate(filter);

        //    var items = new List<Todo>
        //    {
        //        new Todo {Tags = new [] { "a" } },
        //        new Todo {Tags = new [] { "b" } },
        //        new Todo {Tags = new [] { "c" }  },
        //    };
        //    var filteredItems = items.AsQueryable().Where(predicate).ToList();
        //    Assert.DoesNotContain(filteredItems, x => x.Tags.Contains("c"));
        //}

        public class TodoFilter
        {
            public int? Id { get; set; }

            public IEnumerable<string> Type { get; set; }

            // TODO: add OperatorAttribute (AND | OR) + implement IEnumerable support on target side
            //public IEnumerable<string> Tags { get; set; }
        }

        public class Todo
        {
            public int Id { get; set; }
            public string Type { get; set; }
            //public IEnumerable<string> Tags { get; set; }
        }
    }
}
