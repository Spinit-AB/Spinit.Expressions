using System;
using System.Collections.Generic;
using Xunit;

namespace Spinit.Expressions.UnitTests
{
    public class TypeExtensionsTests
    {
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
    }
}
