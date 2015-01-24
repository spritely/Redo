using System;

namespace Spritely.ControlFlow
{
    public static class TryDefault
    {
        private static IRetryStrategy retryStrategy;

        public static IRetryStrategy RetryStrategy
        {
            get
            {
                return retryStrategy ?? (retryStrategy = new SleepAndInfiniteRetryStrategy());
            }
            set
            {
                retryStrategy = value;
            }
        }
    }
}
