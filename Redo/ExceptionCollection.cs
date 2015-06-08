// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionCollection.cs">
//   Copyright (c) 2015. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Spritely.Redo
{
    /// <summary>
    ///     Container for holding types of exceptions.
    /// </summary>
    public class ExceptionCollection : IEnumerable<Type>
    {
        private static readonly object Lock = new object();
        private readonly ICollection<Type> exceptions = new List<Type>();

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Type> GetEnumerator()
        {
            return this.exceptions.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.exceptions.GetEnumerator();
        }

        /// <summary>
        ///     Adds the specified exception type.
        /// </summary>
        /// <typeparam name="T">The type of exception to add.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is a fluent interface where this is the simplest type-safe way to tell the library how to handle specific types.")]
        public void Add<T>() where T : Exception
        {
            var type = typeof(T);

            lock (Lock)
            {
                if (!this.exceptions.Contains(type))
                {
                    this.exceptions.Add(type);
                }
            }
        }

        /// <summary>
        ///     Adds the specified exception type.
        /// </summary>
        /// <param name="exception">The type of exception to add.</param>
        /// <exception cref="System.ArgumentException">Type t must be an Exception type.</exception>
        internal void Add(Type exception)
        {
            if (!typeof(Exception).IsAssignableFrom(exception))
            {
                throw new ArgumentException("Type must be derived from Exception", "exception");
            }

            lock (Lock)
            {
                if (!this.exceptions.Contains(exception))
                {
                    this.exceptions.Add(exception);
                }
            }
        }

        /// <summary>
        ///     Removes the specified exception type.
        /// </summary>
        /// <typeparam name="T">The type of exception to remove.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is a fluent interface where this is the simplest type-safe way to tell the library how to handle specific types.")]
        public void Remove<T>() where T : Exception
        {
            var type = typeof(T);

            lock (Lock)
            {
                if (this.exceptions.Contains(type))
                {
                    this.exceptions.Remove(type);
                }
            }
        }

        /// <summary>
        ///     Resets this instance to an empty list.
        /// </summary>
        public void Reset()
        {
            lock (Lock)
            {
                this.exceptions.Clear();
            }
        }
    }
}
