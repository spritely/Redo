using System;

namespace Spritely.Redo
{
    internal class SafeDelay
    {
        internal static TimeSpan ConstrainBounds(double delay, double minimumBounds = 1,
            double maximumBounds = double.MaxValue)
        {
            if (minimumBounds > maximumBounds)
            {
                throw new ArgumentException("minimumBounds cannot be greater than maximumBounds");
            }

            TimeSpan boundedTimeSpan;
            try
            {
                boundedTimeSpan = TimeSpan.FromMilliseconds(delay);
            }
            catch (OverflowException)
            {
                boundedTimeSpan = TimeSpan.MaxValue;
            }

            var lowBounded = Math.Max(minimumBounds, boundedTimeSpan.TotalMilliseconds);
            var bounded = Math.Min(maximumBounds, lowBounded);

            // This throws overflow exception: TimeSpan.FromMilliseconds(TimeSpan.MaxValue.TotalMilliseconds);
            // Therefore, must first construct the max value differently
            if (Math.Abs(TimeSpan.MaxValue.TotalMilliseconds - bounded) < 0.1)
            {
                return TimeSpan.MaxValue;
            }

            boundedTimeSpan = TimeSpan.FromMilliseconds(bounded);

            return boundedTimeSpan;
        }
    }
}
