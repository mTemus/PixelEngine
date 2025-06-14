using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Extensions
{
    public static class SceneExtensions
    {
        public static T GetComponentFromScene<T>(this Scene scene) where T : Component
        {
            var rootObjects = scene.GetRootGameObjects();

            for (var i = 0; i < rootObjects.Length; i++)
            {
                var component = rootObjects[i].GetComponentInChildren<T>();

                if (component != null)
                    return component;
            }

            return null;
        }
        
        public static bool TryGetComponent<T>(this Scene scene, out T component) where T : Component
        {
            var rootObjects = scene.GetRootGameObjects();

            for (var i = 0; i < rootObjects.Length; i++)
            {
                component = rootObjects[i].GetComponentInChildren<T>();

                if (component != null)
                    return true;
            }

            component = null;
            return false;
        }

        public static bool TryGetComponents<T>(this Scene scene, out List<T> components, bool includeInactive = false) where T : Component
        {
            components = new List<T>();

            if (!scene.IsValid() || !scene.isLoaded)
                return false;

            var rootObjects = scene.GetRootGameObjects();

            for (var i = 0; i < rootObjects.Length; i++)
                components.AddRange(rootObjects[i].GetComponentsInChildren<T>(includeInactive).Where(comp => comp != null));

            components = components.Where(comp => comp).ToList();
            return components.Count > 0;
        }
        
        public static bool TryGetInterfaceComponents<T>(this Scene scene, out List<T> components, bool includeInactive = false) where T : class
        {
            components = new List<T>();

            if (!scene.IsValid() || !scene.isLoaded)
                return false;

            var rootObjects = scene.GetRootGameObjects();

            foreach (var root in rootObjects)
            {
                var allComps = root.GetComponentsInChildren<Component>(includeInactive);
                
                foreach (var comp in allComps)
                    if (comp is T asInterface)
                        components.Add(asInterface);
            }

            return components.Count > 0;
        }
    }
}
