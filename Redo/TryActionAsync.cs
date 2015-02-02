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
    public class TryActionAsync : TryConfigurable<TryActionAsync>
    {
        internal Action f;
        internal Func<Func<object>, Func<object, bool>, TryConfiguration, object> until = Run.Until;

        public TryActionAsync(Action f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;
        }

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

        public async Task Now()
        {
            await Task.Run(() => this.Until(() => true));
        }
    }
}
