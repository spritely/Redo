// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryFunctionAsyncTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using System;
    using NUnit.Framework;

    // Most methods in this class have a near exact equivalent in TryFunctionTest
    [TestFixture]
    public class TryFunctionAsyncTest
    {
        [Test]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.RunningAsync(null as Func<object>));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Test]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryFunction = Try.RunningAsync(() => true);

            Assert.That(tryFunction.until == Run.Until);
        }

        [Test]
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

            Assert.That(actualResult.Result, Is.SameAs(expectedResult));
            Assert.That(actualSatisfied, Is.SameAs(expectedSatisfied));
            Assert.That(actualConfiguration, Is.SameAs(expectedConfiguration));
        }

        [Test]
        public void UntilNotNull_calls_until_with_expected_parameters()
        {
            var expectedResult = new object();
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

            var actualResult = tryAction.UntilNotNull();

            actualResult.Wait();

            Assert.That(actualResult.Result, Is.SameAs(expectedResult));
            Assert.That(actualConfiguration, Is.SameAs(expectedConfiguration));
            Assert.That(actualSatisfied(null), Is.False);
            Assert.That(actualSatisfied(new object()), Is.True);
            Assert.That(actualSatisfied(1), Is.True);
        }
    }
}
