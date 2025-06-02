using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelEngine.Utility.Observer
{
    [Serializable]
    public class ListObserver<T> : IList<T>, IDisposable
    {
        [SerializeField]
        private List<T> m_values = new();

        [SerializeField]
        private UnityEvent<ListObserver<T>> m_onListChanged = new();

        public void AddChangedListener(UnityAction<ListObserver<T>> callback)
        {
            if (callback == null) 
                return;
        
            m_onListChanged.AddListener(callback);
        }

        public void RemoveChangedListener(UnityAction<ListObserver<T>> callback)
        {
            if (callback == null) 
                return;
        
            m_onListChanged.RemoveListener(callback);
        }

        private void NotifyChanged()
        {
            m_onListChanged?.Invoke(this);
        }

        public T this[int index]
        {
            get => m_values[index];
            set
            {
                m_values[index] = value;
                NotifyChanged();
            }
        }

        public int Count => m_values.Count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            m_values.Add(item);
            NotifyChanged();
        }

        public void Clear()
        {
            if (m_values.Count == 0) return;
            m_values.Clear();
            NotifyChanged();
        }

        public bool Contains(T item) => m_values.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => m_values.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => m_values.GetEnumerator();
        public int IndexOf(T item) => m_values.IndexOf(item);

        public void Insert(int index, T item)
        {
            m_values.Insert(index, item);
            NotifyChanged();
        }

        public bool Remove(T item)
        {
            var removed = m_values.Remove(item);
        
            if (removed) 
                NotifyChanged();
            return removed;
        }

        public void RemoveAt(int index)
        {
            m_values.RemoveAt(index);
            NotifyChanged();
        }

        IEnumerator IEnumerable.GetEnumerator() => m_values.GetEnumerator();

        public void Dispose()
        {
            m_onListChanged?.RemoveAllListeners();
            m_values.Clear();
        }
    }
}