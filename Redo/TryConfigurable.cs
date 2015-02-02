using System;

namespace Spritely.Redo
{
    public class TryConfigurable<T> where T : TryConfigurable<T>
    {
        internal TryConfiguration configuration;

        public TryConfigurable()
        {
            this.configuration = TryDefault.NewConfiguration();
        }

        public T With(IRetryStrategy retryStrategy)
        {
            this.configuration.RetryStrategy = retryStrategy;

            return this as T;
        }

        public T Report(ExceptionListener exceptionLogger)
        {
            this.configuration.ExceptionListeners += exceptionLogger;

            return this as T;
        }

        public T Handle<TException>() where TException : Exception
        {
            this.configuration.Handles.Add<TException>();

            return this as T;
        }
    }
}
