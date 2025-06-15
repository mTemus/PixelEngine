using UnityEngine;

namespace PixelEngine.Systems.ServiceLocator
{
    [AddComponentMenu("PixelEngine/Systems/ServiceLocator/Global Service Initializer")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal();
        }

        protected override void Shutdown()
        {
            Container.OnShutdown();
        }
    }
}