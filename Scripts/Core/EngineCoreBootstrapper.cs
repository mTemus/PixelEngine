using System;
using PixelEngine.Core.GameManagement;
using PixelEngine.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelEngine.Core
{
    public class EngineCoreBootstrapper
    {
        public const string CoreSceneName = "Core";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
#if UNITY_EDITOR
            var coreScene = SceneManager.GetSceneByName(CoreSceneName);

            if (coreScene.isLoaded)
            {
                StartCoreScene();
            }
            else
            {
                var activeScene = SceneManager.GetActiveScene();

                if (activeScene.isLoaded)
                {
                    var sceneCount = SceneManager.sceneCount;

                    if (sceneCount == 1)
                    {
                        LoadAndStartCoreScene();
                    }
                    else
                    {
                        SceneManager.sceneLoaded -= OnCoreLoaded;
                        SceneManager.sceneLoaded += OnCoreLoaded;
                    }
                }
            }
#else
            StartCoreScene();
#endif
        }

        private static void OnCoreLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name != CoreSceneName)
                return;
            
            SceneManager.sceneLoaded -= OnCoreLoaded;
            StartCoreScene();
        }

        private static async void LoadAndStartCoreScene()
        {
            Debug.Log("Bootstrapping game...");
            await SceneManager.LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive).AsTask();
            StartCoreScene();
        }
        
        private async static void StartCoreScene()
        {
            var coreScene = SceneManager.GetSceneByName(CoreSceneName);
            coreScene.TryGetComponent<GameManager>(out var gameManager);

            if (!gameManager)
                throw new MissingComponentException($"There is no GameManager on core scene: {CoreSceneName}/{coreScene.name}!");

            await gameManager.PrepareGame();
        }
    }
}
