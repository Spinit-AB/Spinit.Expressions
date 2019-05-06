﻿using System;
using Xunit;

namespace Spinit.Expressions.UnitTests.PredicateGenerator
{
    public class SimplePropertyTypePredicateHandlerTests
    {
        public class CanHandle
        {
            [Theory]
            [MemberData(nameof(GetPropertyNames), typeof(FilterWithSimpleProps), MemberType = typeof(SimplePropertyTypePredicateHandlerTests))]
            public void AssertHandleable(string propertyName)
            {
                var handler = new SimplePropertyTypePredicateHandler<FilterWithSimpleProps, Entity>();
                var property = typeof(FilterWithSimpleProps).GetProperty(propertyName);
                var result = handler.CanHandle(property);
                Assert.True(result);
            }

            [Theory]
            [MemberData(nameof(GetPropertyNames), typeof(FilterWithNullableSimpleProps), MemberType = typeof(SimplePropertyTypePredicateHandlerTests))]
            public void AssertHandleableWhenNullable(string propertyName)
            {
                var handler = new SimplePropertyTypePredicateHandler<FilterWithNullableSimpleProps, Entity>();
                var property = typeof(FilterWithNullableSimpleProps).GetProperty(propertyName);
                var result = handler.CanHandle(property);
                Assert.True(result);
            }

            [Theory]
            [MemberData(nameof(GetPropertyNames), typeof(FilterWithNonSimpleProps), MemberType = typeof(SimplePropertyTypePredicateHandlerTests))]
            public void AssertNonHandleable(string propertyName)
            {
                var handler = new SimplePropertyTypePredicateHandler<FilterWithNonSimpleProps, Entity>();
                var property = typeof(FilterWithNonSimpleProps).GetProperty(propertyName);
                var result = handler.CanHandle(property);
                Assert.False(result);
            }
        }

        public class Handle
        {
            [Theory]
            [MemberData(nameof(GetPropertyNames), typeof(FilterWithNullableSimpleProps), MemberType = typeof(SimplePropertyTypePredicateHandlerTests))]
            public void HandleShouldReturnNullForEmptyProperties(string propertyName)
            {
                var handler = new SimplePropertyTypePredicateHandler<FilterWithNullableSimpleProps, Entity>();
                var property = typeof(FilterWithNullableSimpleProps).GetProperty(propertyName);
                var filter = new FilterWithNullableSimpleProps();
                var result = handler.Handle(filter, property);
                Assert.Null(result);
            }

            [Theory]
            [MemberData(nameof(GetPropertyNames), typeof(FilterWithSimpleProps), MemberType = typeof(SimplePropertyTypePredicateHandlerTests))]
            public void HandleShouldReturnExpectedResult(string propertyName)
            {
                var handler = new SimplePropertyTypePredicateHandler<FilterWithSimpleProps, Entity>();
                var property = typeof(FilterWithSimpleProps).GetProperty(propertyName);
                var filter = new FilterWithSimpleProps
                {
                    StringProp = "" // needs to be set, since default == null
                };
                var result = handler.Handle(filter, property);
                Assert.Matches($@"x => \(x\.{property.Name} == .+\)", result.ToString());
            }
        }

        public static TheoryData<string> GetPropertyNames(Type type)
        {
            var result = new TheoryData<string>();
            foreach (var property in type.GetProperties())
            {
                result.Add(property.Name);
            }
            return result;
        }

        public class FilterWithSimpleProps
        {
            public bool BoolProp { get; set; }
            public byte ByteProp { get; set; }
            public sbyte SByteProp { get; set; }
            public char CharProp { get; set; }
            public decimal DecimalProp { get; set; }
            public double DoubleProp { get; set; }
            public float FloatProp { get; set; }
            public int IntProp { get; set; }
            public uint UIntProp { get; set; }
            public long LongProp { get; set; }
            public ulong ULongProp { get; set; }
            public short ShortProp { get; set; }
            public ushort UShortProp { get; set; }
            public string StringProp { get; set; }
            public MyEnum EnumProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public TimeSpan TimeSpanProp { get; set; }
            public Guid GuidProp { get; set; }
        }

        public class FilterWithNullableSimpleProps
        {
            public bool? BoolProp { get; set; }
            public byte? ByteProp { get; set; }
            public sbyte? SByteProp { get; set; }
            public char? CharProp { get; set; }
            public decimal? DecimalProp { get; set; }
            public double? DoubleProp { get; set; }
            public float? FloatProp { get; set; }
            public int? IntProp { get; set; }
            public uint? UIntProp { get; set; }
            public long? LongProp { get; set; }
            public ulong? ULongProp { get; set; }
            public short? ShortProp { get; set; }
            public ushort? UShortProp { get; set; }
            public string StringProp { get; set; }
            public MyEnum? EnumProp { get; set; }
            public DateTime? DateTimeProp { get; set; }
            public TimeSpan? TimeSpanProp { get; set; }
            public Guid? GuidProp { get; set; }
        }

        public class FilterWithNonSimpleProps
        {
            public dynamic DynamicProp { get; set; }
            public Tuple<string, string> TupleProp { get; set; }
            public Entity EntityProp { get; set; }
        }

        public enum MyEnum
        {
            Value1,
            Value2
        }

        public class Entity
        {
            public bool BoolProp { get; set; }
            public byte ByteProp { get; set; }
            public sbyte SByteProp { get; set; }
            public char CharProp { get; set; }
            public decimal DecimalProp { get; set; }
            public double DoubleProp { get; set; }
            public float FloatProp { get; set; }
            public int IntProp { get; set; }
            public uint UIntProp { get; set; }
            public long LongProp { get; set; }
            public ulong ULongProp { get; set; }
            public short ShortProp { get; set; }
            public ushort UShortProp { get; set; }
            public string StringProp { get; set; }
            public MyEnum EnumProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public TimeSpan TimeSpanProp { get; set; }
            public Guid GuidProp { get; set; }
        }
    }
}
