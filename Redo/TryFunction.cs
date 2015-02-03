// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryFunction.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    /// <summary>
    ///     Part of fluent API when user calls Try.Running() with a function.
    /// </summary>
    /// <typeparam name="T">Type of the result of the call to f passed to Try.Running().</typeparam>
    public sealed class TryFunction<T> : TryOperation<TryFunction<T>>
    {
        internal Func<T> f;
        internal Func<Func<T>, Func<T, bool>, TryConfiguration, T> until = Run.Until;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TryFunction{T}" /> class.
        /// </summary>
        /// <param name="f">The function to call with retries.</param>
        /// <exception cref="System.ArgumentNullException">f;Running requires a valid function to call.</exception>
        public TryFunction(Func<T> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;
        }

        /// <summary>
        ///     Initiates the call and retries until the specified condition is satisfied or the retry strategy cancels the
        ///     request.
        /// </summary>
        /// <param name="satisfied">The operation that determines success.</param>
        public T Until(Func<T, bool> satisfied)
        {
            return this.until(this.f, satisfied, this.configuration);
        }

        /// <summary>
        ///     Initiates the call and retries until the result is not null or the retry strategy cancels the request.
        /// </summary>
        public T UntilNotNull()
        {
            return this.Until(v => v != null);
        }
    }
}
