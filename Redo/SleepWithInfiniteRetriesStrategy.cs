using System;
using System.Threading;

namespace Spritely.Redo
{
    public class SleepWithInfiniteRetriesStrategy : IRetryStrategy
    {
        public static TimeSpan DefaultSleepTime = TimeSpan.FromSeconds(30);

        public TimeSpan SleepTime { get; set; }

        public SleepWithInfiniteRetriesStrategy(TimeSpan sleepTime)
        {
            this.SleepTime = sleepTime;
        }

        public SleepWithInfiniteRetriesStrategy()
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
