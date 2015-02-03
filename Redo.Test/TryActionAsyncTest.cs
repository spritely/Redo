// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryActionAsyncTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Xunit;

namespace Spritely.Redo.Test
{
    public class TryActionAsyncTest
    {
        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.RunningAsync(null));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Fact]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryAction = Try.RunningAsync(() => { });

            Assert.Equal(Run.Until, tryAction.until);
        }

        [Fact]
        public void Until_calls_until_with_expected_parameters()
        {
            var fCalled = false;
            var satisfiedCalled = false;
            var expectedConfiguration = new TryConfiguration();

            TryConfiguration actualConfiguration = null;

            var tryAction = Try.RunningAsync(() => { fCalled = true; });
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                f();
                satisfied(null);
                actualConfiguration = configuration;
                return null;
            };

            var until = tryAction.Until(() =>
            {
                satisfiedCalled = true;
                return true;
            });

            until.Wait();

            Assert.True(fCalled);
            Assert.True(satisfiedCalled);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }
    }
}
