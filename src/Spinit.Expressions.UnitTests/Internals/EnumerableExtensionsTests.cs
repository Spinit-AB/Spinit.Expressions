using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spinit.Expressions.UnitTests.Infrastructure;
using Xunit;

namespace Spinit.Expressions.UnitTests.Internals
{
    public class EnumerableExtensionsTests
    {
        [Theory]
        [MemberData(nameof(GetScenarios))]
        public void IsEmptyShouldReturnExpectedValue(Scenario scenario)
        {
            var result = scenario.Enumerable.IsEmpty();
            Assert.Equal(scenario.ExpectedResult, result);
        }

        public static TheoryData<Scenario> GetScenarios()
        {
            return new TheoryData<Scenario>
            {
                new Scenario
                {
                    Enumerable = Array.Empty<bool>(),
                    ExpectedResult = true
                },
                new Scenario
                {
                    Enumerable = Array.Empty<string>(),
                    ExpectedResult = true
                },
                new Scenario
                {
                    Enumerable = new Collection<decimal>(),
                    ExpectedResult = true
                },
                new Scenario
                {
                    Enumerable = new List<int>(),
                    ExpectedResult = true
                },
                new Scenario
                {
                    Enumerable = new HashSet<int>(),
                    ExpectedResult = true
                },
                new Scenario
                {
                    Enumerable = new bool[] { false },
                    ExpectedResult = false
                },
                new Scenario
                {
                    Enumerable = new string[] { "" },
                    ExpectedResult = false
                },
                new Scenario
                {
                    Enumerable = new Collection<decimal>{ 1M },
                    ExpectedResult = false
                },
                new Scenario
                {
                    Enumerable = new List<int>{ 0, 1, 2 },
                    ExpectedResult = false
                },
                new Scenario
                {
                    Enumerable = new HashSet<int>{ 123 },
                    ExpectedResult = false
                }
            };
        }

        public class Scenario : XunitSerializable
        {
            public IEnumerable Enumerable { get; set; }
            public bool ExpectedResult { get; set; }
        }
    }
}
