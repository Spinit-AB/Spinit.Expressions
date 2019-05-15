using System;
using Spinit.Expressions.UnitTests.Infrastructure;
using Xunit;

namespace Spinit.Expressions.UnitTests.PredicateGenerator.SimpleEnumerablePropertyTypePredicateHandler
{
    public class CanHandle
    {
        [Theory]
        [MemberData(nameof(TypeUtilities.GetPropertyNames), typeof(FilterWithSimpleEnumerableProps), MemberType = typeof(TypeUtilities))]
        public void AssertHandleable(string propertyName)
        {
            var handler = new SimpleEnumerablePropertyTypePredicateHandler<FilterWithSimpleEnumerableProps, Entity>();
            var property = typeof(FilterWithSimpleEnumerableProps).GetProperty(propertyName);
            var result = handler.CanHandle(property);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(TypeUtilities.GetPropertyNames), typeof(FilterWithSimpleEnumerableProps), MemberType = typeof(TypeUtilities))]
        public void AssertHandleableWhenNull(string propertyName)
        {
            var handler = new SimpleEnumerablePropertyTypePredicateHandler<FilterWithSimpleEnumerableProps, Entity>();
            var property = typeof(FilterWithSimpleEnumerableProps).GetProperty(propertyName);
            var result = handler.CanHandle(property);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(TypeUtilities.GetPropertyNames), typeof(FilterWithNonSimpleEnumerableProps), MemberType = typeof(TypeUtilities))]
        public void AssertNonHandleable(string propertyName)
        {
            var handler = new SimpleEnumerablePropertyTypePredicateHandler<FilterWithNonSimpleEnumerableProps, Entity>();
            var property = typeof(FilterWithNonSimpleEnumerableProps).GetProperty(propertyName);
            var result = handler.CanHandle(property);
            Assert.False(result);
        }
    }

    public class Handle
    {
        [Theory]
        [MemberData(nameof(TypeUtilities.GetPropertyNames), typeof(FilterWithSimpleEnumerableProps), MemberType = typeof(TypeUtilities))]
        public void HandleShouldReturnNullForEmptyProperties(string propertyName)
        {
            var handler = new SimpleEnumerablePropertyTypePredicateHandler<FilterWithSimpleEnumerableProps, Entity>();
            var property = typeof(FilterWithSimpleEnumerableProps).GetProperty(propertyName);
            var filter = new FilterWithSimpleEnumerableProps();
            var result = handler.Handle(filter, property);
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(TypeUtilities.GetPropertyNames), typeof(FilterWithSimpleEnumerableProps), MemberType = typeof(TypeUtilities))]
        public void HandleShouldReturnExpectedResult(string propertyName)
        {
            var handler = new SimpleEnumerablePropertyTypePredicateHandler<FilterWithSimpleEnumerableProps, Entity>();
            var property = typeof(FilterWithSimpleEnumerableProps).GetProperty(propertyName);
            var filter = new FilterWithSimpleEnumerableProps();
            var underlyingType = property.PropertyType.GetEnumerableUnderlyingType();
            var array = Array.CreateInstance(underlyingType, 1);
            array.Initialize();
            property.SetValue(filter, array);
            var result = handler.Handle(filter, property);
            Assert.Matches($@"x => value\(.*\[\]\).Contains\(x\.{property.Name}\)", result.ToString());
        }
    }    

    public class FilterWithSimpleEnumerableProps
    {
        public bool[] BoolProp { get; set; }
        public byte[] ByteProp { get; set; }
        public sbyte[] SByteProp { get; set; }
        public char[] CharProp { get; set; }
        public decimal[] DecimalProp { get; set; }
        public double[] DoubleProp { get; set; }
        public float[] FloatProp { get; set; }
        public int[] IntProp { get; set; }
        public uint[] UIntProp { get; set; }
        public long[] LongProp { get; set; }
        public ulong[] ULongProp { get; set; }
        public short[] ShortProp { get; set; }
        public ushort[] UShortProp { get; set; }
        public string[] StringProp { get; set; }
        public StringComparison[] EnumProp { get; set; }
        public DateTime[] DateTimeProp { get; set; }
        public TimeSpan[] TimeSpanProp { get; set; }
        public Guid[] GuidProp { get; set; }
    }

    public class FilterWithNonSimpleEnumerableProps
    {
        public dynamic[] DynamicProp { get; set; }
        public Tuple<string, string>[] TupleProp { get; set; }
        public Entity[] EntityProp { get; set; }
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
        public StringComparison EnumProp { get; set; }
        public DateTime DateTimeProp { get; set; }
        public TimeSpan TimeSpanProp { get; set; }
        public Guid GuidProp { get; set; }
    }
}
