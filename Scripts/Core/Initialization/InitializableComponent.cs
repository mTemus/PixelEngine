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
        
        public InitializableComponent(int priority, IInitializable initializable)
        {
            m_priority = priority;
            m_initializable = new InterfaceReference<IInitializable> { Value = initializable };
        }
    }
}