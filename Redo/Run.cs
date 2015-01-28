using System;
using System.Linq;

namespace Spritely.Redo
{
    internal static class Run
    {
        public static TResult Until<TResult>(Func<TResult> f, Func<TResult, bool> satisfied,
            TryConfiguration configuration)
        {
            var result = default(TResult);
            while (true)
            {
                try
                {
                    result = f();
                }
                catch (Exception ex)
                {
                    configuration.Report(ex);

                    // If there are no exception handlers then handle all exceptions by default
                    var shouldHandle = !configuration.Handles.Any() ||
                                       configuration.Handles.Any(t => t.IsInstanceOfType(ex));

                    if (!shouldHandle)
                    {
                        throw;
                    }

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
