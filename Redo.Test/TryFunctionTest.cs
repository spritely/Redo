using System;
using System.Linq;
using Moq;
using Xunit;

namespace Spritely.Redo.Test
{
    public class TryFunctionTest : IDisposable
    {
        public TryFunctionTest()
        {
            // Replace the default retry strategy with a test instance that doesn't delay and quits after 50 tries
            var retryStrategy = new Mock<IRetryStrategy>();

            // false, false, false...., true (when ran 50 times)
            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns<long>(attempt => attempt > 50);

            TryDefault.RetryStrategy = retryStrategy.Object;
        }

        public void Dispose()
        {
            TryDefault.Reset();
        }

        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.Running(null as Func<object>));
        }

        // TryFunctionTest validates shared functional paths between TryAction and TryFunction.
        // These next two methods ensure the TryFunction methods call the same underlying functionality.
        [Fact]
        public void until_defaults_to_UntilExtension_Until()
        {
            var tryFunction = Try.Running(() => true);

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

            var tryAction = Try.Running(() => expectedResult);
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                actualSatisfied = satisfied;
                actualConfiguration = configuration;
                return f();
            };

            var actualResult = tryAction.Until(expectedSatisfied);

            Assert.Same(expectedResult, actualResult);
            Assert.Same(expectedSatisfied, actualSatisfied);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }

        [Fact]
        public void Now_calls_until_with_expected_parameters()
        {
            var expectedResult = new object();
            var satisfiedCallResult = false;
            var expectedConfiguration = new TryConfiguration();

            Func<object, bool> actualSatisfied = null;
            TryConfiguration actualConfiguration = null;

            var tryAction = Try.Running(() => expectedResult);
            tryAction.configuration = expectedConfiguration;
            tryAction.until = (f, satisfied, configuration) =>
            {
                satisfiedCallResult = satisfied(null);
                actualConfiguration = configuration;
                return f();
            };

            var actualResult = tryAction.Now();

            Assert.Same(expectedResult, actualResult);
            Assert.True(satisfiedCallResult);
            Assert.Same(expectedConfiguration, actualConfiguration);
        }

        [Fact]
        public void With_sets_configuration_RetryStrategy()
        {
            var tryAction = Try.Running(() => true);

            var configuration = new TryConfiguration();
            var retryStrategy = new Mock<IRetryStrategy>();
            tryAction.configuration = configuration;

            tryAction.With(retryStrategy.Object);

            Assert.Same(retryStrategy.Object, configuration.RetryStrategy);
        }

        [Fact]
        public void Report_adds_value_to_configuration_ExceptionListeners()
        {
            var tryAction = Try.Running(() => true);
            var expectedException = new Exception();
            Exception actualException = null;

            tryAction.Report(ex => actualException = ex);

            tryAction.configuration.ExceptionListeners(expectedException);

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Handle_adds_exception_to_configuration_Handles()
        {
            var tryAction = Try.Running(() => true);

            tryAction.Handle<TestException1>();

            Assert.True(tryAction.configuration.Handles.Contains(typeof (TestException1)));
        }

        [Fact]
        public void Until_uses_default_retry_strategy()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            TryDefault.RetryStrategy = retryStrategy.Object;

            // On exception ShouldQuit() = false so code will reach Wait()
            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns(false);

            // Run twice to ensure Until() reaches Wait()
            var i = 0;
            Try.Running<object>(() => { throw new Exception(); })
                .Until(_ => i++ >= 1);

            retryStrategy.Verify(s => s.ShouldQuit(It.IsAny<long>()), Times.AtLeastOnce);
            retryStrategy.Verify(s => s.Wait(It.IsAny<long>()), Times.AtLeastOnce);
        }

        [Fact]
        public void With_sets_the_retry_strategy()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // On exception ShouldQuit() = false so code will reach Wait()
            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns(false);

            // Run twice to ensure Until() reaches Wait()
            var i = 0;
            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => i++ >= 1);

            retryStrategy.Verify(s => s.ShouldQuit(It.IsAny<long>()), Times.AtLeastOnce);
            retryStrategy.Verify(s => s.Wait(It.IsAny<long>()), Times.AtLeastOnce);
        }

        [Fact]
        public void RetryStrategy_Wait_is_called_with_current_1_based_attempt_value()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            var i = 0;

            retryStrategy.Setup(s => s.Wait(i + 1)).Verifiable();

            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => i++ >= 10);

            retryStrategy.Verify();
        }

        [Fact]
        public void Until_returns_result_of_successful_call()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            var expected = new object();
            var actual = Try.Running(() => expected)
                .With(retryStrategy.Object)
                .Until(_ => true);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Until_retries_until_Until_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            TryDefault.RetryStrategy = retryStrategy.Object;

            // false, false, false...., true
            var falseCount = new Random().Next(2, 10);
            var untilReturns = Enumerable.Range(0, falseCount).Select(i => false).Concat(new[] {true}).ToList();
            var calls = 0;

            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => untilReturns[calls++]);

            Assert.Equal(falseCount + 1, calls);
        }

        [Fact]
        public void Until_retries_until_ShouldQuit_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // false, false, false...., true
            var falseCount = new Random().Next(2, 10);
            var shouldQuitReturns =
                Enumerable.Range(0, falseCount).Select(i => false).Concat(new[] {true}).GetEnumerator();
            var calls = 0;
            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns<long>(attempt =>
            {
                calls++;
                return attempt > falseCount;
            });

            var times = 0;

            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(_ => times++ >= (falseCount + 2))); // No infinite loop on test failure

            Assert.Equal(falseCount + 1, calls);
        }

        [Fact]
        public void Wait_is_not_called_after_ShouldQuit_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns(true);

            var i = 0;
            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(_ => i++ >= 2)); // Do not return via Until on the first attempt

            retryStrategy.Verify(s => s.Wait(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public void Wait_is_not_called_when_Until_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => true);

            retryStrategy.Verify(s => s.Wait(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public void Until_rethrows_original_exception_when_ShouldQuit_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit(It.IsAny<long>())).Returns(true);

            var expectedException = new Exception();
            var i = 0;
            try
            {
                Try.Running<object>(() => { throw expectedException; })
                    .With(retryStrategy.Object)
                    .Until(_ => i++ >= 2); // No infinite loop on test failure
            }
            catch (Exception actualException)
            {
                Assert.Same(expectedException, actualException);
            }
        }

        [Fact]
        public void Until_reports_exceptions_to_default_delegates()
        {
            var expectedException = new Exception();
            Exception actualException = null;
            TryDefault.ExceptionListeners += ex => actualException = ex;

            Try.Running<object>(() => { throw expectedException; })
                .Until(_ => true);

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Until_reports_exceptions_to_call_specific_delegates()
        {
            var expectedException = new Exception();
            Exception actualException = null;

            Try.Running<object>(() => { throw expectedException; })
                .Report(ex => actualException = ex)
                .Until(_ => true);

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Until_defaults_to_handling_Exception_when_no_default_handlers_specified()
        {
            Try.Running<object>(() => { throw new Exception(); })
                .Until(_ => true);
        }

        [Fact]
        public void Until_uses_default_exception_handler_when_Handle_not_called()
        {
            TryDefault.AddHandle<TestException1>();

            Try.Running<object>(() => { throw new TestException1(); })
                .Until(_ => true);

            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .Until(_ => true));
        }

        [Fact]
        public void Until_handles_exceptions_specified_with_Handle()
        {
            Try.Running<object>(() => { throw new TestException1(); })
                .Handle<TestException1>()
                .Until(_ => true);
        }

        [Fact]
        public void Until_propagates_exceptions_not_specified_with_Handle()
        {
            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .Handle<TestException1>()
                    .Until(_ => true));
        }

        [Fact]
        public void Until_handles_multiple_exception_types_specified_with_Handle()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            TryDefault.RetryStrategy = retryStrategy.Object;

            var i = 0;
            Try.Running<object>(() =>
            {
                if (i == 0)
                {
                    throw new TestException1();
                }

                throw new TestException2();
            })
                .Handle<TestException1>()
                .Handle<TestException2>()
                .Until(_ => i++ >= 2);
        }

        [Fact]
        public void Until_propagates_exceptions_not_specified_with_multiple_Handle_calls()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            TryDefault.RetryStrategy = retryStrategy.Object;

            var i = 0;
            Assert.Throws<TestException3>(() =>
                Try.Running<object>(() =>
                {
                    if (i == 0)
                    {
                        throw new TestException1();
                    }

                    throw new TestException3();
                })
                    .Handle<TestException1>()
                    .Handle<TestException2>()
                    .Until(_ => i++ >= 2));
        }

        private class TestException1 : Exception
        {
        }

        private class TestException2 : Exception
        {
        }

        private class TestException3 : Exception
        {
        }
    }
}
