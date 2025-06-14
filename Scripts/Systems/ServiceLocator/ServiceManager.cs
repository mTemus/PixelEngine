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
            
            return this;
        }

        #endregion
    }
}
