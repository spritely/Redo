using System;

namespace Spritely.Redo
{
    internal class TryConfiguration
    {
        private IRetryStrategy retryStrategy;

        public IRetryStrategy RetryStrategy
        {
            get
            {
                return this.retryStrategy ?? (this.retryStrategy = TryDefault.RetryStrategy);
            }
            set
            {
                this.retryStrategy = value;
            }
        }

        public void Log(Exception ex)
        {
            // TODO: Allow callers to attach delegates that can be called
        }
    }
}
