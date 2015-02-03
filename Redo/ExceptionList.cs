// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionList.cs">
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
    public class ExceptionList : IEnumerable<Type>
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
        /// <param name="t">The type of exception to add.</param>
        /// <exception cref="System.ArgumentException">Type t must be an Exception type.</exception>
        public void Add(Type t)
        {
            if (!typeof(Exception).IsAssignableFrom(t))
            {
                throw new ArgumentException("Type t must be an Exception type");
            }

            lock (Lock)
            {
                if (!this.exceptions.Contains(t))
                {
                    this.exceptions.Add(t);
                }
            }
        }

        /// <summary>
        ///     Removes the specified exception type.
        /// </summary>
        /// <typeparam name="T">The type of exception to remove.</typeparam>
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
