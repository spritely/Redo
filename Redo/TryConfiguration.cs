// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryConfiguration.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    internal class TryConfiguration
    {
        public IRetryStrategy RetryStrategy { get; set; }
        public ExceptionListener ExceptionListeners { get; set; }
        public ExceptionCollection Handles { get; set; }

        public void Report(Exception ex)
        {
            this.ExceptionListeners(ex);
        }
    }
}
