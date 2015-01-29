using System;
using System.Diagnostics;
using Xunit;

namespace Spritely.Redo.Test
{
    public class ProgressiveDelayRetryStrategyTest
    {
        private readonly Random random = new Random();

        [Fact]
        public void Constructor_assigns_arguments_to_properties()
        {
            var expectedScaleFactor = this.random.NextDouble();
            var expectedMaxRetries = this.random.Next();
            var expectedDelay = TimeSpan.FromMilliseconds(this.random.Next());
            var retryStrategy = new ProgressiveDelayRetryStrategy(expectedScaleFactor, expectedMaxRetries, expectedDelay);

            Assert.Equal(expectedScaleFactor, retryStrategy.ScaleFactor);
            Assert.Equal(expectedMaxRetries, retryStrategy.MaxRetries);
            Assert.Equal(expectedDelay, retryStrategy.Delay);
        }

        [Fact]
        public void Constructor_assigns_default_properties()
        {
            var expectedScaleFactor = this.random.NextDouble();
            var retryStrategy = new ProgressiveDelayRetryStrategy(expectedScaleFactor);

            Assert.Equal(expectedScaleFactor, retryStrategy.ScaleFactor);
            Assert.Equal(TryDefault.MaxRetries, retryStrategy.MaxRetries);
            Assert.Equal(TryDefault.Delay, retryStrategy.Delay);
        }

        [Fact]
        public void ShouldQuit_returns_false_when_attempt_less_than_or_equal_maxretries()
        {
            var retryStrategy = new ProgressiveDelayRetryStrategy(1);

            Assert.False(retryStrategy.ShouldQuit(TryDefault.MaxRetries - 1));
            Assert.False(retryStrategy.ShouldQuit(TryDefault.MaxRetries));
        }

        [Fact]
        public void ShouldQuit_returns_true_when_attempt_greater_than_maxretries()
        {
            var retryStrategy = new ProgressiveDelayRetryStrategy(1);

            Assert.True(retryStrategy.ShouldQuit(TryDefault.MaxRetries + 1));
        }

        [Fact]
        public void Wait_calls_calculate_with_expected_values()
        {
            var wasCalled = false;
            var expectedAttempt = this.random.Next(1, int.MaxValue);
            var expectedScaleFactor = this.random.NextDouble();
            var retryStrategy = new ProgressiveDelayRetryStrategy(expectedScaleFactor);
            retryStrategy.Delay = TimeSpan.FromMilliseconds(this.random.Next(1, int.MaxValue));

            retryStrategy.calculateSleepTime = (attempt, delay, scaleFactor) =>
            {
                wasCalled = true;
                Assert.Equal(expectedAttempt, attempt);
                Assert.Equal(retryStrategy.Delay, delay);
                Assert.Equal(expectedScaleFactor, scaleFactor);
                return TimeSpan.FromMilliseconds(1); // This will cause a delay in test execution - keep value tiny
            };

            retryStrategy.Wait(expectedAttempt);
            Assert.True(wasCalled);
        }

        [Fact]
        public void Wait_sleeps_for_the_time_returned_from_calculateSleepTime()
        {
            var retryStrategy = new ProgressiveDelayRetryStrategy(1);

            // This will cause a delay in test execution - keep value tiny
            retryStrategy.calculateSleepTime = (_, __, ___) => TimeSpan.FromMilliseconds(50);

            var stopWatch = Stopwatch.StartNew();
            retryStrategy.Wait(1);
            stopWatch.Stop();

            // Stopwatch is pretty accurate, but test has possibility of other system delays introducing extra
            // delays and 50 milliseconds is pretty tiny so being generous on the high side
            Assert.InRange(stopWatch.ElapsedMilliseconds, 50, 100);
        }

        [Fact]
        public void CalculateSleepTime_returns_expected_delays()
        {
            var scaleFactor = 10;
            var delay = TimeSpan.FromMilliseconds(2);

            Assert.Equal(2, ProgressiveDelayRetryStrategy.CalculateSleepTime(1, delay, 10).TotalMilliseconds);
            Assert.Equal(20, ProgressiveDelayRetryStrategy.CalculateSleepTime(2, delay, 10).TotalMilliseconds);
            Assert.Equal(40, ProgressiveDelayRetryStrategy.CalculateSleepTime(3, delay, 10).TotalMilliseconds);
            Assert.Equal(100, ProgressiveDelayRetryStrategy.CalculateSleepTime(6, delay, 10).TotalMilliseconds);
        }

        [Fact]
        public void CalculateSleepTime_ensures_delay_is_at_least_1()
        {
            var actualDelay = ProgressiveDelayRetryStrategy.CalculateSleepTime(1, TimeSpan.Zero, -1);

            Assert.Equal(1, actualDelay.TotalMilliseconds);
        }

        [Fact]
        public void CalculateSleepTime_ensures_delay_is_no_greater_than_max_timespan_value()
        {
            var nearMaxTimeSpan = TimeSpan.MaxValue.Subtract(TimeSpan.FromMilliseconds(1));
            var actualDelay = ProgressiveDelayRetryStrategy.CalculateSleepTime(10, nearMaxTimeSpan, 10);

            Assert.Equal(TimeSpan.MaxValue, actualDelay);
        }
    }
}
