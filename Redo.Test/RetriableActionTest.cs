﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetriableActionTest.cs">
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
    using System.Globalization;
    using FluentAssertions;
    using Xunit;

    public class RetriableActionTest
    {
        private Action Action(Action act) => () => act();

        [Fact]
        public void Now_does_not_delay_when_first_execution_succeeds()
        {
            var called = false;
            var retriableOperation = new BackOffStrategy(
                _ =>
                {
                    called = true;
                    return TimeSpan.FromSeconds(10);
                }).Run(() => { });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
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
                .Run(() =>
                    {
                        times++;
                        throw new InvalidOperationException();
                    });

            Action(() => retriableOperation.Now()).ShouldThrow<InvalidOperationException>();
            times.Should().Be(maxRetries + 1); // Initial call and maxRetries retry attempts
        }

        [Fact]
        public void Now_calls_reporter_with_each_failure()
        {
            var retries = 4;
            var times = 0;
            var called = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .WithReporter(
                    ex =>
                    {
                        called++;
                        ex.Should().BeOfType<InvalidOperationException>();
                        ex.Message.Should().Be(times.ToString(CultureInfo.InvariantCulture));
                    })
                .Run(() =>
                {
                    times++;

                    if (times > retries)
                    {
                        return;
                    }

                    throw new InvalidOperationException(times.ToString(CultureInfo.InvariantCulture));
                });

            retriableOperation.Now();
            called.Should().Be(retries);
        }

        [Fact]
        public void Now_retries_on_all_exceptions_when_unspecified()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .Run(() =>
                {
                    times++;

                    switch (times)
                    {
                        case 1:
                            throw new FormatException();
                        case 2:
                            throw new InvalidOperationException();
                    }
                });

            retriableOperation.Now();
            times.Should().Be(3);
        }

        [Fact]
        public void Now_only_retries_specified_exceptions_when_any_specified()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .RetryOn<FormatException>()
                .RetryOn<InvalidOperationException>()
                .Run(() =>
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

            Action(() => retriableOperation.Now()).ShouldThrow<ArgumentException>();
            times.Should().Be(3);
        }

        [Fact]
        public void Now_throws_when_encountering_a_throw_on_exception()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .ThrowOn<ArgumentException>()
                .Run(() =>
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

            Action(() => retriableOperation.Now()).ShouldThrow<ArgumentException>();
            times.Should().Be(3);
        }

        [Fact]
        public void Now_throws_when_encountering_any_throw_on_exception()
        {
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .ThrowOn<ArgumentException>()
                .ThrowOn<IndexOutOfRangeException>()
                .Run(() =>
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

            Action(() => retriableOperation.Now()).ShouldThrow<IndexOutOfRangeException>();
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
                .Run(() =>
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

            Action(() => retriableOperation.Now()).ShouldThrow<IndexOutOfRangeException>();
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
            Action operation = () => { };

            Action(() => new RetriableAction(null, maxRetries, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableAction(getDelay, 0, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentOutOfRangeException>();
            Action(() => new RetriableAction(getDelay, -1, report, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentOutOfRangeException>();
            Action(() => new RetriableAction(getDelay, maxRetries, null, exceptionsToRetry, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableAction(getDelay, maxRetries, report, null, exceptionsToThrow, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableAction(getDelay, maxRetries, report, exceptionsToRetry, null, operation)).ShouldThrow<ArgumentNullException>();
            Action(() => new RetriableAction(getDelay, maxRetries, report, exceptionsToRetry, exceptionsToThrow, null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Until_throws_on_null_argument()
        {
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .Run(() => { });

            Action(() => retriableOperation.Until(null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Now_calls_Until_up_to_max_retries_when_until_does_not_become_valid()
        {
            var maxRetries = 6;
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .WithMaxRetries(maxRetries)
                .Run(() => { })
                .Until(
                    () =>
                    {
                        times++;
                        return false;
                    });

            Action(() => retriableOperation.Now()).ShouldThrow<TimeoutException>();
            times.Should().Be(maxRetries + 1); // Initial call and maxRetries retry attempts
        }

        [Fact]
        public void Now_finishes_when_Until_becomes_valid()
        {
            var retries = 8;
            var times = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .Run(() => { })
                .Until(
                    () =>
                    {
                        times++;
                        return times == retries;
                    });

            retriableOperation.Now();
            times.Should().Be(retries);
        }

        [Fact]
        public void Now_finishes_when_any_Until_becomes_valid()
        {
            var retries1 = 7;
            var retries2 = 2;
            var times1 = 0;
            var times2 = 0;
            var retriableOperation = Using.ConstantBackOff(TimeSpan.FromMilliseconds(10))
                .Run(() => { })
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

            retriableOperation.Now();
            times1.Should().Be(retries2);
            times2.Should().Be(retries2);
        }
    }
}