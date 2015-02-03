// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialDelayRetryStrategy.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Spritely.Redo
{
    /// <summary>
    ///     A retry strategy that backs off with an exponential decay.
    /// </summary>
    public class ExponentialDelayRetryStrategy : IRetryStrategy
    {
        internal Func<long, TimeSpan, double, TimeSpan> calculateSleepTime = CalculateSleepTime;

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
        ///     Gets or sets the scale factor.
        /// </summary>
        /// <value>
        ///     The scale factor.
        /// </value>
        public double ScaleFactor { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExponentialDelayRetryStrategy" /> class.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="delay">The delay.</param>
        public ExponentialDelayRetryStrategy(double scaleFactor, int maxRetries, TimeSpan delay)
        {
            this.ScaleFactor = scaleFactor;
            this.MaxRetries = maxRetries;
            this.Delay = delay;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExponentialDelayRetryStrategy" /> class.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        public ExponentialDelayRetryStrategy(double scaleFactor)
        {
            this.ScaleFactor = scaleFactor;
            this.MaxRetries = TryDefault.MaxRetries;
            this.Delay = TryDefault.Delay;
        }

        /// <inheritdoc />
        public bool ShouldQuit(long attempt)
        {
            return attempt > this.MaxRetries;
        }

        /// <inheritdoc />
        public void Wait(long attempt)
        {
            var sleepTime = this.calculateSleepTime(attempt, this.Delay, this.ScaleFactor);

            Thread.Sleep(sleepTime);
        }

        internal static TimeSpan CalculateSleepTime(long attempt, TimeSpan delay, double scaleFactor)
        {
            var factor = Math.Pow(scaleFactor, (attempt - 1));
            var safefactor = (factor < 1) ? 1 : factor;
            var totalDelay = delay.TotalMilliseconds * safefactor;

            var sleepTime = SafeDelay.ConstrainBounds(totalDelay);

            return sleepTime;
        }
    }
}
