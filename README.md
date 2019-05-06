Spinit.Expressions
========================================

![Licence: MIT](https://img.shields.io/github/license/Spinit-AB/Spinit.Expressions.svg)
![Azure DevOps builds](https://img.shields.io/azure-devops/build/spinitforce/d7ddce33-e90d-4c48-9976-24d1676759e2/8/master.svg)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/spinitforce/d7ddce33-e90d-4c48-9976-24d1676759e2/8.svg?compact_message)
![Nuget](https://img.shields.io/nuget/v/Spinit.Expressions.svg)


Contains .NET Expression Helpers

Install
-------

Package Manager:

    PM> Install-Package Spinit.Expressions

.NET CLI:

    > dotnet add package Spinit.Expressions

Usage
-----

### Combining expressions

#### And

Combines two expressions using AndAlso (&&) and uses parameters from the first expression

Example:

    Expression<Func<int, bool>> first = x => x > 10;
    Expression<Func<int, bool>> second = y => y < 20;
    var result = first.And(second); // combines the expressions using AndAlso
    // result expression: x => x > 10 && x < 20

#### Or

Combines two expressions using OrElse (||) and uses parameters from the first expression

Example:

    Expression<Func<int, bool>> first = x => x >= 10;
    Expression<Func<int, bool>> second = y => y <= -10;
    var result = first.And(second); // combines the expressions using OrElse
    // result expression: x => x >= 10 || x <= -10

### Remapping expression

RemapTo is used to remap an expression on a type to another type that have the original type as a property.

Example:

    public class MyClass 
    {
        public string Id { get; set; }
    }

    ...

    Expression<Func<string, bool>> stringExpression = a => a == "123";
    var myClassExpression = stringExpression.RemapTo<MyClass, string, bool>(myClass => myClass.Id); // provide a path
    // myClassExpression: myClass => myClass.Id == "123"

### Predicate generator

This is a utility class for generating an expression on a class given another "filter" class.

Example scenario: You have a webapi using some ORM (NHibernate, EntityFramework, Spinit.CosmosDb) and want to allow api users to supply a filter for your data.

Metacode for this scenario:

    public class MyEntity
    {
        public string Id { get; set; }
        ... 
        public MyStatusEnum Status { get; set; }
        public string Category { get; set; }
        ...
    }

    public class MyEntityFilter 
    {
        public MyStatusEnum Status { get; set; }
        public IEnumerable<string> Category { get; set; }
    }

    public MyApiController : BaseController
    {
        private readonly IQueryable<MyEntity> _entities;

        // WebApi action method
        public IEnumerable<MyEntity> Get(MyEntityFilter filter)
        {
            var predicate = new PredicateBuilder<MyEntityFilter, MyEntity>().Generate(filter);
            // if filter.Status and filter.Category is set the resulting expression looks like:
            // x => x.Status == filter.Status && filter.Category.Contains(x.Category)
            return _entities.Where(predicate);
        }
    }
