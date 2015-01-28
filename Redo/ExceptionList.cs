using System;
using System.Collections;
using System.Collections.Generic;

namespace Spritely.Redo
{
    public class ExceptionList : IEnumerable<Type>
    {
        private readonly ICollection<Type> exceptions = new List<Type>();

        public IEnumerator<Type> GetEnumerator()
        {
            return this.exceptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.exceptions.GetEnumerator();
        }

        public void Add<T>() where T : Exception
        {
            this.exceptions.Add(typeof (T));
        }

        public void Add(Type t)
        {
            if (!typeof (Exception).IsAssignableFrom(t))
            {
                throw new ArgumentException("Type t must be an Exception type");
            }

            this.exceptions.Add(t);
        }

        public void Remove<T>() where T : Exception
        {
            this.exceptions.Remove(typeof (T));
        }

        public void Reset()
        {
            this.exceptions.Clear();
        }
    }
}
