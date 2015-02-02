using System;
using System.Linq;

namespace Spritely.Redo
{
    public static class TryDefault
    {
        private static readonly object Lock = new object();
        private static ExceptionList handles;
        private static IRetryStrategy retryStrategy;
        private static ExceptionListener exceptionListeners;
        private static TimeSpan delay;
        private static int maxRetries;

        public static TimeSpan Delay
        {
            get { return delay; }
            set
            {
                lock (Lock)
                {
                    delay = value;
                }
            }
        }

        public static int MaxRetries
        {
            get { return maxRetries; }
            set
            {
                lock (Lock)
                {
                    maxRetries = value;
                }
            }
        }

        public static IRetryStrategy RetryStrategy
        {
            get { return retryStrategy; }
            set
            {
                lock (Lock)
                {
                    retryStrategy = value ?? new SimpleDelayRetryStrategy();
                }
            }
        }

        public static ExceptionListener ExceptionListeners
        {
            get { return exceptionListeners; }
            set
            {
                lock (Lock)
                {
                    exceptionListeners = value ?? (exceptionListeners = ex => { });
                }
            }
        }

        static TryDefault()
        {
            Reset();
        }

        public static void AddHandle<T>() where T : Exception
        {
            lock (Lock)
            {
                handles.Add<T>();
            }
        }

        public static void RemoveHandle<T>() where T : Exception
        {
            lock (Lock)
            {
                handles.Remove<T>();
            }
        }

        public static void ResetHandles<T>() where T : Exception
        {
            lock (Lock)
            {
                handles.Reset();
            }
        }

        public static void Reset()
        {
            lock (Lock)
            {
                retryStrategy = new SimpleDelayRetryStrategy();
                exceptionListeners = ex => { };
                handles = new ExceptionList();
                maxRetries = 30;
                delay = TimeSpan.FromSeconds(1);
            }
        }

        internal static TryConfiguration NewConfiguration()
        {
            TryConfiguration configuration;
            lock (Lock)
            {
                configuration = new TryConfiguration
                {
                    ExceptionListeners = ExceptionListeners,
                    Handles = new ExceptionList(),
                    RetryStrategy = RetryStrategy
                };

                handles.ToList().ForEach(h => configuration.Handles.Add(h));
            }

            return configuration;
        }
    }
}
