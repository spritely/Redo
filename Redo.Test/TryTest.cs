// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class TryTest
    {
        [Test]
        public void Running_returns_a_TryAction_when_an_action_is_passed()
        {
            var actual = Try.Running(() => { });

            Assert.That(actual, Is.InstanceOf<TryAction>());
        }

        [Test]
        public void Running_returns_a_TryFunction_when_a_function_is_passed()
        {
            var actual = Try.Running(() => new object());

            Assert.That(actual, Is.InstanceOf<TryFunction<object>>());
        }
    }
}
