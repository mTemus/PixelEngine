using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PixelEngine.Systems.ServiceLocator
{
    public class ServicesRegistrar : MonoBehaviour
    {
        //TODO: initialization
        //TODO: unregister
        [SerializeField]
        private ERegistrarLevel m_registrarLevel = ERegistrarLevel.Global;
    
        [SerializeField]
        private List<Object> m_services = new List<Object>();

        private void Register()
        {
            ServiceLocator sl;
            
            switch (m_registrarLevel)
            {
                case ERegistrarLevel.Global:
                    sl = ServiceLocator.Global;
                    break;
            
                case ERegistrarLevel.Scene:
                    sl = ServiceLocator.ForSceneOf(this);
                    break;
            
                case ERegistrarLevel.Entity:
                    sl = ServiceLocator.ForEntity(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Register(sl);
        }

        private void Register(ServiceLocator sl)
        {
            foreach (var service in m_services)
            {
                sl.Register(service);

#if UNITY_EDITOR
                Debug.Log($"Registered service: {service.name} in {sl.name}");
#endif
            }
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
