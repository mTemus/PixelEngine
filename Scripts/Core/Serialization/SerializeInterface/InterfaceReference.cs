using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PixelEngine.Core.Serialization.SerializeInterface
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject> where TObject : Object where TInterface : class
    {
        [SerializeField, HideInInspector] 
        private TObject m_underlyingValue;

        public TInterface Value
        {
            get
            {
                if (m_underlyingValue == null)
                    return null;
                
                if (m_underlyingValue is TInterface @interface)
                    return @interface;
                
                // There are sometimes edge cases where the value is null but still tries to pass through so null need to be returned
                Debug.LogError($"{m_underlyingValue}/{m_underlyingValue?.GetType()} does not implement {typeof(TInterface)}");
                return null;
            }
            set => m_underlyingValue = value switch
            {
                null => null,
                TObject newValue => newValue,
                _ => throw new ArgumentException($"{value} needs to be of type {typeof(TObject)}.", string.Empty)
            };
        }

        public TObject UnderlyingValue
        {
            get => m_underlyingValue;
            set => m_underlyingValue = value;
        }

        public InterfaceReference() { }

        public InterfaceReference(TObject target) => m_underlyingValue = target;

        public InterfaceReference(TInterface @interface) => m_underlyingValue = @interface as TObject;

        public static implicit operator TInterface(InterfaceReference<TInterface, TObject> obj) => obj.Value;
    }

    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }
}