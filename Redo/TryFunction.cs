// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryFunction.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    public class TryFunction<T> : TryConfigurable<TryFunction<T>>
    {
        internal Func<T> f;
        internal Func<Func<T>, Func<T, bool>, TryConfiguration, T> until = Run.Until;

        public TryFunction(Func<T> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;
        }

        public T Until(Func<T, bool> satisfied)
        {
            return this.until(this.f, satisfied, this.configuration);
        }

        public T Now()
        {
            return this.until(this.f, _ => true, this.configuration);
        }
    }
}
