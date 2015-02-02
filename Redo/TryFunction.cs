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
