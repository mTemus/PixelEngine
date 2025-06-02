using UnityEngine;

namespace PixelEngine.Utility.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static bool HasInstance => instance != null;

        public static T Instance
        {
            get
            {
                if (instance != null) 
                    return instance;
                
                instance = FindAnyObjectByType<T>();
                
                if (instance != null) 
                    return instance;
                
                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                instance = go.AddComponent<T>();

                return instance;
            }
        }

        public static T TryGetInstance() => HasInstance ? instance : null;

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) 
                return;

            instance = this as T;
        }
    }
}