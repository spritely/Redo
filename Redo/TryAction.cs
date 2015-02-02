// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryAction.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    public class TryAction : TryConfigurable<TryAction>
    {
        internal Action f;
        internal Func<Func<object>, Func<object, bool>, TryConfiguration, object> until = Run.Until;

        public TryAction(Action f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;
        }

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

        public void Now()
        {
            this.Until(() => true);
        }
    }
}
