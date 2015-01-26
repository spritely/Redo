using System;

namespace Spritely.ControlFlow
{
    internal static class UntilExtension
    {
        public static TResult Until<TException, TResult>(this Func<TResult> f, Func<TResult, bool> satisfied, TryConfiguration configuration)
            where TException : Exception
        {
            var result = default(TResult);
            while (true)
            {
                try
                {
                    result = f();
                }
                catch (TException ex)
                {
                    configuration.Log(ex);
                    if (configuration.RetryStrategy.ShouldQuit())
                    {
                        throw;
                    }
                }

                if (satisfied(result))
                {
                    return result;
                }

                configuration.RetryStrategy.Wait();
            }
        }
    }
}
