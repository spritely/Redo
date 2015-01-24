using System;

namespace Spritely.ControlFlow
{
    public static class Try
    {
        public static TryAction Running(Action f)
        {
            return new TryAction(f);
        }

        public static TryFunction<T> Running<T>(Func<T> f)
        {
            return new TryFunction<T>(f);
        }
    }
}
