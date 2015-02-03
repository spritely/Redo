// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryStrategy.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo
{
    /// <summary>
    ///     A strategy for retrying operations.
    /// </summary>
    public interface IRetryStrategy
    {
        /// <summary>
        ///     A retryable operation calls this method to determine if it should quit.
        /// </summary>
        /// <param name="attempt">The attempt.</param>
        /// <returns>true if operation should be cancelled, false otherwise.</returns>
        bool ShouldQuit(long attempt);

        /// <summary>
        ///     A retryable operation calls this method to introduce wait time before retrying again.
        /// </summary>
        /// <param name="attempt">The attempt.</param>
        void Wait(long attempt);
    }
}
