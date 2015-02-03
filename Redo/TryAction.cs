// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryAction.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    /// <summary>
    ///     Part of fluent API when user calls Try.Running() with an action.
    /// </summary>
    public sealed class TryAction : TryOperation<TryAction>
    {
        internal Action f;
        internal Func<Func<object>, Func<object, bool>, TryConfiguration, object> until = Run.Until;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TryAction" /> class.
        /// </summary>
        /// <param name="f">The action to call with retries.</param>
        /// <exception cref="System.ArgumentNullException">f;Running requires a valid function to call.</exception>
        public TryAction(Action f)
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
        public void Until(Func<bool> satisfied)
        {
            // Converting Action into a Func<object> so Run logic can be shared
            Func<object> f = () =>
            {
                this.f();
                return null;
            };

            this.until(f, _ => satisfied(), this.configuration);
        }
    }
}
