using UnityEngine;
using UnityUtils;

namespace PixelEngine.Systems.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator m_container;
        
        protected internal ServiceLocator Container => m_container.OrNull() ?? (m_container = GetComponent<ServiceLocator>());

        private bool m_hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }

        public void BootstrapOnDemand()
        {
            if (m_hasBeenBootstrapped) return;
            m_hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
    
    [AddComponentMenu("PixelEngine/Systems/ServiceLocator/Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] private bool m_dontDestroyOnLoad = true;
        
        protected override void Bootstrap()
        {
            
        }
    }
    
    [AddComponentMenu("PixelEngine/Systems/ServiceLocator/Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            
        }
    }
}