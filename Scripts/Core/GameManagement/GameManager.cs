using System;
using System.Threading.Tasks;
using EditorAttributes;
using PixelEngine.Core.Initialization;
using PixelEngine.Core.SceneManagement;
using PixelEngine.Core.SceneManagement.Loading;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    //TODO: forcing load game with a game state name to fill
    public class GameManager : MonoBehaviour
    {
        [Title("References", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField, Required]
        private GlobalInitializer m_globalInitializer;
        
        [SerializeField, Required]
        private SceneController m_coreSceneController;
        
        [SerializeField, Required]
        private SceneLoader m_sceneLoader;
        
        [Title("Variables", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField, Required] 
        private EGameModeVariable m_gameModeVariable;

        [Title("Editor Only", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField]
        private bool m_forceGameMode;
        
        [ShowField("m_forceGameMode")]
        [SerializeField]
        private EGameMode m_gameMode;
        
        public async Task PrepareGame()
        {
#if UNITY_EDITOR
            m_gameModeVariable.Value = m_forceGameMode ? m_gameMode : EGameMode.Editor;
#else
            m_gameModeVariable.Value = EGameMode.MainMenu;            
#endif
            //TODO:
            // Core initialization

            await m_coreSceneController.StartScene(EGameMode.NewGame);

            switch (m_gameModeVariable.Value)
            {
                case EGameMode.Editor:
#if UNITY_EDITOR
                    await m_globalInitializer.InitializeActiveScene();
#endif
                    break;
                
                case EGameMode.MainMenu:
                    await m_sceneLoader.LoadScene(ESceneType.MainMenu);
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
