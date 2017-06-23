﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressiveBackOffTest.cs">
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

    public class ProgressiveBackOffTest
    {
        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_once()
        {
            var times = 0;
            var expectedDelay = TimeSpan.FromMilliseconds(500);
            var retriableOperation = Using.ProgressiveBackOff(expectedDelay, scaleFactor: 10.0).Run(
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

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 250);
            times.Should().Be(2); // 1 failure + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_twice()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(200);
            var expectedDelay = TimeSpan.FromMilliseconds(600); // 200 + (2 * 200)
            var retriableOperation = Using.ProgressiveBackOff(delay, scaleFactor: 2.0).Run(
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

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 250);
            times.Should().Be(3); // 2 failures + 1 success
        }

        [Fact]
        public void Now_delays_by_expected_time_when_execution_fails_five_times()
        {
            var times = 0;
            var delay = TimeSpan.FromMilliseconds(100);
            var expectedDelay = TimeSpan.FromMilliseconds(1100); // 100 + (100 * 1) + (100 * 2) + (100 * 3) + (100 * 4)
            var retriableOperation = Using.ProgressiveBackOff(delay).Run(
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

            stopWatch.Elapsed.Should().BeCloseTo(expectedDelay, precision: 300);
            times.Should().Be(6); // 5 failures + 1 success
        }
    }
}
