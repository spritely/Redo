// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryDefault.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Spritely.Redo
{
    /// <summary>
    ///     Container for all try operation default configuration options.
    /// </summary>
    public static class TryDefault
    {
        private static readonly object Lock = new object();
        private static ExceptionList handles;
        private static IRetryStrategy retryStrategy;
        private static ExceptionListener exceptionListeners;
        private static TimeSpan delay;
        private static int maxRetries;

        /// <summary>
        ///     Gets or sets the default delay for new retry operations.
        /// </summary>
        /// <value>
        ///     The default delay.
        /// </value>
        public static TimeSpan Delay
        {
            get { return delay; }
            set
            {
                lock (Lock)
                {
                    delay = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the default maximum retries for new retry operations.
        /// </summary>
        /// <value>
        ///     The default maximum retries.
        /// </value>
        public static int MaxRetries
        {
            get { return maxRetries; }
            set
            {
                lock (Lock)
                {
                    maxRetries = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the default retry strategy for new retry operations.
        /// </summary>
        /// <value>
        ///     The retry strategy.
        /// </value>
        public static IRetryStrategy RetryStrategy
        {
            get { return retryStrategy; }
            set
            {
                lock (Lock)
                {
                    retryStrategy = value ?? new ConstantDelayRetryStrategy();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the default exception listeners for new retry operations.
        /// </summary>
        /// <value>
        ///     The default exception listeners.
        /// </value>
        public static ExceptionListener ExceptionListeners
        {
            get { return exceptionListeners; }
            set
            {
                lock (Lock)
                {
                    exceptionListeners = value ?? (exceptionListeners = ex => { });
                }
            }
        }

        static TryDefault()
        {
            Reset();
        }

        /// <summary>
        ///     Adds a default exception handler for new retry operations.
        /// </summary>
        /// <typeparam name="T">The type of exception to handle.</typeparam>
        public static void AddHandle<T>() where T : Exception
        {
            lock (Lock)
            {
                handles.Add<T>();
            }
        }

        /// <summary>
        ///     Removes a default exception handler for new retry operations.
        /// </summary>
        /// <typeparam name="T">The type of exception to remove.</typeparam>
        public static void RemoveHandle<T>() where T : Exception
        {
            lock (Lock)
            {
                handles.Remove<T>();
            }
        }

        /// <summary>
        ///     Resets the default exception handlers for new operations to an empty list which if left unspecified will result in
        ///     all exceptions being handled.
        /// </summary>
        public static void ResetHandles()
        {
            lock (Lock)
            {
                handles.Reset();
            }
        }

        /// <summary>
        ///     Resets all defaults back to their pre-defined values.
        /// </summary>
        public static void Reset()
        {
            lock (Lock)
            {
                retryStrategy = new ConstantDelayRetryStrategy();
                exceptionListeners = ex => { };
                handles = new ExceptionList();
                maxRetries = 30;
                delay = TimeSpan.FromSeconds(1);
            }
        }

        internal static TryConfiguration NewConfiguration()
        {
            TryConfiguration configuration;
            lock (Lock)
            {
                configuration = new TryConfiguration
                {
                    ExceptionListeners = ExceptionListeners,
                    Handles = new ExceptionList(),
                    RetryStrategy = RetryStrategy
                };

                handles.ToList().ForEach(h => configuration.Handles.Add(h));
            }

            return configuration;
        }
    }
}
