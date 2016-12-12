﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryActionAsyncTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class TryActionAsyncTest
    {
        [Test]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.RunningAsync(null));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Test]
        public void until_defaults_to_UntilExtension_UntilAsync()
        {
            var tryAction = Try.RunningAsync(() => Task.Run(() => { }));

            Assert.That(tryAction.until == Run.UntilAsync);
        }

        [Test]
        public async Task Until_calls_until_with_expected_parameters()
        {
            var fCalled = false;
            var satisfiedCalled = false;
            var expectedConfiguration = new TryConfiguration();

            TryConfiguration actualConfiguration = null;

            var tryAction = Try.RunningAsync(() => Task.Run(() => { fCalled = true; }));
            tryAction.configuration = expectedConfiguration;
            tryAction.until = async (f, satisfied, configuration) =>
            {
                await f();
                satisfied(null);
                actualConfiguration = configuration;
                return null;
            };

            await tryAction.Until(() =>
            {
                satisfiedCalled = true;
                return true;
            });

            Assert.That(fCalled, Is.True);
            Assert.That(satisfiedCalled, Is.True);
            Assert.That(actualConfiguration, Is.SameAs(expectedConfiguration));
        }
    }
}
