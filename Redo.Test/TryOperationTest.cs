// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryConfigurableTest.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Moq;
using Xunit;

namespace Spritely.Redo.Test
{
    public class TryOperationTest
    {
        [Fact]
        public void With_sets_configuration_RetryStrategy()
        {
            var tryAction = Try.Running(() => { });

            var configuration = new TryConfiguration();
            var retryStrategy = new Mock<IRetryStrategy>();
            tryAction.configuration = configuration;

            tryAction.With(retryStrategy.Object);

            Assert.Same(retryStrategy.Object, configuration.RetryStrategy);
        }

        [Fact]
        public void Report_adds_value_to_configuration_ExceptionListeners()
        {
            var tryAction = Try.Running(() => { });
            var expectedException = new Exception();
            Exception actualException = null;

            tryAction.Report(ex => actualException = ex);

            tryAction.configuration.ExceptionListeners(expectedException);

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Handle_adds_exception_to_configuration_Handles()
        {
            var tryAction = Try.Running(() => { });

            tryAction.Handle<TestException>();

            Assert.True(tryAction.configuration.Handles.Contains(typeof(TestException)));
        }

        private class TestException : Exception
        {
        }
    }
}
