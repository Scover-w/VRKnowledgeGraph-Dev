using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyHashSet<T> : IEnumerable<T>
{
    private HashSet<T> _hashSet;

    public ReadOnlyHashSet(HashSet<T> hashSet)
    {
        _hashSet = hashSet ?? throw new ArgumentNullException(nameof(hashSet));
    }

    public int Count => _hashSet.Count;

    public bool Contains(T item)
    {
        return _hashSet.Contains(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _hashSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _hashSet.GetEnumerator();
    }
}
