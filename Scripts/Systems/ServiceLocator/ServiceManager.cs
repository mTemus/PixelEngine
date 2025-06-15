using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelEngine.Systems.ServiceLocator
{
    public class ServiceManager
    {
        private readonly Dictionary<Type, object> m_services = new Dictionary<Type, object>();
        public IEnumerable<object> RegisteredServices => m_services.Values;

        #region Get

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            
            if (m_services.TryGetValue(type, out var serviceObj))
                return serviceObj as T;
            
            throw new ArgumentException($"ServiceManager --- Service of type {type.FullName} was not registered!");
        }
        
        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);

            if (m_services.TryGetValue(type, out object serviceObj))
            {
                service = serviceObj as T;
                return true;
            }
            
            service = null;
            return false;
        }

        #endregion
        
        #region Add

        public ServiceManager Register<T>(T service)
        {
            var type = typeof(T);

            return TryAddService(type, service);
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
                throw new ArgumentException($"ServiceManager --- Service of type {type.FullName} is not assignable from {service.GetType().FullName}!()");
            
            return TryAddService(type, service);
        }

        private ServiceManager TryAddService(Type type, object service)
        {
            if (!m_services.TryAdd(type, service))
                Debug.LogError($"ServiceManager --- Service of type {type.FullName} is already registered!()");
            
            
#if UNITY_EDITOR
            Debug.Log($"ServiceManager --- Service of type {service.GetType().Name} added!");
#endif
            return this;
        }
        
        #endregion

        #region Remove
        
        public ServiceManager Unregister<T>()  where T : class
        {
            var serviceType = typeof(T);
            return Unregister(serviceType);
        }
        
        public ServiceManager Unregister(Type serviceType)
        {
            if (!m_services.ContainsKey(serviceType))
            {
                Debug.LogError($"ServiceManager --- Service of type {serviceType.Name} is not registered!");
                return this;
            }
            
            if (!TryRemoveService(serviceType))
            {
                Debug.LogError($"ServiceManager --- Service of type {serviceType.Name} could not be removed!");
                return this;
            }

#if UNITY_EDITOR
            Debug.Log($"ServiceManager --- Service of type {serviceType.Name} removed!");
#endif
            return this;
        }
        
        private bool TryRemoveService(Type type)
        {
            return m_services.Remove(type);
        }

        #endregion
    }
}
