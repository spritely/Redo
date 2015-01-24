using System;
using System.Threading;

namespace Spritely.ControlFlow
{
    public class SleepAndInfiniteRetryStrategy : IRetryStrategy
    {
        public static TimeSpan DefaultSleepTime = TimeSpan.FromSeconds(30);
        
        public TimeSpan SleepTime { get; set; }

        public SleepAndInfiniteRetryStrategy(TimeSpan sleepTime)
        {
            this.SleepTime = sleepTime;
        }

        public SleepAndInfiniteRetryStrategy()
        {
            this.SleepTime = DefaultSleepTime;
        }

        public bool ShouldQuit()
        {
            return false;
        }

        public void Wait()
        {
            Thread.Sleep(SleepTime);
        }
    }
}
