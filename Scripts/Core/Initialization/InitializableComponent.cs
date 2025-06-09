using System;
using PixelEngine.Core.Serialization.SerializeInterface;
using UnityEngine;

namespace PixelEngine.Core.Initialization
{
    [Serializable]
    public class InitializableComponent
    {
        [SerializeField] 
        [Range(0, 100)] 
        private int m_priority;

        [SerializeField] 
        private InterfaceReference<IInitializable> m_initializable;

        public int Priority => m_priority;
        public IInitializable Component 
        {
            get
            {
                if (m_initializable == null || m_initializable.Value == null)
                    return null;
                
                return m_initializable.Value;
            }    
        }

        public InitializableComponent(IInitializable initializable)
        {
            m_initializable = new InterfaceReference<IInitializable> { Value = initializable };
        }

        public override bool Equals(object obj)
        {
            if (obj is not InitializableComponent other)
                return false;
            
            return Component == other.Component;
        }

        public override int GetHashCode()
        {
            return Component.GetHashCode();
        }
    }
}