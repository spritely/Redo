﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryOperation.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Spritely.Redo
{
    /// <summary>
    ///     Base class for all Try... classes.
    /// </summary>
    /// <typeparam name="T">The type of operation (must be the type of the child class).</typeparam>
    public class TryOperation<T> where T : TryOperation<T>
    {
        internal TryConfiguration configuration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TryOperation{T}" /> class.
        /// </summary>
        public TryOperation()
        {
            this.configuration = TryDefault.NewConfiguration();
        }

        /// <summary>
        ///     Specifies the retry strategy for this operation.
        /// </summary>
        /// <param name="retryStrategy">The retry strategy.</param>
        /// <returns>This instance for chaining.</returns>
        public T With(IRetryStrategy retryStrategy)
        {
            this.configuration.RetryStrategy = retryStrategy;

            return this as T;
        }

        /// <summary>
        ///     Adds an exception logger to which reports will be published.
        /// </summary>
        /// <param name="exceptionLogger">The exception logger.</param>
        /// <returns>This instance for chaining.</returns>
        public T Report(ExceptionListener exceptionLogger)
        {
            this.configuration.ExceptionListeners += exceptionLogger;

            return this as T;
        }

        /// <summary>
        ///     Specifies an exception type to be handled. If none are specified all will be caught.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to handle.</typeparam>
        /// <returns>This instance for chaining.</returns>
        public T Handle<TException>() where TException : Exception
        {
            this.configuration.Handles.Add<TException>();

            return this as T;
        }
    }
}