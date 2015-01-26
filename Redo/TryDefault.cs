namespace Spritely.Redo
{
    public static class TryDefault
    {
        private static IRetryStrategy retryStrategy;

        public static IRetryStrategy RetryStrategy
        {
            get
            {
                return retryStrategy ?? (retryStrategy = new SleepWithInfiniteRetriesStrategy());
            }
            set
            {
                retryStrategy = value;
            }
        }
    }
}
