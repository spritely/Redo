﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetriableActionAsyncTest.cs">
//   Copyright (c) 2017. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Spritely.Redo source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class RetriableActionAsyncTest
    {
        private Action Action(Action act) => () => act();
        private readonly Func<Task> doNothingRunAsync = () => Task.FromResult(null as object);

        [Fact]
        public async Task Now_does_not_delay_when_first_execution_succeeds()
        {
            var called = false;
            var retriableOperation = new BackOffStrategy(
                _ =>
                {
                    called = true;
                    return TimeSpan.FromSeconds(10);
                }).RunAsync(doNothingRunAsync);

            var stopWatch = Stopwatch.StartNew();
            await retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(200));
            called.Should().BeFalse();
        }

        [Fact]
        public void Now_retries_up_to_specified_max_retries()
        {
            var maxRetries = 13;
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .WithMaxRetries(maxRetries)
                .RunAsync(() =>
                    {
                        times++;
                        throw new InvalidOperationException();
                    });

            AssertThrowsAggregateExceptionWithSingleInner<InvalidOperationException>(retriableOperation);
            times.Should().Be(maxRetries + 1); // Initial call and maxRetries retry attempts
        }

        [Fact]
        public async Task Now_calls_reporter_with_each_failure()
        {
            var retries = 4;
            var times = 0;
            var called = 0;
            Func<Task> run = () =>
            {
                times++;

                if (times > retries)
                {
                    return Task.FromResult(null as object);
                }

                throw new InvalidOperationException(times.ToString());
            };

            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .WithReporter(
                    ex =>
                    {
                        called++;
                        ex.Should().BeOfType<InvalidOperationException>();
                        ex.Message.Should().Be(times.ToString());
                    })
                .RunAsync(run);

            await retriableOperation.Now();
            called.Should().Be(retries);
        }

        [Fact]
        public async Task Now_retries_on_all_exceptions_when_unspecified()
        {
            var times = 0;
            Func<Task> run = () =>
            {
                times++;

                switch (times)
                {
                    case 1:
                        throw new FormatException();
                    case 2:
                        throw new InvalidOperationException();
                }

                return Task.FromResult(null as object);
            };

            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RunAsync(run);

            await retriableOperation.Now();
            times.Should().Be(3);
        }

        [Fact]
        public void Now_only_retries_specified_exceptions_when_any_specified()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RetryOn<FormatException>()
                .RetryOn<InvalidOperationException>()
                .RunAsync(() =>
                {
                    times++;

                    switch (times)
                    {
                        case 1:
                            throw new FormatException();
                        case 2:
                            throw new InvalidOperationException();
                    }

                    throw new ArgumentException();
                });

            AssertThrowsAggregateExceptionWithSingleInner<ArgumentException>(retriableOperation);
            times.Should().Be(3);
        }

        [Fact]
        public void Now_throws_when_encountering_a_throw_on_exception()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .ThrowOn<ArgumentException>()
                .RunAsync(() =>
                {
                    times++;

                    switch (times)
                    {
                        case 1:
                            throw new FormatException();
                        case 2:
                            throw new InvalidOperationException();
                    }

                    throw new ArgumentException();
                });

            AssertThrowsAggregateExceptionWithSingleInner<ArgumentException>(retriableOperation);
            times.Should().Be(3);
        }

        [Fact]
        public void Now_throws_when_encountering_any_throw_on_exception()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .ThrowOn<ArgumentException>()
                .ThrowOn<IndexOutOfRangeException>()
                .RunAsync(() =>
                {
                    times++;

                    switch (times)
                    {
                        case 1:
                            throw new FormatException();
                        case 2:
                            throw new InvalidOperationException();
                        case 3:
                            throw new IndexOutOfRangeException();
                    }

                    throw new ArgumentException();
                });

            AssertThrowsAggregateExceptionWithSingleInner<IndexOutOfRangeException>(retriableOperation);
            times.Should().Be(3);
        }

        [Fact]
        public void Now_gives_priority_to_throw_on_exception_over_retry_on_exception()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .ThrowOn<ArgumentException>()
                .ThrowOn<IndexOutOfRangeException>()
                .RetryOn<FormatException>()
                .RetryOn<InvalidOperationException>()
                .RetryOn<IndexOutOfRangeException>()
                .RunAsync(() =>
                {
                    times++;

                    switch (times)
                    {
                        case 1:
                            throw new FormatException();
                        case 2:
                            throw new InvalidOperationException();
                        case 3:
                            throw new IndexOutOfRangeException();
                    }

                    throw new ArgumentException();
                });

            AssertThrowsAggregateExceptionWithSingleInner<IndexOutOfRangeException>(retriableOperation);
            times.Should().Be(3);
        }

        [Fact]
        public void Constructor_throws_on_invalid_arguments()
        {
            Func<long, TimeSpan> getDelay = _ => TimeSpan.Zero;
            var maxRetries = 5;
            Action<Exception> report = _ => { };
            var exceptionsToRetry = new List<Type>();
            var exceptionsToThrow = new List<Type>();
            Func<Task> operation = () => Task.FromResult(null as object);

            Action(() => new RetriableActionAsync(null, maxRetries, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableActionAsync(getDelay, 0, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentOutOfRangeException>();
            Action(() => new RetriableActionAsync(getDelay, -1, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentOutOfRangeException>();
            Action(() => new RetriableActionAsync(getDelay, maxRetries, null, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableActionAsync(getDelay, maxRetries, report, null, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableActionAsync(getDelay, maxRetries, report, exceptionsToRetry, null, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableActionAsync(getDelay, maxRetries, report, exceptionsToRetry, exceptionsToThrow, null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Until_throws_on_null_argument()
        {
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RunAsync(doNothingRunAsync);

            Action(() => retriableOperation.Until(null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Now_calls_Until_up_to_max_retries_when_until_does_not_become_valid()
        {
            var maxRetries = 6;
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .WithMaxRetries(maxRetries)
                .RunAsync(doNothingRunAsync)
                .Until(
                    () =>
                    {
                        times++;
                        return false;
                    });

            AssertThrowsAggregateExceptionWithSingleInner<TimeoutException>(retriableOperation);
            times.Should().Be(maxRetries + 1); // Initial call and maxRetries retry attempts
        }

        [Fact]
        public async Task Now_finishes_when_Until_becomes_valid()
        {
            var retries = 8;
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RunAsync(doNothingRunAsync)
                .Until(
                    () =>
                    {
                        times++;
                        return times == retries;
                    });

            await retriableOperation.Now();
            times.Should().Be(retries);
        }

        [Fact]
        public async Task Now_finishes_when_any_Until_becomes_valid()
        {
            var retries1 = 7;
            var retries2 = 2;
            var times1 = 0;
            var times2 = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RunAsync(doNothingRunAsync)
                .Until(
                    () =>
                    {
                        times1++;
                        return times1 == retries1;
                    })
                .Until(
                    () =>
                    {
                        times2++;
                        return times2 == retries2;
                    });

            await retriableOperation.Now();
            times1.Should().Be(retries2);
            times2.Should().Be(retries2);
        }

        private void AssertThrowsAggregateExceptionWithSingleInner<TException>(RetriableActionAsync retriableOperation) 
            where TException : Exception
        {
            var exception = Action(() => Task.Run(retriableOperation.Now).Wait()).ShouldThrow<AggregateException>().And;
            exception.InnerExceptions.Count.Should().Be(1);
            exception.InnerExceptions.First().GetType().Should().Be<TException>();
        }
    }
}
