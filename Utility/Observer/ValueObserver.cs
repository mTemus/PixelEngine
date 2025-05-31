using System;
using System.Collections;
using System.Reflection;
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
        private UnityEvent<T> m_onValueChanged;

        [SerializeField] 
        private UnityEvent<T> m_onValueSet;

        public T Value
        {
            get => m_value;
            set => Set(value);
        }

        public static implicit operator T(ValueObserver<T> observer) => observer.m_value;

        public ValueObserver(T value, UnityAction<T> callback = null)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                throw new InvalidOperationException($"Type {typeof(T)} is a collection, which is not allowed.");
            
            m_value = value;
            m_onValueChanged = new UnityEvent<T>();
            
            if (callback != null) 
                m_onValueChanged.AddListener(callback);
        }

        private void Set(T value)
        {
            m_onValueSet.Invoke(value);
            
            if (Equals(m_value, value)) 
                return;
            
            m_value = value;
            Invoke();
        }

        public void Invoke()
        {
            m_onValueChanged.Invoke(m_value);
        }

        #region Value Changed

        public void AddChangedListener(UnityAction<T> callback)
        {
            if (callback == null) 
                return;
            
            if (m_onValueChanged == null) 
                m_onValueChanged = new UnityEvent<T>();

#if UNITY_EDITOR
            UnityEventTools.AddPersistentListener(m_onValueChanged, callback);
#else
        onValueChanged.AddListener(callback);
#endif
        }

        public void RemoveChangedListener(UnityAction<T> callback)
        {
            if (callback == null) 
                return;
            
            if (m_onValueChanged == null) 
                return;

#if UNITY_EDITOR
            UnityEventTools.RemovePersistentListener(m_onValueChanged, callback);
#else
        onValueChanged.RemoveListener(callback);
#endif
        }

        #endregion

        #region Value Set

        public void AddSetListener(UnityAction<T> callback)
        {
            if (callback == null) return;
            if (m_onValueSet == null) m_onValueSet = new UnityEvent<T>();

#if UNITY_EDITOR
            UnityEventTools.AddPersistentListener(m_onValueSet, callback);
#else
        m_onValueSet.AddListener(callback);
#endif
        }

        public void RemoveSetListener(UnityAction<T> callback)
        {
            if (callback == null) return;
            if (m_onValueSet == null) return;

#if UNITY_EDITOR
            UnityEventTools.RemovePersistentListener(m_onValueSet, callback);
#else
        m_onValueSet.RemoveListener(callback);
#endif
        }

        #endregion
        
        public void RemoveAllListeners()
        {
            if (m_onValueChanged == null) 
                return;

#if UNITY_EDITOR
            var fieldInfo = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
            var value = fieldInfo.GetValue(m_onValueChanged);
            value.GetType().GetMethod("Clear").Invoke(value, null);
#else
        onValueChanged.RemoveAllListeners();
#endif
            if (m_onValueSet == null)
                return;
            
#if UNITY_EDITOR
            fieldInfo = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
            value = fieldInfo.GetValue(m_onValueSet);
            value.GetType().GetMethod("Clear").Invoke(value, null);
#else
        onValueSet.RemoveAllListeners();
#endif      
            
        }

        public void Dispose()
        {
            RemoveAllListeners();
            m_onValueChanged = null;
            m_value = default;
        }
    }
}