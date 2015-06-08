// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryActionTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TryActionTest
    {
        [Test]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.Running(null));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Test]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryAction = Try.Running(() => { });

            Assert.That(tryAction.until == Run.Until);
        }

        [Test]
        public void Until_calls_until_with_expected_parameters()
        {
            var fCalled = false;
            var satisfiedCalled = false;
            var expectedConfiguration = new TryConfiguration();

            TryConfiguration actualConfiguration = null;

            var tryAction = Try.Running(() => { fCalled = true; });
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                f();
                satisfied(null);
                actualConfiguration = configuration;
                return null;
            };

            tryAction.Until(() =>
            {
                satisfiedCalled = true;
                return true;
            });

            Assert.That(fCalled, Is.True);
            Assert.That(satisfiedCalled, Is.True);
            Assert.That(actualConfiguration, Is.EqualTo(expectedConfiguration));
        }
    }
}
