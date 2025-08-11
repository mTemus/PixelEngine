using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedSceneManager.Models;
using CustomInspector;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.SceneManagement;
using PixelEngine.Core.SceneManagement.Events;
using PixelEngine.Core.SceneManagement.Loading;
using PixelEngine.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = AdvancedSceneManager.Models.Scene;

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
        private ScriptableEventSceneCollection m_sceneCollectionLoadedEvent;
        
        [Tab("Events")]
        [SerializeField]
        private ScriptableEventSceneCollection m_sceneCollectionPreUnloadedEvent;

        [Tab("Events")]
        [SerializeField]
        private ScriptableEventScene m_sceneLoadedEvent;
        
        [Tab("Events")]
        [SerializeField]
        private ScriptableEventScene m_scenePreUnloadedEvent;

        #region Initialization

        public void EarlyInitialize()
        {
            m_sceneCollectionLoadedEvent.OnRaised += OnSceneCollectionOpened;
            m_sceneCollectionPreUnloadedEvent.OnRaised += OnSceneCollectionPreUnload;
            
            m_sceneLoadedEvent.OnRaised += OnSceneLoaded;
            m_scenePreUnloadedEvent.OnRaised += OnScenePreUnload;
        }

        

        public void Uninitialize()
        {
            m_sceneCollectionLoadedEvent.OnRaised -= OnSceneCollectionOpened;
            m_sceneCollectionPreUnloadedEvent.OnRaised -= OnSceneCollectionPreUnload;
        }

        #endregion

        #region Scene Collection

        private void OnSceneCollectionOpened(SceneCollection sceneCollection)
        {
            var collectionData = sceneCollection.UserData<SceneCollectionMetadata>();

            if (collectionData == null)
            {
                Debug.LogError($"A collection without metadata was opened: {sceneCollection.name}!");
                return;
            }
            
            var initializableScenes = collectionData.InitializableScenes;

            if (initializableScenes.Count == 0)
                return;
            
            for (var i = 0; i < initializableScenes.Count; i++)
                TryToInitializeScene(initializableScenes[i]);
        }

        private  void OnSceneCollectionPreUnload(SceneCollection sceneCollection)
        {
            var collectionData = sceneCollection.UserData<SceneCollectionMetadata>();

            if (collectionData == null)
            {
                Debug.LogError($"A collection without metadata was opened: {sceneCollection.name}!");
                return;
            }
            
            var initializableScenes = collectionData.InitializableScenes;

            if (initializableScenes.Count == 0)
                return;
            
            for (var i = 0; i < initializableScenes.Count; i++)
                TryToUninitializeScene(initializableScenes[i]);
        }

        #endregion

        #region Single Scene

        private void OnSceneLoaded(Scene scene)
        {
            TryToInitializeScene(scene);
        }

        private void OnScenePreUnload(Scene scene)
        {
            TryToUninitializeScene(scene);
        }

        #endregion

        #region Initialization Logic

        private void TryToInitializeScene(Scene asmScene)
        {
            if (!asmScene.internalScene.HasValue)
            {
                Debug.LogError($"Trying to initialize scene without internal scene: {asmScene.name}!");
                return;
            }
            
            var scene = asmScene.internalScene.Value;

            if (scene.isDirty || !scene.isLoaded)
            {
#if UNITY_EDITOR
                Debug.LogError($"GlobalInitializer --- Trying to initialize dirty or not loaded scene: {scene.name}!");
#endif
                return;
            }

            if (scene.TryGetComponent<SceneController>(out var sceneController))
                sceneController.StartScene(m_gameModeVariable.Value);
        }

        private void TryToUninitializeScene(Scene asmScene)
        {
            if (!asmScene.internalScene.HasValue)
            {
                Debug.LogError($"Trying to uninitialize scene without internal scene: {asmScene.name}!");
                return;
            }
            
            var scene = asmScene.internalScene.Value;

            if (scene.isDirty || !scene.isLoaded)
            {
#if UNITY_EDITOR
                Debug.LogError($"GlobalInitializer --- Trying to uninitialize dirty or not loaded scene: {scene.name}!");
#endif
                return;
            }

            if (scene.TryGetComponent<SceneController>(out var sceneController))
                sceneController.StopUsingScene(m_gameModeVariable.Value);
        }
        
        #endregion

#if UNITY_EDITOR
        public void InitializeActiveScene()
        {
            var scene = SceneManager.GetActiveScene();

            if (scene.name.Contains("Fallback") || scene.name.Contains("Core") || scene.name.Contains("Splash"))
                return;
            
            if (scene.TryGetComponent<SceneController>(out var sceneController))
                sceneController.StartScene(m_gameModeVariable.Value);
            else
                throw new Exception($"GlobalInitializer --- Scene {scene.name} is an active scene but doesn't have a scene controller!");
        }
#endif
    }
}