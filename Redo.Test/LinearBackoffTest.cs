﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearBackOffTest.cs">
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
    using System.Diagnostics;
    using FluentAssertions;
    using Xunit;

    public class LinearBackOffTest
    {
        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_once_with_overload_1()
        {
            var times = 0;
            var expectedDelay = TimeSpan.FromMilliseconds(150);
            var retriableOperation = Using.LinearBackOff(expectedDelay).Run(
                () =>
                {
                    times++;
                    if (times > 1)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 50);
            times.Should().Be(2); // 1 failure + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_once_with_overload_2()
        {
            var times = 0;
            var expectedDelay = TimeSpan.FromMilliseconds(150);
            var retriableOperation = Using.LinearBackOff(expectedDelay, TimeSpan.FromSeconds(5)).Run(
                () =>
                {
                    times++;
                    if (times > 1)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 50);
            times.Should().Be(2); // 1 failure + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_twice_with_overload_1()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(100);
            var expectedDelay = TimeSpan.FromMilliseconds(300); // 100 + 200
            var retriableOperation = Using.LinearBackOff(delay).Run(
                () =>
                {
                    times++;
                    if (times > 2)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 75);
            times.Should().Be(3); // 2 failures + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_twice_with_overload_2()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(100);
            var delta = TimeSpan.FromMilliseconds(50);
            var expectedDelay = TimeSpan.FromMilliseconds(250); // 100 + 150
            var retriableOperation = Using.LinearBackOff(delay, delta).Run(
                () =>
                {
                    times++;
                    if (times > 2)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 75);
            times.Should().Be(3); // 2 failures + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_five_times_with_overload_1()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(50);
            var expectedDelay = TimeSpan.FromMilliseconds(750); // 50 + 100 + 150 + 200 + 250
            var retriableOperation = Using.LinearBackOff(delay).Run(
                () =>
                {
                    times++;
                    if (times > 5)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 150);
            times.Should().Be(6); // 5 failures + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_five_times_with_overload_2()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(50);
            var delta = TimeSpan.FromMilliseconds(20);
            var expectedDelay = TimeSpan.FromMilliseconds(540); // 50 + 70 + 90 + 110 + 120
            var retriableOperation = Using.LinearBackOff(delay, delta).Run(
                () =>
                {
                    times++;
                    if (times > 5)
                    {
                        return;
                    }
                    throw new InvalidOperationException();
                });

            var stopWatch = Stopwatch.StartNew();
            retriableOperation.Now();
            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 150);
            times.Should().Be(6); // 5 failures + 1 success
        }
    }
}
