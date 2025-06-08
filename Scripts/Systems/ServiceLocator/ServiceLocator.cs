using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

// https://www.youtube.com/watch?v=D4r5EyYQvwY
namespace PixelEngine.Systems.ServiceLocator
{
    //TODO: initialization
    //TODO: unregister
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator m_global;
        private static Dictionary<Scene, ServiceLocator> m_sceneContainers;
        private static List<GameObject> m_tmpSceneGameObjects;
        
        readonly ServiceManager m_services = new ServiceManager();

        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        #region Bootstrapping

        protected internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (m_global == this)
            {
                Debug.LogWarning("ServiceLocator --- This global ServiceLocator is already registered!", this);
            }
            else if (m_global != null)
            {
                Debug.LogError("ServiceLocator --- Another ServiceLocator is already registered as global!", this);
            }
            else
            {
                m_global = this;
                
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }
        
        protected internal void ConfigureAsScene()
        {
            var scene = gameObject.scene;
            
            if (m_sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator --- This scene ServiceLocator is already registered!", this);
                return;
            }
            
            m_sceneContainers[scene] = this;
        }

        //TODO: this to uninitialization
        private void OnDestroy()
        {
            if (m_global == this)
                m_global = null;
            else if (m_sceneContainers.ContainsValue(this))
                m_sceneContainers.Remove(gameObject.scene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            m_global = null;
            m_sceneContainers = new Dictionary<Scene, ServiceLocator>();
            m_tmpSceneGameObjects = new List<GameObject>();
        }
        
        #endregion
        
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

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) 
                return this;

            if (TryGetNextInHierarchy(out var container))
            {
                container.Get(out service);
                return container;
            }
            
            throw new ArgumentException($"ServiceLocator --- Service of type {typeof(T).FullName} was not registered!");
        }

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

        #region Editor Context

#if UNITY_EDITOR
        
        [MenuItem("PixelEngine/Systems/Service Locator/Add Global")]
        private static void AddGlobal() => new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper)); 
        
        [MenuItem("PixelEngine/Systems/Service Locator/Add Scene")]
        private static void AddScene() => new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));

#endif

        #endregion
    }
}