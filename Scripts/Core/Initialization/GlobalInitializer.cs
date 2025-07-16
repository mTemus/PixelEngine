using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomInspector;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.SceneManagement;
using PixelEngine.Core.SceneManagement.Events;
using PixelEngine.Core.SceneManagement.Loading;
using PixelEngine.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: unitask
namespace PixelEngine.Core.Initialization
{
    public class GlobalInitializer : MonoBehaviour, IInitializable
    {
        [Tab("Variables")]
        [SerializeField] 
        private EGameModeVariable m_gameModeVariable;
        
        [Tab("Events")]
        [SerializeField]
        private ScriptableEventSceneGroup m_sceneGroupLoadedEvent;
        
        [Tab("Events")]
        [SerializeField]
        private ScriptableEventSceneGroup m_sceneGroupPreUnloadedEvent;

        [Tab("Events")]
        [SerializeField]
        private ScriptableEventSceneData m_sceneLoadedEvent;
        
        [Tab("Events")]
        [SerializeField]
        private ScriptableEventSceneData m_scenePreUnloadedEvent;

        #region Initialization

        public void EarlyInitialize()
        {
            m_sceneGroupLoadedEvent.OnRaised += OnSceneGroupLoaded;
            m_sceneGroupPreUnloadedEvent.OnRaised += OnSceneGroupPreUnloaded;
            m_sceneLoadedEvent.OnRaised += OnSceneLoaded;
            m_scenePreUnloadedEvent.OnRaised += OnScenePreUnloaded;
        }

        public void Uninitialize()
        {
            m_sceneGroupLoadedEvent.OnRaised -= OnSceneGroupLoaded;
            m_sceneGroupPreUnloadedEvent.OnRaised -= OnSceneGroupPreUnloaded;
            m_sceneLoadedEvent.OnRaised -= OnSceneLoaded;
            m_scenePreUnloadedEvent.OnRaised -= OnScenePreUnloaded;
        }
        
        #endregion

        #region Scene Loaded

        private async void OnSceneGroupLoaded(SceneGroup sceneGroup)
        {
            var scenes = new List<SceneData>(sceneGroup.Scenes);
            
            for (var i = 0; i < scenes.Count; i++)
            {
                var sceneData = scenes[i];

                await TryToInitializeScene(sceneData); 

                if (sceneData.SceneType == ESceneType.Gameplay)
                    SceneManager.SetActiveScene(sceneData.Scene.LoadedScene);
            }
        }

        private async void OnSceneLoaded(SceneData sceneData)
        {
            await TryToInitializeScene(sceneData); 
        }

        #endregion

        #region Scene Unloaded

        private async void OnSceneGroupPreUnloaded(SceneGroup sceneGroup)
        {
            var scenes = new List<SceneData>(sceneGroup.Scenes);

            for (var i = 0; i < scenes.Count; i++)
            {
                var sceneData = scenes[i];

                await TryToInitializeScene(sceneData);
            }
        }

        private async void OnScenePreUnloaded(SceneData sceneData)
        {
            await UninitializeScene(sceneData);
        }

        #endregion

        private async Task TryToInitializeScene(SceneData sceneData)
        {
            if (!sceneData.IsInitializable)
                return;

            var scene = sceneData.Scene.LoadedScene;

            if (scene.isDirty || !scene.isLoaded)
            {
#if UNITY_EDITOR
                Debug.LogError($"GlobalInitializer --- Trying to initialize dirty or not loaded scene: {scene.name}!");
#endif
                return;
            }

            if (scene.TryGetComponent<SceneController>(out var sceneController))
                await sceneController.StartScene(m_gameModeVariable.Value);
            else
                throw new Exception($"GlobalInitializer --- Scene {scene.name} is marked as initializable but doesn't have a scene controller!");
        }
        
        private async Task UninitializeScene(SceneData sceneData)
        {
            if (!sceneData.IsInitializable)
                return;

            if (sceneData.Scene.LoadedScene.TryGetComponent<SceneController>(out var sceneController))
                await sceneController.StopUsingScene(m_gameModeVariable.Value);
            else
                throw new Exception($"GlobalInitializer --- Scene {sceneData.Scene.Name} is marked as initializable but doesn't have a scene controller!");
        }

#if UNITY_EDITOR
        public async Task InitializeActiveScene()
        {
            var scene = SceneManager.GetActiveScene(); 
            
            if (scene.TryGetComponent<SceneController>(out var sceneController))
                await sceneController.StartScene(m_gameModeVariable.Value);
            else
                throw new Exception($"GlobalInitializer --- Scene {scene.name} is an active scene but doesn't have a scene controller!");
        }
#endif
    }
}