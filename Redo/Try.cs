// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Try.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    using System.Threading.Tasks;

    /// <summary>
    ///     The main entry point to start building a retryable function.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Try", Justification = "This is intentially designed to have a matching name as try keyword because it does a very similar thing and the name should make this obvious.")]
    public static class Try
    {
        /// <summary>
        ///     Constructs a retryable function.
        /// </summary>
        /// <param name="action">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryAction Running(Action action)
        {
            return new TryAction(action);
        }

        /// <summary>
        ///     Constructs a retryable function.
        /// </summary>
        /// <typeparam name="T">The return type of f.</typeparam>
        /// <param name="function">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryFunction<T> Running<T>(Func<T> function)
        {
            return new TryFunction<T>(function);
        }

        /// <summary>
        ///     Constructs a retryable asynchronous function.
        /// </summary>
        /// <param name="action">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryActionAsync RunningAsync(Func<Task> action)
        {
            return new TryActionAsync(action);
        }

        /// <summary>
        ///     Constructs a retryable asynchronous function.
        /// </summary>
        /// <typeparam name="T">The return type of f.</typeparam>
        /// <param name="function">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryFunctionAsync<T> RunningAsync<T>(Func<Task<T>> function)
        {
            return new TryFunctionAsync<T>(function);
        }
    }
}
