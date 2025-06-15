using System;
using System.Collections.Generic;
using PixelEngine.Core.Initialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PixelEngine.Systems.ServiceLocator
{
    public class ServicesRegistrar : MonoBehaviour, IInitializable
    {
        //TODO: initialization
        //TODO: unregister
        [SerializeField]
        private ERegistrarLevel m_registrarLevel = ERegistrarLevel.Global;
    
        [SerializeField]
        private List<Object> m_services = new List<Object>();

        #region Initialization

        public void EarlyInitialize()
        {
            Register();
        }

        public void Uninitialize()
        {
            Unregister();
        }

        #endregion
        
        private void Register()
        {
            var locator = GetServiceLocator();
            
            foreach (var service in m_services)
            {
                locator.Register(service);

#if UNITY_EDITOR
                Debug.Log($"Registered service: {service.name} in {locator.name}");
#endif
            }
        }

        private void Unregister()
        {
            var locator = GetServiceLocator();
            
            foreach (var service in m_services)
            {
                locator.Unregister(service.GetType());

#if UNITY_EDITOR
                Debug.Log($"Registered service: {service.name} in {locator.name}");
#endif
            }
        }

        private ServiceLocator GetServiceLocator()
        {
            return m_registrarLevel switch
            {
                ERegistrarLevel.Global => ServiceLocator.GlobalContainer,
                ERegistrarLevel.Scene => ServiceLocator.ForSceneOf(this),
                ERegistrarLevel.Entity => ServiceLocator.ForEntity(this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        [Serializable]
        public enum ERegistrarLevel
        {
            Global = 0,
            Scene = 1,
            Entity = 2
        }
    }
}
