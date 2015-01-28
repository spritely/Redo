using System;
using System.Linq;

namespace Spritely.Redo
{
    public class TryAction
    {
        internal TryConfiguration configuration;
        internal Action f;
        internal Func<Func<object>, Func<object, bool>, TryConfiguration, object> until = Run.Until;

        public TryAction(Action f)
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

        public void Until(Func<bool> satisfied)
        {
            // Converting Action into a Func<object> so Run logic can be shared
            Func<object> f = () =>
            {
                this.f();
                return null;
            };

            this.until(f, _ => satisfied(), this.configuration);
        }

        public TryAction With(IRetryStrategy retryStrategy)
        {
            this.configuration.RetryStrategy = retryStrategy;

            return this;
        }

        public TryAction Report(ExceptionListener exceptionLogger)
        {
            this.configuration.ExceptionListeners += exceptionLogger;

            return this;
        }

        public TryAction Handle<TException>() where TException : Exception
        {
            this.configuration.Handles.Add<TException>();

            return this;
        }
    }
}
