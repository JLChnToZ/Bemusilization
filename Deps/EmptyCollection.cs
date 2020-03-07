using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    /// <summary>
    /// A dummy class can pretend to be an empty immutable list/collection/enumerable/enumerator/set (both generic and non-generic).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EmptyCollection<T>: IReadOnlyList<T>, ISet<T>, IList<T>, IList, IEnumerator<T> {
        /// <summary>
        /// Default instance of an empty collection.
        /// </summary>
        public static EmptyCollection<T> Default { get; } = new EmptyCollection<T>();

        int ICollection<T>.Count => 0;

        int IReadOnlyCollection<T>.Count => 0;

        int ICollection.Count => 0;

        bool ICollection<T>.IsReadOnly => true;

        bool IList.IsReadOnly => true;

        bool IList.IsFixedSize => true;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => new object();

        T IList<T>.this[int index] {
            get => throw new IndexOutOfRangeException();
            set => throw new NotSupportedException();
        }

        T IReadOnlyList<T>.this[int index] =>
            throw new IndexOutOfRangeException();

        object IList.this[int index] {
            get => throw new IndexOutOfRangeException();
            set => throw new NotSupportedException();
        }

        T IEnumerator<T>.Current =>
            throw new InvalidOperationException();

        object IEnumerator.Current =>
            throw new InvalidOperationException();

        private EmptyCollection() { }

        void ICollection<T>.Add(T item) =>
            throw new NotSupportedException();

        int IList.Add(object value) =>
            throw new NotSupportedException();

        bool ISet<T>.Add(T item) => false;

        void IList<T>.Insert(int index, T item) =>
            throw new NotSupportedException();

        void IList.Insert(int index, object value) =>
            throw new NotSupportedException();

        void ISet<T>.UnionWith(IEnumerable<T> other) =>
            throw new NotSupportedException();

        void ISet<T>.ExceptWith(IEnumerable<T> other) {
            throw new NotImplementedException();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other) =>
            throw new NotSupportedException();

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) =>
            throw new NotSupportedException();

        bool ICollection<T>.Contains(T item) => false;

        bool IList.Contains(object value) => false;

        int IList<T>.IndexOf(T item) => -1;

        int IList.IndexOf(object value) => -1;

        bool ISet<T>.IsSubsetOf(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : true;

        bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : IsEmpty(other);

        bool ISet<T>.IsSupersetOf(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : false;

        bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : false;

        bool ISet<T>.Overlaps(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : true;

        bool ISet<T>.SetEquals(IEnumerable<T> other) =>
            other == null ? throw new ArgumentNullException(nameof(other)) : IsEmpty(other);

        void IList.Remove(object value) =>
            throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => false;

        void IList<T>.RemoveAt(int index) =>
            throw new NotSupportedException();

        void IList.RemoveAt(int index) =>
            throw new NotSupportedException();

        void ICollection<T>.Clear() =>
            throw new NotSupportedException();

        void IList.Clear() =>
            throw new NotSupportedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            if(array == null)
                throw new ArgumentNullException(nameof(array));
            if(arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        void ICollection.CopyTo(Array array, int index) {
            if(array == null)
                throw new ArgumentNullException(nameof(array));
            if(index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }

        void IDisposable.Dispose() { }

        /// <summary>
        /// Check if the enumerable is empty.
        /// </summary>
        /// <param name="enumerable">The enumerable object to be checked.</param>
        /// <returns><c>true</c> if enumerable is empty, otherwise <c>false</c>.</returns>
        public static bool IsEmpty(IEnumerable<T> enumerable) {
            if(enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if(enumerable is EmptyCollection<T>)
                return true;
            if(enumerable is Array array)
                return array.Length == 0;
            if(enumerable is ICollection<T> typedCollection)
                return typedCollection.Count == 0;
            if(enumerable is IReadOnlyCollection<T> readonlyCollection)
                return readonlyCollection.Count == 0;
            if(enumerable is ICollection collection)
                return collection.Count == 0;
            using(var enumerator = enumerable.GetEnumerator())
                return !enumerator.MoveNext();
        }
    }
}
