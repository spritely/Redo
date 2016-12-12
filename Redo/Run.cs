// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Run.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Spritely.Redo
{
    using System.Threading.Tasks;

    internal static class Run
    {
        public static TResult Until<TResult>(
            Func<TResult> f,
            Func<TResult, bool> satisfied,
            TryConfiguration configuration)
        {
            var result = default(TResult);
            var attempt = 1L;
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

                    if (configuration.RetryStrategy.ShouldQuit(attempt))
                    {
                        throw;
                    }
                }

                if (satisfied(result))
                {
                    return result;
                }

                configuration.RetryStrategy.Wait(attempt++);
            }
        }

        public static async Task<TResult> UntilAsync<TResult>(
            Func<Task<TResult>> f,
            Func<TResult, bool> satisfied,
            TryConfiguration configuration)
        {
            var result = default(TResult);
            var attempt = 1L;
            while (true)
            {
                try
                {
                    result = await f();
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

                    if (configuration.RetryStrategy.ShouldQuit(attempt))
                    {
                        throw;
                    }
                }

                if (satisfied(result))
                {
                    return result;
                }

                configuration.RetryStrategy.Wait(attempt++);
            }
        }
    }
}
