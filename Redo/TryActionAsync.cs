// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryActionAsync.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Spritely.Redo
{
    /// <summary>
    ///     Part of fluent API when user calls Try.RunningAsync() with an asynchronous action.
    /// </summary>
    public sealed class TryActionAsync : TryOperation<TryActionAsync>
    {
        internal Action f;
        internal Func<Func<object>, Func<object, bool>, TryConfiguration, object> until = Run.Until;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TryActionAsync" /> class.
        /// </summary>
        /// <param name="action">The action to call with retries.</param>
        /// <exception cref="System.ArgumentNullException">f;Running requires a valid function to call.</exception>
        public TryActionAsync(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "Running requires a valid function to call.");
            }

            this.f = action;
        }

        /// <summary>
        ///     Initiates the call and retries until the specified condition is satisfied or the retry strategy cancels the
        ///     request.
        /// </summary>
        /// <param name="satisfied">The operation that determines success.</param>
        public async Task Until(Func<bool> satisfied)
        {
            // Converting Action into a Func<object> so Run logic can be shared
            Func<object> f = () =>
            {
                this.f();
                return null;
            };

            await Task.Run(() => this.until(f, _ => satisfied(), this.configuration));
        }
    }
}
