using System;

namespace Spritely.Redo
{
    internal class TryConfiguration
    {
        public IRetryStrategy RetryStrategy { get; set; }
        public ExceptionListener ExceptionListeners { get; set; }
        public ExceptionList Handles { get; set; }

        public void Report(Exception ex)
        {
            this.ExceptionListeners(ex);
        }
    }
}
