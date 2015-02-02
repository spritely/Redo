// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleDelayRetryStrategy.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Spritely.Redo
{
    public class SimpleDelayRetryStrategy : IRetryStrategy
    {
        internal Func<TimeSpan, TimeSpan> calculateSleepTime = CalculateSleepTime;
        public TimeSpan Delay { get; set; }
        public int MaxRetries { get; set; }

        public SimpleDelayRetryStrategy(int maxRetries, TimeSpan delay)
        {
            this.MaxRetries = maxRetries;
            this.Delay = delay;
        }

        public SimpleDelayRetryStrategy()
        {
            this.MaxRetries = TryDefault.MaxRetries;
            this.Delay = TryDefault.Delay;
        }

        public bool ShouldQuit(long attempt)
        {
            return attempt > this.MaxRetries;
        }

        public void Wait(long attempt)
        {
            var sleepTime = this.calculateSleepTime(this.Delay);

            Thread.Sleep(sleepTime);
        }

        internal static TimeSpan CalculateSleepTime(TimeSpan delay)
        {
            var sleepTime = TimeSpan.FromMilliseconds(Math.Max(1, delay.TotalMilliseconds));

            return sleepTime;
        }
    }
}
