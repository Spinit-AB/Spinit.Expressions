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
