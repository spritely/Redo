// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Delegates.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    /// <summary>
    ///     Interface for reporting exceptions that occur during retry handling.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public delegate void ExceptionListener(Exception exception);
}
