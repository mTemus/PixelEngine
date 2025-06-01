using System;
using System.Collections;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace PixelEngine.Utility.Observer
{
    [Serializable]
    public class ValueObserver<T>
    {
        [SerializeField] 
        private T m_value;

        [SerializeField]
        private T m_previousValue;

        [SerializeField] 
        private UnityEvent<ValueObserver<T>> m_onValueChanged;

        [SerializeField] 
        private UnityEvent<ValueObserver<T>> m_onValueSet;
        
        public T Value
        {
            get => m_value;
            set => Set(value);
        }

        public T PreviousValue => m_previousValue;

        public static implicit operator T(ValueObserver<T> observer) => observer.m_value;

        public ValueObserver(T value, UnityAction<ValueObserver<T>> callback = null)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                throw new InvalidOperationException($"Type {typeof(T)} is a collection, which is not allowed.");
            
            m_value = value;
            m_onValueChanged = new UnityEvent<ValueObserver<T>>();
            
            if (callback != null) 
                m_onValueChanged.AddListener(callback);
        }

        private void Set(T value)
        {
            m_onValueSet.Invoke(this);
            
            if (Equals(m_value, value)) 
                return;
            
            m_previousValue = m_value;
            m_value = value;
            Invoke();
        }

        public void Invoke()
        {
            m_onValueChanged.Invoke(this);
        }

        #region Value Changed

        public void AddChangedListener(UnityAction<ValueObserver<T>> callback)
        {
            if (callback == null) 
                return;
            
            if (m_onValueChanged == null) 
                m_onValueChanged = new UnityEvent<ValueObserver<T>>();

            UnityEventTools.AddPersistentListener(m_onValueChanged, callback);

            m_onValueChanged.AddListener(callback);
        }

        public void RemoveChangedListener(UnityAction<ValueObserver<T>> callback)
        {
            if (callback == null) 
                return;
            
            if (m_onValueChanged == null) 
                return;

            UnityEventTools.RemovePersistentListener(m_onValueChanged, callback);

            m_onValueChanged.RemoveListener(callback);
        }

        #endregion

        #region Value Set

        public void AddSetListener(UnityAction<ValueObserver<T>> callback)
        {
            m_onValueSet ??= new UnityEvent<ValueObserver<T>>();
            
            UnityEventTools.AddPersistentListener(m_onValueSet, callback);

            m_onValueSet.AddListener(callback);
        }

        public void RemoveSetListener(UnityAction<ValueObserver<T>> callback)
        {
            m_onValueSet?.RemoveListener(callback);
        }

        #endregion
        
        public void RemoveAllListeners()
        {
            m_onValueChanged?.RemoveAllListeners();
            m_onValueSet?.RemoveAllListeners();
        }

        public void Dispose()
        {
            RemoveAllListeners();
            m_onValueChanged = null;
            m_onValueSet = null;
            m_value = default;
            m_previousValue = default;
        }
    }
}