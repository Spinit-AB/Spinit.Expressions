using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Xunit;

namespace Spinit.Expressions.UnitTests.Internals.TypeExtensions
{
    public class IsSimple
    {
        [Theory]
        [MemberData(nameof(GetIsSimpleTheories))]
        public void ShouldBeSimple(Type type)
        {
            Assert.True(type.IsSimple());
        }

        [Theory]
        [MemberData(nameof(GetIsNotSimpleTheories))]
        public void ShouldNotBeSimple(Type type)
        {
            Assert.False(type.IsSimple());
        }

        public static TheoryData<Type> GetIsSimpleTheories()
        {
            var result = new TheoryData<Type>();
            foreach (var type in GetSimpleTypes())
            {
                result.Add(type);
            }
            return result;
        }

        public static TheoryData<Type> GetIsNotSimpleTheories()
        {
            var result = new TheoryData<Type>();
            foreach (var type in GetNoneSimpleTypes())
            {
                result.Add(type);
            }
            return result;
        }

        public static Type[] GetSimpleTypes()
        {
            return new[]
            {
                    typeof(bool),
                    typeof(byte),
                    typeof(sbyte),
                    typeof(char),
                    typeof(decimal),
                    typeof(double),
                    typeof(float),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(short),
                    typeof(ushort),
                    typeof(string),
                    typeof(StringComparison), // enum
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid),

                    // nullables
                    typeof(bool?),
                    typeof(byte?),
                    typeof(sbyte?),
                    typeof(char?),
                    typeof(decimal?),
                    typeof(double?),
                    typeof(float?),
                    typeof(int?),
                    typeof(uint?),
                    typeof(long?),
                    typeof(ulong?),
                    typeof(short?),
                    typeof(ushort?),
                    typeof(StringComparison?), // enum
                    typeof(DateTime?),
                    typeof(DateTimeOffset?),
                    typeof(TimeSpan?),
                    typeof(Guid?)
                };
        }

        public static Type[] GetNoneSimpleTypes()
        {
            return new[]
            {
                    typeof(object),
                    typeof(StringBuilder),
                    typeof(Point)
                };
        }
    }

    public class IsNullable
    {
        [Fact]
        public void StringIsNotNullable()
        {
            Assert.False(typeof(string).IsNullable());
        }

        [Fact]
        public void IntIsNotNullable()
        {
            Assert.False(typeof(int).IsNullable());
        }

        [Fact]
        public void NullabeIntIsNullable()
        {
            Assert.True(typeof(int?).IsNullable());
            Assert.True(typeof(Nullable<int>).IsNullable());
        }

        [Fact]
        public void ClassDoesNotImplementNullable()
        {
            Assert.False(typeof(TestClass).IsNullable());
        }

#if false
            // only for C#8, nullable string
            [Fact]
            public void NullabestringIsNullable()
            {
                Assert.True(typeof(string?).IsNullable());
                Assert.True(typeof(Nullable<string>).IsNullable());
            }
#endif
        public class TestClass
        {
            public int IntProperty1 { get; set; }
        }
    }

    public class IsNullableOfT
    {
        [Fact]
        public void NullableIntIsNotNullableOfBool()
        {
            Assert.False(typeof(int?).IsNullableOf<bool>());
        }

        [Fact]
        public void NullableIntIsNullableOfInt()
        {
            Assert.True(typeof(int?).IsNullableOf<int>());
        }
    }

    public class IsNullableOf
    {
        [Fact]
        public void NullableIntIsNullableOfPrimitiveType()
        {
            Assert.True(typeof(int?).IsNullableOf(x => x.IsPrimitive));
        }

        [Fact]
        public void NullableIntIsNullableOfInt()
        {
            Assert.True(typeof(int?).IsNullableOf(x => x == typeof(int)));
        }
    }

    public class IsEnumerable
    {
        [Fact]
        public void StringIsAnEnumerable()
        {
            Assert.True(typeof(string).IsEnumerable());
        }

        [Fact]
        public void ICollectionOfStringIsAnEnumerable()
        {
            Assert.True(typeof(ICollection<string>).IsEnumerable());
        }

        [Fact]
        public void ListOfStringIsAnEnumerable()
        {
            Assert.True(typeof(List<string>).IsEnumerable());
        }

        [Fact]
        public void BoolArrayIsAnEnumerable()
        {
            Assert.True(typeof(bool[]).IsEnumerable());
        }
    }

    public class IsEnumerableOfT
    {
        [Fact]
        public void StringIsAnEnumerableOfChar()
        {
            Assert.True(typeof(string).IsEnumerableOf<char>());
        }

        [Fact]
        public void ICollectionOfStringIsAnEnumerableOfString()
        {
            Assert.True(typeof(ICollection<string>).IsEnumerableOf<string>());
        }

        [Fact]
        public void ListOfStringIsAnEnumerableOfString()
        {
            Assert.True(typeof(List<string>).IsEnumerableOf<string>());
        }
    }

    public class IsEnumerableOf
    {
        [Fact]
        public void IntArrayIsEnumerableOfPrimitiveType()
        {
            Assert.True(typeof(int[]).IsEnumerableOf(x => x.IsPrimitive));
        }

        [Fact]
        public void StringListIsEnumerableOfString()
        {
            Assert.True(typeof(List<string>).IsEnumerableOf(x => x == typeof(string)));
        }
    }

    public class IsEquatable
    {
        [Theory]
        [MemberData(nameof(GetTheories))]
        public void ShouldReturnExpectedValue(Type type, bool expectedValue)
        {
            Assert.Equal(expectedValue, type.IsEquatable());
        }

        public static TheoryData<Type, bool> GetTheories()
        {
            var result = new TheoryData<Type, bool>();
            foreach (var type in GetEquatableTypes())
            {
                result.Add(type, true);
            }
            foreach (var type in GetNoneEquatableTypes())
            {
                result.Add(type, false);
            }
            return result;
        }

        public static Type[] GetEquatableTypes()
        {
            return new[]
            {
                    typeof(bool),
                    typeof(byte),
                    typeof(sbyte),
                    typeof(char),
                    typeof(decimal),
                    typeof(double),
                    typeof(float),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(short),
                    typeof(ushort),
                    typeof(string),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid),
                    typeof(Point),
                };
        }

        public static Type[] GetNoneEquatableTypes()
        {
            return new[]
            {
                    typeof(object),
                    typeof(StringComparison), // enum
                    typeof(StringBuilder),

                    // nullables
                    typeof(bool?),
                    typeof(byte?),
                    typeof(sbyte?),
                    typeof(char?),
                    typeof(decimal?),
                    typeof(double?),
                    typeof(float?),
                    typeof(int?),
                    typeof(uint?),
                    typeof(long?),
                    typeof(ulong?),
                    typeof(short?),
                    typeof(ushort?),
                    typeof(StringComparison?), // enum
                    typeof(DateTime?),
                    typeof(DateTimeOffset?),
                    typeof(TimeSpan?),
                    typeof(Guid?)
                };
        }
    }

    public class GetEnumerableUnderlyingType
    {
        [Theory]
        [MemberData(nameof(GetEnumerableTheories))]
        public void ShouldReturnExpectedType(Type enumerableType, Type expectedResult)
        {
            var result = enumerableType.GetEnumerableUnderlyingType();
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData(nameof(GetNonEnumerableTheories))]
        public void ShouldReturnNullForNonEnumerables(Type nonEnumerableType)
        {
            var result = nonEnumerableType.GetEnumerableUnderlyingType();
            Assert.Null(result);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionIfTypeImplementsMultipleIEnumerable()
        {
            Assert.Throws<ArgumentException>(() => typeof(ClassWithMultipleIEnumerable).GetEnumerableUnderlyingType());
        }

        public class ClassWithMultipleIEnumerable : IEnumerable<string>, IEnumerable<bool>
        {
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<bool> IEnumerable<bool>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class NonEnumerableClass
        {
            public int IntProp { get; set; }
        }

        public static TheoryData<Type, Type> GetEnumerableTheories()
        {
            return new TheoryData<Type, Type>
            {
                { typeof(IEnumerable<string>), typeof(string) },
                { typeof(int[]), typeof(int) },
                { typeof(Collection<DateTime>), typeof(DateTime) },
                { typeof(List<bool>), typeof(bool) },
                { typeof(string), typeof(char) }
            };
        }

        public static TheoryData<Type> GetNonEnumerableTheories()
        {
            return new TheoryData<Type>
            {
                { typeof(int) },
                { typeof(DateTime) },
                { typeof(bool) },
                { typeof(NonEnumerableClass) },
            };
        }
    }
}
