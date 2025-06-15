using PixelEngine.Core.Initialization;
using UnityEngine;

namespace PixelEngine.Systems.ServiceLocator
{
    [AddComponentMenu("PixelEngine/Systems/ServiceLocator/Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureAsScene();
        }

        protected override void Shutdown()
        {
            Container.OnShutdown();
        }
    }
}