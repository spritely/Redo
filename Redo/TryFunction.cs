using System;

namespace Spritely.Redo
{
    public class TryFunction<T>
    {
        internal Func<T> F;
        internal TryConfiguration Configuration;

        public TryFunction(Func<T> f)
        {
            this.F = f;
            this.Configuration = new TryConfiguration();
        }

        public T Until(Func<T, bool> satisfied)
        {
            return this.F.Until<Exception, T>(satisfied, this.Configuration);
        }

        public TryFunction<T> With(IRetryStrategy retryStrategy)
        {
            this.Configuration.RetryStrategy = retryStrategy;

            return this;
        }
    }
}
