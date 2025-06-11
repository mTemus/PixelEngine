using PixelEngine.Core.GameManagement;
using PixelEngine.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core
{
    public class EngineCoreBootstrapper
    {
        public const string CoreSceneName = "Core";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
#if UNITY_EDITOR
            var coreScene = SceneManager.GetSceneByName(CoreSceneName);
            
            if (coreScene.IsValid() && coreScene.isLoaded)
                StartCoreScene();
            else
                LoadAndStartCoreScene();
#else
            LoadAndStartCoreScene();
#endif
        }

        private static async void LoadAndStartCoreScene()
        {
            Debug.Log("Bootstrapping game...");
            await SceneManager.LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive).AsTask();
            StartCoreScene();
        }
        
        private static void StartCoreScene()
        {
            var coreScene = SceneManager.GetSceneByName(CoreSceneName);
            coreScene.TryGetComponent<GameManager>(out var gameManager);

            if (!gameManager)
                throw new MissingComponentException($"There is no GameManager on core scene: {CoreSceneName}/{coreScene.name}!");

            gameManager.PrepareGame();
        }
    }
}
