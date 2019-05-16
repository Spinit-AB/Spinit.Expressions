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

```console
PM> Install-Package Spinit.Expressions
```

.NET CLI:
```console
> dotnet add package Spinit.Expressions
```

Usage
-----

### Combining predicates

#### And

Combines two predicates using AndAlso (&&) and uses parameters from the first expression.

Example:

```csharp
Expression<Func<int, bool>> first = x => x > 10;
Expression<Func<int, bool>> second = y => y < 20;
var result = first.And(second); // combines the expressions using AndAlso
// result expression: (int x) => x > 10 && x < 20
```

#### Or

Combines two predicates using OrElse (||) and uses parameters from the first expression.

Example:

```csharp
Expression<Func<int, bool>> first = x => x >= 10;
Expression<Func<int, bool>> second = y => y <= -10;
var result = first.Or(second); // combines the expressions using OrElse
// result expression: (int x) => x >= 10 || x <= -10
```

#### Combine

Combines multiple predicates using the supplied operator and uses parameters from the first expression.

Example:

```csharp
var predicates = new List<Expression<Func<string, bool>>>
{
    x => x.Contains("a"),
    y => y.Contains("b"),
    z => z.Contains("c")
};
var result = predicates.Combine(CombineOperator.And);
// result expression (string x) => x.Contains("a") && x.Contains("b") && x.Contains("c")
```

### Remapping expression

`RemapTo` is used to remap an expression on a type to another type, given a selector that is of the original type.

Example:

```csharp
public class MyClass 
{
    public string Id { get; set; }
}

...

Expression<Func<string, bool>> stringExpression = s => s == "123";
var myClassExpression = stringExpression.RemapTo((MyClass x) => x.Id); // provide a path
// myClassExpression: (MyClass x) => x.Id == "123"
```

### Predicate generator

This is a utility class for generating an expression on a class given another "filter" class.

Example scenario: You have a webapi using some ORM (NHibernate, EntityFramework, Spinit.CosmosDb) and want to allow api users to supply a filter for your data.

Metacode for this scenario:

```csharp
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
    [TargetPropertyName(nameof(MyEntityFilter.Status))] // This attribute is optional but recommended
    public MyStatusEnum? Status { get; set; }
    [TargetPropertyName(nameof(MyEntityFilter.Category))]
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
```

Out of the box the `PredicateGenerator` handles "simple types", eg types that contains a single value. You can supply your own `IPropertyPredicateHandler` and add it using `PredicateGenerator.AddHandler`.

Conventions: 
 * If no `TargetPropertyName` attribute exists the property name of the filter class and the entity must match.
 * When using an enumerable on the filter class `Contains` is used, eg it should match any of the supplied values (OR).
 * If the filter value is null (or an empty enumerable) no predicate is applied for the current property.


### Predicate

Utility class for defining a predicate expression e.g. `Expression<Func<SomeType, bool>>`

Example:

```csharp
// instead of
Expression<Func<string, bool>> predicate = s => s == "123";
// you can use
var predicate = Predicate.Of<string>(s => s == "123");
```

### Type expressions

Utility class for expressions from types.

#### GetPropertyExpression

Returns an expression that points to the specified property.

Example for sorting a collection on a dynamic property:

```csharp
public class MyEntity
{
    public string Name { get; set; }
}

public MyApiController : BaseController
{
    private readonly IQueryable<MyEntity> _entities;

    // WebApi action method
    public IEnumerable<MyEntity> Get(string orderBy)
    {
        var orderByExpression = TypeExpressions.GetPropertyExpression<MyEntity>(orderBy);
        // if orderBy == "Name" orderByExpression is: x => x.Name
        return _entities.OrderBy(orderByExpression);
    }
}
```