using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    public class ExtendedSortedSet<T>: ICollection<T>, IReadOnlyCollection<T>, ISet<T>, ICollection {
        private readonly SortedSet<T> sortedSet;
        private readonly WrappedComparer comparer;

        public int Count => sortedSet.Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized =>
            (sortedSet as ICollection).IsSynchronized;

        object ICollection.SyncRoot =>
            (sortedSet as ICollection).SyncRoot;

        public IEqualityComparer<T> EqualityComparer =>
            comparer.baseEqualityComparer;

        public IComparer<T> Comparer =>
            comparer.baseComparer;

        public ExtendedSortedSet() :
            this(null, null, null) { }

        public ExtendedSortedSet(IComparer<T> comparer) :
            this(null, comparer, null) { }

        public ExtendedSortedSet(IEqualityComparer<T> equalityComparer) :
            this(null, null, equalityComparer) { }

        public ExtendedSortedSet(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer) :
            this(collection, null, equalityComparer) { }

        public ExtendedSortedSet(IEnumerable<T> collection, IComparer<T> comparer = null, IEqualityComparer<T> equalityComparer = null) {
            this.comparer = new WrappedComparer(comparer, equalityComparer) {
                insertMode = InsertMode.OldFirst,
            };
            sortedSet = new SortedSet<T>(
                collection != null ?
                new WrappedEnumereable(this.comparer, collection) as IEnumerable<T> :
                EmptyCollection<T>.Default, this.comparer);
        }

        public bool Add(T item) =>
            Add(item, 0);

        public bool Add(T item, InsertMode insertMode) {
            comparer.insertMode = insertMode;
            return sortedSet.Add(comparer.newEntry = item);
        }

        void ICollection<T>.Add(T item) {
            comparer.insertMode = InsertMode.Restricted;
            (sortedSet as ICollection<T>).Add(item);
        }

        public void Clear() =>
            sortedSet.Clear();

        public bool Contains(T item) =>
            sortedSet.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            sortedSet.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            sortedSet.GetEnumerator();

        public bool Remove(T item) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            sortedSet.GetEnumerator();

        public void UnionWith(IEnumerable<T> other) =>
            UnionWith(other, InsertMode.Restricted);

        public void UnionWith(IEnumerable<T> other, InsertMode insertMode) =>
            sortedSet.UnionWith(EnsureWrapped(other, insertMode));

        public void ExceptWith(IEnumerable<T> other) =>
            ExceptWith(other, InsertMode.Restricted);

        public void ExceptWith(IEnumerable<T> other, InsertMode insertMode) =>
            sortedSet.ExceptWith(EnsureWrapped(other, insertMode));

        public void IntersectWith(IEnumerable<T> other) =>
            IntersectWith(other, InsertMode.Restricted);

        public void IntersectWith(IEnumerable<T> other, InsertMode insertMode) =>
            sortedSet.IntersectWith(EnsureWrapped(other, insertMode));

        public void SymmetricExceptWith(IEnumerable<T> other) =>
            SymmetricExceptWith(other, InsertMode.Restricted);

        public void SymmetricExceptWith(IEnumerable<T> other, InsertMode insertMode) =>
            sortedSet.SymmetricExceptWith(EnsureWrapped(other, insertMode));

        private IEnumerable<T> EnsureWrapped(IEnumerable<T> other, InsertMode insertMode) =>
            insertMode == InsertMode.Restricted ? other : new WrappedEnumereable(comparer, other);

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other) {
            comparer.insertMode = InsertMode.Free;
            return sortedSet.SetEquals(other);
        }

        void ICollection.CopyTo(Array array, int index) =>
            (sortedSet as ICollection).CopyTo(array, index);

        private class WrappedComparer: IComparer<T> {
            public readonly IComparer<T> baseComparer;
            public readonly IEqualityComparer<T> baseEqualityComparer;
            public InsertMode insertMode;
            public T newEntry;

            public WrappedComparer(IComparer<T> baseComparer, IEqualityComparer<T> baseEqualityComparer) {
                this.baseComparer = baseComparer ?? Comparer<T>.Default;
                this.baseEqualityComparer = baseEqualityComparer ?? EqualityComparer<T>.Default;
            }

            public int Compare(T x, T y) {
                if(baseEqualityComparer.Equals(x, y))
                    return 0;
                int comparation = baseComparer.Compare(x, y);
                if(comparation != 0)
                    return comparation;
                switch(insertMode) {
                    case InsertMode.Free:
                        return -1;
                    case InsertMode.NewFirst:
                        if(baseEqualityComparer.Equals(x, newEntry))
                            return 1;
                        if(baseEqualityComparer.Equals(y, newEntry))
                            return -1;
                        break;
                    case InsertMode.OldFirst:
                        if(baseEqualityComparer.Equals(x, newEntry))
                            return -1;
                        if(baseEqualityComparer.Equals(y, newEntry))
                            return 1;
                        break;
                }
                return 0;
            }
        }

        private struct WrappedEnumereable: IEnumerable<T> {
            private readonly IEnumerable<T> baseEnumerable;
            private readonly WrappedComparer wrappedComparer;

            public WrappedEnumereable(WrappedComparer wrappedComparer, IEnumerable<T> baseEnumerable) {
                this.wrappedComparer = wrappedComparer;
                this.baseEnumerable = baseEnumerable;
            }

            public IEnumerator<T> GetEnumerator() =>
                new WrappedEnumerator(wrappedComparer, baseEnumerable.GetEnumerator());

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private struct WrappedEnumerator: IEnumerator<T> {
            private readonly IEnumerator<T> baseEnumerator;
            private readonly WrappedComparer wrappedComparer;

            public WrappedEnumerator(WrappedComparer wrappedComparer, IEnumerator<T> baseEnumerator) {
                this.wrappedComparer = wrappedComparer;
                this.baseEnumerator = baseEnumerator;
            }

            public T Current =>
                wrappedComparer.newEntry = baseEnumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose() => baseEnumerator.Dispose();

            public bool MoveNext() => baseEnumerator.MoveNext();

            public void Reset() => baseEnumerator.Reset();
        }
    }

    public enum InsertMode {
        Restricted,
        Free,
        NewFirst,
        OldFirst,
    }
}
