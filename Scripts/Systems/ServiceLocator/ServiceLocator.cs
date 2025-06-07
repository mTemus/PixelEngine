using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

// https://www.youtube.com/watch?v=D4r5EyYQvwY
namespace PixelEngine.Systems.ServiceLocator
{
    //TODO: initialization
    //TODO: unregister
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator m_global;
        private static Dictionary<Scene, ServiceLocator> m_sceneContainers;

        readonly ServiceManager m_services = new ServiceManager();

        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        #region Locators

        public static ServiceLocator Global
        {
            get
            {
                if (m_global)
                    return m_global;

                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return m_global;
                }


                var container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return m_global;
            }
        }

        public static ServiceLocator ForEntity(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
        }

        private static List<GameObject> m_tmpSceneGameObjects;

        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;

            if (m_sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
                return container;

            Debug.Log($"ServiceLocator --- Container for scene {scene.name} is not registered. Finding manually.");

            m_tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(m_tmpSceneGameObjects);

            foreach (var go in m_tmpSceneGameObjects.Where(go =>
                         go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if (!go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) || bootstrapper == null)
                    continue;

                bootstrapper.BootstrapOnDemand();
                return bootstrapper.Container;
            }

            return Global;
        }

        #endregion

        #region Register

        public ServiceLocator Register<T>(T service)
        {
            m_services.Register<T>(service);
            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            m_services.Register(type, service);
            return this;
        } 

        #endregion

        #region Get

        private bool TryGetService<T>(out T service) where T : class
        {
            return m_services.TryGet(out service);
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == m_global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        #endregion
    }
}