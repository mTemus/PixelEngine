using EditorAttributes;
using PixelEngine.Core.Initialization;
using UnityEngine;
using UnityUtils;

namespace PixelEngine.Systems.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour, IInitializable
    {
        private ServiceLocator m_container;
        
        protected internal ServiceLocator Container => m_container.OrNull() ?? (m_container = GetComponent<ServiceLocator>());

        [SerializeField, ReadOnly]
        private bool m_hasBeenBootstrapped;

        public void EarlyInitialize()
        {
            BootstrapOnDemand();
        }

        public void Uninitialize()
        {
            Shutdown();
        }

        public void BootstrapOnDemand()
        {
            if (m_hasBeenBootstrapped) return;
            m_hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
        protected abstract void Shutdown();
    }
}