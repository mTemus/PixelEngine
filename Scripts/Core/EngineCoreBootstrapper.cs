using PixelEngine.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core
{
    public class EngineCoreBootstrapper
    {
        public const string CoreSceneName = "Core";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Init()
        {
#if UNITY_EDITOR
            var coreScene = SceneManager.GetSceneByName(CoreSceneName);
            
            if (coreScene.IsValid() && coreScene.isLoaded)
                return;
            
            Debug.Log("Bootstrapping game...");
            await SceneManager.LoadSceneAsync(CoreSceneName, LoadSceneMode.Single).AsTask();
#endif
        }
    }
}
