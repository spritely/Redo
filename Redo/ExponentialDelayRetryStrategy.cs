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
    public class ExponentialDelayRetryStrategy : IRetryStrategy
    {
        internal Func<long, TimeSpan, double, TimeSpan> calculateSleepTime = CalculateSleepTime;
        public TimeSpan Delay { get; set; }
        public int MaxRetries { get; set; }
        public double ScaleFactor { get; set; }

        public ExponentialDelayRetryStrategy(double scaleFactor, int maxRetries, TimeSpan delay)
        {
            this.ScaleFactor = scaleFactor;
            this.MaxRetries = maxRetries;
            this.Delay = delay;
        }

        public ExponentialDelayRetryStrategy(double scaleFactor)
        {
            this.ScaleFactor = scaleFactor;
            this.MaxRetries = TryDefault.MaxRetries;
            this.Delay = TryDefault.Delay;
        }

        public bool ShouldQuit(long attempt)
        {
            return attempt > this.MaxRetries;
        }

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
