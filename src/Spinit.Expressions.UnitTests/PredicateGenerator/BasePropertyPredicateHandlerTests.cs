using System;
using Xunit;

namespace Spinit.Expressions.UnitTests.PredicateGenerator.BasePropertyPredicateHandler
{
    public class GetTargetProperty
    {
        [Fact]
        public void ShouldReturnByConvention()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyUsingConvention)));
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnExpectedValueByConvention()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyUsingConvention)));
            Assert.Equal(typeof(Target).GetProperty(nameof(Target.PropertyUsingConvention)), result);
        }

        [Fact]
        public void ShouldReturnByAttribute()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyUsingAttribute)));
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldReturnExpectedValueByAttribute()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyUsingAttribute)));
            Assert.Equal(typeof(Target).GetProperty(nameof(Target.PropertyUsingAttribute_Suffix)), result);
        }

        [Fact]
        public void ShouldReturnNullIfPropertyNotFoundByConvention()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyWithoutMatchingTarget)));
            Assert.Null(result);
        }

        [Fact]
        public void ShouldReturnNullIfPropertyNotFoundByAttribute()
        {
            var result = BasePropertyPredicateHandler<Source, Target>.GetTargetProperty(typeof(Source).GetProperty(nameof(Source.PropertyUsingAttributeWithoutMatchingTarget)));
            Assert.Null(result);
        }

        public class Source
        {
            public int PropertyUsingConvention { get; set; }
            [TargetPropertyName(nameof(Target.PropertyUsingAttribute_Suffix))]
            public DateTime PropertyUsingAttribute { get; set; }

            public string PropertyWithoutMatchingTarget { get; set; }

            [TargetPropertyName("ThereIsNoSpoon")]
            public bool PropertyUsingAttributeWithoutMatchingTarget { get; set; }
        }

        public class Target
        {
            public int PropertyUsingConvention { get; set; }
            public DateTime PropertyUsingAttribute_Suffix { get; set; }
        }
    }
}
