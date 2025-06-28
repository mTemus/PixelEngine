using System;
using System.Threading.Tasks;
using CustomInspector;
using PixelEngine.Core.Initialization;
using PixelEngine.Core.SceneManagement;
using PixelEngine.Core.SceneManagement.Loading;
using PixelEngine.UI;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    //TODO: forcing load game with a game state name to fill
    public class GameManager : MonoBehaviour
    {
        [Tab("References")]
        [SerializeField]
        private GlobalInitializer m_globalInitializer;
        
        [Tab("References")]
        [SerializeField]
        private SceneController m_coreSceneController;
        
        [Tab("References")]
        [SerializeField]
        private SceneLoader m_sceneLoader;
        
        [Tab("References")]
        [SerializeField]
        private Blackscreen m_blackscreen;
        
        [Tab("Variables")]
        [SerializeField] 
        private EGameModeVariable m_gameModeVariable;
        
        [Tab("Editor Only")]
        [SerializeField]
        private bool m_forceGameMode;
        
        [Tab("Editor Only")]
        [ShowIf("m_forceGameMode")]
        [SerializeField]
        private EGameMode m_gameMode;
        
        public async Task PrepareGame()
        {
#if UNITY_EDITOR
            m_gameModeVariable.Value = m_forceGameMode ? m_gameMode : EGameMode.Editor;
#else
            m_gameModeVariable.Value = EGameMode.MainMenu;            
#endif
            var gameStartTask = m_coreSceneController.StartScene(EGameMode.NewGame);

            await gameStartTask;

            switch (m_gameModeVariable.Value)
            {
                case EGameMode.Editor:
#if UNITY_EDITOR
                    await m_globalInitializer.InitializeActiveScene();
#endif
                    break;
                
                case EGameMode.MainMenu:
                    m_blackscreen.Show(0f);
                    await m_sceneLoader.LoadScene(ESceneType.MainMenu);
                    m_blackscreen.Hide(0.3f);
                    break;
                
                case EGameMode.NewGame:
                    break;
                
                case EGameMode.LoadGame:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }


            // if Editor
            // 1. Some scene is loaded, so start initialization (as new) of the scene
            
            // if Main Menu
            // 1. Load Main Menu, initialize and let player decide if he wants a New Game or Load Game

            // if New Game
            // 1. Set GameModeVariable value to 'NewGame'
            // 2. Start new game cycle
            // -> Black screen ON
            // -> Unload Main Menu
            // -> Load first scene group
            // -> Run initialization of new scenes as new
            // -> Black screen OFF
            // Start game (unlock movement, start ticking, etc.)
            // Done.

            // if Load Game
            // 1. Set GameModeVariable value to 'LoadGame'
            // 2. Start load game cycle
            // -> Black screen ON
            // -> Unload Main Menu
            // -> Load group from save state
            // -> Run initialization of loaded scenes as loaded
            // -> Black screen OFF
            // Start game (unlock movement, start ticking, etc.)
            // Done.
        }









    }
}
