using System;

namespace Spritely.ControlFlow
{
    public class TryAction
    {
        internal Action F;
        internal TryConfiguration Configuration;

        public TryAction(Action f)
        {
            this.F = f;
            this.Configuration = new TryConfiguration();
        }

        public void Until(Func<bool> satisfied)
        {
            // Converting Action into a Func<object> so Until logic can be shared
            Func<object> f = (() =>
            {
                this.F();
                return null;
            });
            
            f.Until<Exception, object>(_ => satisfied(), this.Configuration);
        }

        public TryAction With(IRetryStrategy retryStrategy)
        {
            this.Configuration.RetryStrategy = retryStrategy;

            return this;
        }
    }
}
