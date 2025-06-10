using System;
using PixelEngine.Core.Serialization.SerializeInterface;
using UnityEngine;

namespace PixelEngine.Core.Initialization
{
    [Serializable]
    public class InitializableComponent
    {
        [SerializeField] 
        private InterfaceReference<IInitializable> m_component;

        [Tooltip("Higher number then higher priority in group.")]
        [SerializeField, Range(0, 100)] private int m_priority;

        public IInitializable Component => m_component.Value;
        public int Priority => m_priority;

        public InitializableComponent(IInitializable initializableInterface, int priority = 0)
        {
            m_component = new InterfaceReference<IInitializable>() { Value = initializableInterface };
            m_priority = priority;
        }
    }
}