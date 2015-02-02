// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Try.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    /// <summary>
    ///     The main entry point to start building a retryable function.
    /// </summary>
    public static class Try
    {
        /// <summary>
        ///     Constructs a retryable function.
        /// </summary>
        /// <param name="f">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryAction Running(Action f)
        {
            return new TryAction(f);
        }

        /// <summary>
        ///     Constructs a retryable function.
        /// </summary>
        /// <typeparam name="T">The return type of f.</typeparam>
        /// <param name="f">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryFunction<T> Running<T>(Func<T> f)
        {
            return new TryFunction<T>(f);
        }

        /// <summary>
        ///     Constructs a retryable asynchronous function.
        /// </summary>
        /// <param name="f">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryActionAsync RunningAsync(Action f)
        {
            return new TryActionAsync(f);
        }

        /// <summary>
        ///     Constructs a retryable asynchronous function.
        /// </summary>
        /// <typeparam name="T">The return type of f.</typeparam>
        /// <param name="f">The function to call with retries.</param>
        /// <returns>A retryable function.</returns>
        /// <exception cref="System.ArgumentNullException">Running requires a valid function to call.</exception>
        public static TryFunctionAsync<T> RunningAsync<T>(Func<T> f)
        {
            return new TryFunctionAsync<T>(f);
        }
    }
}
