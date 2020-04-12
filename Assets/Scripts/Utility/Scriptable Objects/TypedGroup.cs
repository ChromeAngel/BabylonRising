using System.Collections.Generic;
using UnityEngine;

public class TypedGroup<T> : ScriptableObject
{
    public Publication OnGroupChanged;
    public List<T> items = new List<T>();

    public void Add(T item)
    {
        if (items.Contains(item))
            return;

        items.Add(item);

        if (OnGroupChanged != null)
            OnGroupChanged.Publish();
    }
    public void Remove(T item)
    {
        if (!items.Contains(item))
            return;

        items.Remove(item);

        if (OnGroupChanged != null)
            OnGroupChanged.Publish();
    }
}