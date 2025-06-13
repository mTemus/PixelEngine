using System;
using System.Threading.Tasks;
using EditorAttributes;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.Initialization;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        [Title("References", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField, Required]
        private SceneInitializationManager m_initialization;
        
        private bool m_initializationDone;
        private bool m_loadingDone;
        
        public bool SceneIsReady => m_initializationDone && m_loadingDone;
        
        public async Task StartScene(EGameMode gameMode)
        {
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
            
            await Task.Yield();
        }

        public void FocusOnScene()
        {
            
        }
        
        public async Task StopUsingScene(EGameMode gameMode)
        {
            m_initialization.Uninitialize();
            
            await Task.Yield();
        }
    }
}
