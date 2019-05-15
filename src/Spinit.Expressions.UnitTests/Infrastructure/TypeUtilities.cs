using System;
using Xunit;

namespace Spinit.Expressions.UnitTests.Infrastructure
{
    public static class TypeUtilities
    {
        public static TheoryData<string> GetPropertyNames(Type type)
        {
            var result = new TheoryData<string>();
            foreach (var property in type.GetProperties())
            {
                result.Add(property.Name);
            }
            return result;
        }
    }
}
