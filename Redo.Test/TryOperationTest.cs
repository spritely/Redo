// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryConfigurableTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using System;
    using System.Linq;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TryOperationTest
    {
        [Test]
        public void With_sets_configuration_RetryStrategy()
        {
            var tryAction = Try.Running(() => { });

            var configuration = new TryConfiguration();
            var retryStrategy = new Mock<IRetryStrategy>();
            tryAction.configuration = configuration;

            tryAction.With(retryStrategy.Object);

            Assert.That(configuration.RetryStrategy, Is.SameAs(retryStrategy.Object));
        }

        [Test]
        public void Report_adds_value_to_configuration_ExceptionListeners()
        {
            var tryAction = Try.Running(() => { });
            var expectedException = new Exception();
            Exception actualException = null;

            tryAction.Report(ex => actualException = ex);

            tryAction.configuration.ExceptionListeners(expectedException);

            Assert.That(actualException, Is.SameAs(expectedException));
        }

        [Test]
        public void Handle_adds_exception_to_configuration_Handles()
        {
            var tryAction = Try.Running(() => { });

            tryAction.Handle<TestException>();

            Assert.That(tryAction.configuration.Handles.Contains(typeof(TestException)), Is.True);
        }

        [Serializable]
        private class TestException : Exception
        {
        }
    }
}
