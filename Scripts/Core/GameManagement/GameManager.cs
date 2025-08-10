using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using PixelEngine.Core.Initialization;
using PixelEngine.Core.SceneManagement;
using PixelEngine.Core.SceneManagement.Loading;
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
        private SceneLoaderASMWrapper m_sceneLoader;
        
        [Tab("Variables")]
        [SerializeField] 
        private EGameModeVariable m_gameModeVariable;
        
        [Tab("Variables")]
        [SerializeField, ForceFill, AssetsOnly]
        private ScriptableEnumSceneCollectionName m_mainMenuSceneCollectionName;
        
        [Tab("Editor Only")]
        [SerializeField]
        private bool m_forceGameMode;
        
        [Tab("Editor Only")]
        [ShowIf("m_forceGameMode")]
        [SerializeField]
        private EGameMode m_gameMode;
        
        [SerializeField]
        private List<SceneCollectionWithId> m_sceneCollections = new List<SceneCollectionWithId>();
        
        [Tab("Debug")]
        [SerializeField]
        private GameContext m_gameContext;

        private void Start()
        {
            try
            {
                PrepareGame();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to start game. GameManager: {e.Message} | {e.StackTrace}");
            }
        }
        
        public void PrepareGame()
        {
#if UNITY_EDITOR
            m_gameModeVariable.Value = m_forceGameMode ? m_gameMode : EGameMode.Editor;
#else
            m_gameModeVariable.Value = EGameMode.MainMenu;            
#endif
            m_coreSceneController.StartScene(EGameMode.NewGame);

            switch (m_gameModeVariable.Value)
            {
                case EGameMode.Editor:
#if UNITY_EDITOR
                    m_globalInitializer.InitializeActiveScene();
#endif
                    break;
                
                case EGameMode.MainMenu:
                    var mainMenuScene = m_sceneCollections.First(sc => sc.ID == m_mainMenuSceneCollectionName);
                    m_sceneLoader.LoadSceneCollection(mainMenuScene.SceneCollection);
                    //TODO: wait for scene initialized?
                    
                    break;

                //TODO: 
                case EGameMode.NewGame:
                    break;
                    
                case EGameMode.LoadGame:
                    //TODO: continue or game state
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartGame(GameContext gameContext)
        {
            m_gameContext = gameContext;
            m_gameModeVariable.Value = gameContext.GameMode;
            
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

            // if Load Game / Continue
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
