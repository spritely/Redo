using System;
using System.Threading.Tasks;

namespace Spritely.Redo
{
    public class TryFunctionAsync<T> : TryConfigurable<TryFunctionAsync<T>>
    {
        internal Func<T> f;
        internal Func<Func<T>, Func<T, bool>, TryConfiguration, T> until = Run.Until;

        public TryFunctionAsync(Func<T> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException("f", "Running requires a valid function to call.");
            }

            this.f = f;
        }

        public async Task<T> Until(Func<T, bool> satisfied)
        {
            return await Task.Run(() => this.until(this.f, satisfied, this.configuration));
        }

        public async Task<T> Now()
        {
            return await Task.Run(() => this.until(this.f, _ => true, this.configuration));
        }
    }
}
