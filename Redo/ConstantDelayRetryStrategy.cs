﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantDelayRetryStrategy.cs">
//   Copyright (c) 2016. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Spritely.Redo source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Spritely.Redo
{
    /// <summary>
    ///     A retry strategy with a constant delay.
    /// </summary>
    public class ConstantDelayRetryStrategy : IRetryStrategy
    {
        internal Func<TimeSpan, TimeSpan> calculateSleepTime = CalculateSleepTime;

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        /// <value>
        ///     The delay.
        /// </value>
        public TimeSpan Delay { get; set; }

        /// <summary>
        ///     Gets or sets the maximum retries.
        /// </summary>
        /// <value>
        ///     The maximum retries.
        /// </value>
        public int MaxRetries { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantDelayRetryStrategy" /> class.
        /// </summary>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="delay">The delay.</param>
        public ConstantDelayRetryStrategy(int maxRetries, TimeSpan delay)
        {
            MaxRetries = maxRetries;
            Delay = delay;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantDelayRetryStrategy" /> class.
        /// </summary>
        public ConstantDelayRetryStrategy()
        {
            MaxRetries = TryDefault.MaxRetries;
            Delay = TryDefault.Delay;
        }

        /// <inheritdoc />
        public bool ShouldQuit(long attempt)
        {
            return attempt > MaxRetries;
        }

        /// <inheritdoc />
        public void Wait(long attempt)
        {
            var sleepTime = calculateSleepTime(Delay);

            Thread.Sleep(sleepTime);
        }

        internal static TimeSpan CalculateSleepTime(TimeSpan delay)
        {
            var sleepTime = TimeSpan.FromMilliseconds(Math.Max(1, delay.TotalMilliseconds));

            return sleepTime;
        }
    }
}
