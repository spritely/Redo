using Moq;
using System;
using System.Linq;
using Xunit;

namespace Spritely.ControlFlow.Test
{
    public class TryTest
    {
        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.Running(null as Action));
            Assert.Throws<ArgumentNullException>(() => Try.Running(null as Func<object>));
        }

        [Fact]
        public void Until_hides_exception_from_caller()
        {
            Try.Running(() => { throw new Exception(); })
                .Until(() => true);
        }

        [Fact]
        public void With_sets_the_retry_strategy_on_action_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // On exception ShouldQuit() = false so code will reach Wait()
            retryStrategy.Setup(s => s.ShouldQuit()).Returns(false);

            // Run twice to ensure Until() reaches Wait()
            var times = 1;
            Try.Running(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(() => times++ == 2);

            retryStrategy.Verify(s => s.ShouldQuit(), Times.AtLeastOnce);
            retryStrategy.Verify(s => s.Wait(), Times.AtLeastOnce);
        }

        [Fact]
        public void With_sets_the_retry_strategy_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // On exception ShouldQuit() = false so code will reach Wait()
            retryStrategy.Setup(s => s.ShouldQuit()).Returns(false);

            // Run twice to ensure Until() reaches Wait()
            var times = 1;
            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => times++ == 2);

            retryStrategy.Verify(s => s.ShouldQuit(), Times.AtLeastOnce);
            retryStrategy.Verify(s => s.Wait(), Times.AtLeastOnce);
        }

        [Fact]
        public void Until_returns_result_of_successful_call_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            var expected = new Object();
            var actual = Try.Running(() => expected)
                    .With(retryStrategy.Object)
                    .Until(_ => true);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Until_retries_until_ShouldQuit_returns_true_on_action_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // false, false, false...., true
            var falseCount = new Random().Next(2, 10);
            var shouldQuitReturns = Enumerable.Range(0, falseCount).Select(i => false).Concat(new bool[] { true }).GetEnumerator();
            var calls = 0;
            retryStrategy.Setup(s => s.ShouldQuit()).Returns(() =>
            {
                calls++;
                shouldQuitReturns.MoveNext();
                return shouldQuitReturns.Current;
            });

            var times = 0;
            Assert.Throws<Exception>(() =>
                Try.Running(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(() => times++ == (falseCount + 2))); // No infinite loop on test failure

            Assert.Equal(falseCount + 1, calls);
        }

        [Fact]
        public void Until_retries_until_ShouldQuit_returns_true_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            // false, false, false...., true
            var falseCount = new Random().Next(2, 10);
            var shouldQuitReturns = Enumerable.Range(0, falseCount).Select(i => false).Concat(new bool[] { true }).GetEnumerator();
            var calls = 0;
            retryStrategy.Setup(s => s.ShouldQuit()).Returns(() =>
            {
                calls++;
                shouldQuitReturns.MoveNext();
                return shouldQuitReturns.Current;
            });

            var times = 0;

            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(_ => times++ == (falseCount + 2))); // No infinite loop on test failure

            Assert.Equal(falseCount + 1, calls);
        }

        [Fact]
        public void Wait_is_not_called_after_ShouldQuit_returns_true_on_action_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit()).Returns(true);

            var times = 0;
            Assert.Throws<Exception>(() =>
                Try.Running(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(() => times++ == 2)); // Do not return via Until on the first attempt

            retryStrategy.Verify(s => s.Wait(), Times.Never);
        }

        [Fact]
        public void Wait_is_not_called_after_ShouldQuit_returns_true_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit()).Returns(true);

            var times = 0;
            Assert.Throws<Exception>(() =>
                Try.Running<object>(() => { throw new Exception(); })
                    .With(retryStrategy.Object)
                    .Until(_ => times++ == 2)); // Do not return via Until on the first attempt

            retryStrategy.Verify(s => s.Wait(), Times.Never);
        }

        [Fact]
        public void Wait_is_not_called_when_Until_returns_true_on_action_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            Try.Running(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(() => true);

            retryStrategy.Verify(s => s.Wait(), Times.Never);
        }

        [Fact]
        public void Wait_is_not_called_when_Until_returns_true_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => true);

            retryStrategy.Verify(s => s.Wait(), Times.Never);
        }

        [Fact]
        public void Until_rethrows_original_exception_when_ShouldQuit_returns_true_on_action_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit()).Returns(true);

            var expectedException = new Exception();
            var times = 0;
            try
            {
                Try.Running(() => { throw expectedException; })
                    .With(retryStrategy.Object)
                    .Until(() => times++ == 2); // No infinite loop on test failure
            }
            catch (Exception actualException)
            {
                Assert.Same(expectedException, actualException);
            }
        }

        [Fact]
        public void Until_rethrows_original_exception_when_ShouldQuit_returns_true_on_function_runners()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            retryStrategy.Setup(s => s.ShouldQuit()).Returns(true);

            var expectedException = new Exception();
            var times = 0;
            try
            {
                Try.Running<object>(() => { throw expectedException; })
                    .With(retryStrategy.Object)
                    .Until(_ => times++ == 2); // No infinite loop on test failure
            }
            catch (Exception actualException)
            {
                Assert.Same(expectedException, actualException);
            }
        }
    }
}
