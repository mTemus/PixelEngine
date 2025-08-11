using System;
using CustomInspector;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.Initialization;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        [Tab("References")]
        [SerializeField]
        private SceneInitializationManager m_initialization;
        
        private bool m_initializationDone;
        private bool m_loadingDone;
        
        public bool SceneIsReady => m_initializationDone && m_loadingDone;
        
        public void StartScene(EGameMode gameMode)
        {
            if (SceneIsReady)
            {
                Debug.LogError($"Trying to start scene that is already initialized: {gameObject.scene.name}!");
                return;
            }
            
#if UNITY_EDITOR
            Debug.Log($"Starting scene {gameObject.scene.name} in mode: {gameMode}.");
#endif
            
            m_initialization.StartInitialization();
            
            switch (gameMode)
            {
                case EGameMode.Editor:
                case EGameMode.MainMenu:
                case EGameMode.NewGame:
                    m_loadingDone = true;
                    m_initialization.InitializeAsNew();
                    m_initializationDone = true;
                    break;
                
                case EGameMode.LoadGame:
                    //TODO: loading here
                    m_loadingDone = true;
                    m_initialization.InitializeAsLoaded();
                    m_initializationDone = true;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        public void FocusOnScene()
        {
            
        }
        
        public void StopUsingScene(EGameMode gameMode)
        {
            m_initialization.Uninitialize();
        }
    }
}
