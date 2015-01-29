using System;
using System.Linq;
using Moq;
using Xunit;

namespace Spritely.Redo.Test
{
    // Most methods in this class have a near exact equivalent in TryFunctionTest
    public class TryActionTest
    {
        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.Running(null));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryAction methods call the same underlying functionality.
        [Fact]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryAction = Try.Running(() => { });

            Assert.Equal(Run.Until, tryAction.until);
        }

        [Fact]
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

            Assert.True(fCalled);
            Assert.True(satisfiedCalled);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }

        [Fact]
        public void Now_calls_until_with_expected_parameters()
        {
            var fCalled = false;
            var satisfiedCallResult = false;
            var expectedConfiguration = new TryConfiguration();

            TryConfiguration actualConfiguration = null;

            var tryAction = Try.Running(() => { fCalled = true; });
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                f();
                satisfiedCallResult = satisfied(null);
                actualConfiguration = configuration;
                return null;
            };

            tryAction.Now();

            Assert.True(fCalled);
            Assert.True(satisfiedCallResult);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }

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

            Assert.True(tryAction.configuration.Handles.Contains(typeof (TestException)));
        }

        private class TestException : Exception
        {
        }
    }
}
