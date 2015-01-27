using Moq;
using System;
using System.Linq;
using Xunit;

namespace Spritely.Redo.Test
{
    // Most methods in this class have a near exact equivalent in TryActionTest
    public class TryFunctionTest
    {
        [Fact]
        public void Running_throws_on_null_argument()
        {
            Assert.Throws<ArgumentNullException>(() => Try.Running(null as Func<object>));
        }

        [Fact]
        public void Until_hides_exception_from_caller()
        {
            Try.Running<object>(() => { throw new Exception(); })
                .Until(_ => true);
        }

        [Fact]
        public void Until_uses_default_retry_strategy()
        {
            var retryStrategy = new Mock<IRetryStrategy>();
            TryDefault.RetryStrategy = retryStrategy.Object;

            // On exception ShouldQuit() = false so code will reach Wait()
            retryStrategy.Setup(s => s.ShouldQuit()).Returns(false);

            // Run twice to ensure Until() reaches Wait()
            var times = 1;
            Try.Running<object>(() => { throw new Exception(); })
                .Until(_ => times++ == 2);

            retryStrategy.Verify(s => s.ShouldQuit(), Times.AtLeastOnce);
            retryStrategy.Verify(s => s.Wait(), Times.AtLeastOnce);
        }

        [Fact]
        public void With_sets_the_retry_strategy()
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
        public void Until_returns_result_of_successful_call()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            var expected = new Object();
            var actual = Try.Running(() => expected)
                    .With(retryStrategy.Object)
                    .Until(_ => true);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Until_retries_until_ShouldQuit_returns_true()
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
        public void Wait_is_not_called_after_ShouldQuit_returns_true()
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
        public void Wait_is_not_called_when_Until_returns_true()
        {
            var retryStrategy = new Mock<IRetryStrategy>();

            Try.Running<object>(() => { throw new Exception(); })
                .With(retryStrategy.Object)
                .Until(_ => true);

            retryStrategy.Verify(s => s.Wait(), Times.Never);
        }

        [Fact]
        public void Until_rethrows_original_exception_when_ShouldQuit_returns_true()
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

        [Fact]
        public void Until_logs_exceptions_to_default_delegates()
        {
            var expectedException = new Exception();
            Exception actualException = null;
            TryDefault.ExceptionLoggers += ex => actualException = ex;

            Try.Running<object>(() => { throw expectedException; })
                .Until(_ => true);

            Assert.Same(expectedException, actualException);
        }

        [Fact]
        public void Until_logs_exceptions_to_call_specific_delegates()
        {
            var expectedException = new Exception();
            Exception actualException = null;

            Try.Running<object>(() => { throw expectedException; })
                .With(ex => actualException = ex)
                .Until(_ => true);

            Assert.Same(expectedException, actualException);
        }
    }
}
