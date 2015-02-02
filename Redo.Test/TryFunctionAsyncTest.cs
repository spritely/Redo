// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryFunctionAsyncTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Xunit;

namespace Spritely.Redo.Test
{
    // Most methods in this class have a near exact equivalent in TryFunctionTest
    public class TryFunctionAsyncTest
    {
        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.RunningAsync(null as Func<object>));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Fact]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryFunction = Try.RunningAsync(() => true);

            Assert.Equal(Run.Until, tryFunction.until);
        }

        [Fact]
        public void Until_calls_until_with_expected_parameters()
        {
            var expectedResult = new object();
            Func<object, bool> expectedSatisfied = _ => true;
            var expectedConfiguration = new TryConfiguration();

            Func<object, bool> actualSatisfied = null;
            TryConfiguration actualConfiguration = null;

            var tryAction = Try.RunningAsync(() => expectedResult);
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                actualSatisfied = satisfied;
                actualConfiguration = configuration;
                return f();
            };

            var actualResult = tryAction.Until(expectedSatisfied);
            actualResult.Wait();

            Assert.Same(expectedResult, actualResult.Result);
            Assert.Same(expectedSatisfied, actualSatisfied);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }

        [Fact]
        public void Now_calls_until_with_expected_parameters()
        {
            var expectedResult = new object();
            var satisfiedCallResult = false;
            var expectedConfiguration = new TryConfiguration();

            TryConfiguration actualConfiguration = null;

            var tryAction = Try.RunningAsync(() => expectedResult);
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                satisfiedCallResult = satisfied(null);
                actualConfiguration = configuration;
                return f();
            };

            var actualResult = tryAction.Now();
            actualResult.Wait();

            Assert.Same(expectedResult, actualResult.Result);
            Assert.True(satisfiedCallResult);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }
    }
}
