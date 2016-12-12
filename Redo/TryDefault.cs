﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TryDefault.cs">
//   Copyright (c) 2016. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Spritely.Redo source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Spritely.Redo
{
    using System;
    using System.Linq;
    using Spritely.Redo.Internal;

    /// <summary>
    ///     Container for all try operation default configuration options.
    /// </summary>
#if !SpritelyRecipesProject
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Spritely.Recipes", "See package version number")]
#pragma warning disable 0436
#endif
    public static partial class TryDefault
    {
        internal static readonly object _Lock = new object();
        internal static ExceptionCollection _handles;
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
                lock (_Lock)
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
                lock (_Lock)
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
                lock (_Lock)
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
                lock (_Lock)
                {
                    exceptionListeners = value ?? (exceptionListeners = ex => { });
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The constructor should guarantee same behavior as he Reset method.")]
        static TryDefault()
        {
            Reset();
        }

        /// <summary>
        ///     Adds a default exception handler for new retry operations.
        /// </summary>
        /// <typeparam name="T">The type of exception to handle.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is a fluent interface where this is the simplest type-safe way to tell the library how to handle specific types.")]
        public static void AddHandle<T>() where T : Exception
        {
            lock (_Lock)
            {
                _handles.Add<T>();
            }
        }

        /// <summary>
        ///     Removes a default exception handler for new retry operations.
        /// </summary>
        /// <typeparam name="T">The type of exception to remove.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is a fluent interface where this is the simplest type-safe way to tell the library how to handle specific types.")]
        public static void RemoveHandle<T>() where T : Exception
        {
            lock (_Lock)
            {
                _handles.Remove<T>();
            }
        }

        /// <summary>
        ///     Resets the default exception handlers for new operations to an empty list which if left unspecified will result in
        ///     all exceptions being handled.
        /// </summary>
        public static void ResetHandles()
        {
            lock (_Lock)
            {
                _handles.Reset();
            }
        }

        /// <summary>
        ///     Resets all defaults back to their pre-defined values.
        /// </summary>
        public static void Reset()
        {
            lock (_Lock)
            {
                retryStrategy = new ConstantDelayRetryStrategy();
                exceptionListeners = ex => { };
                _handles = new ExceptionCollection();
                maxRetries = 30;
                delay = TimeSpan.FromSeconds(1);
            }
        }
    }
#if !SpritelyRecipesProject
#pragma warning restore 0436
#endif
}
