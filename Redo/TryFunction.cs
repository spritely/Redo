using System;
using System.Linq;

namespace Spritely.Redo
{
    public class TryFunction<T>
    {
        internal TryConfiguration configuration;
        internal Func<T> f;
        internal Func<Func<T>, Func<T, bool>, TryConfiguration, T> until = Run.Until;

        public TryFunction(Func<T> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;

            lock (TryDefault.Lock)
            {
                this.configuration = new TryConfiguration
                {
                    ExceptionListeners = TryDefault.ExceptionListeners,
                    Handles = new ExceptionList(),
                    RetryStrategy = TryDefault.RetryStrategy
                };

                TryDefault.handles.ToList().ForEach(h => this.configuration.Handles.Add(h));
            }
        }

        public T Until(Func<T, bool> satisfied)
        {
            return this.until(this.f, satisfied, this.configuration);
        }

        public TryFunction<T> With(IRetryStrategy retryStrategy)
        {
            this.configuration.RetryStrategy = retryStrategy;

            return this;
        }

        public TryFunction<T> Report(ExceptionListener exceptionLogger)
        {
            this.configuration.ExceptionListeners += exceptionLogger;

            return this;
        }

        public TryFunction<T> Handle<TException>() where TException : Exception
        {
            this.configuration.Handles.Add<TException>();

            return this;
        }
    }
}
