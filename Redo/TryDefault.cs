using System;

namespace Spritely.Redo
{
    public static class TryDefault
    {
        internal static object Lock = new object();
        internal static ExceptionList handles;
        private static IRetryStrategy retryStrategy;
        private static ExceptionListener exceptionListeners;

        static TryDefault()
        {
            Reset();
        }

        public static IRetryStrategy RetryStrategy
        {
            get { return retryStrategy; }
            set
            {
                lock (Lock)
                {
                    retryStrategy = value ?? new SleepWithInfiniteRetriesStrategy();
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
                retryStrategy = new SleepWithInfiniteRetriesStrategy();
                exceptionListeners = ex => { };
                handles = new ExceptionList();
            }
        }
    }
}
