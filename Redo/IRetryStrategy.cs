// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryStrategy.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo
{
    public interface IRetryStrategy
    {
        bool ShouldQuit(long attempt);
        void Wait(long attampt);
    }
}
