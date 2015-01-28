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
    }
}
