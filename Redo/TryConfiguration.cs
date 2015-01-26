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

        private LogException exceptionLoggers;

        public LogException ExceptionLoggers
        {
            get
            {
                return this.exceptionLoggers ?? (this.exceptionLoggers = TryDefault.ExceptionLoggers);
            }
            set
            {
                this.exceptionLoggers = value;
            }
        }

        public void Log(Exception ex)
        {
            this.ExceptionLoggers(ex);
        }
    }
}
