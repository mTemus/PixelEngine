using UnityEngine;

namespace PixelEngine.Systems.ServiceLocator
{
    [AddComponentMenu("PixelEngine/Systems/ServiceLocator/Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] private bool m_dontDestroyOnLoad = true;
        
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(m_dontDestroyOnLoad);
        }
    }
}