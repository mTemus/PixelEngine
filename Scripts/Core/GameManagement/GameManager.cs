using System;
using System.Collections.Generic;
using AdvancedSceneManager.Models;
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
        [Tab("Game")]
        [SerializeField]
        private GameContext m_gameContext;
        
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
        private GameContextVariable m_gameContextVariable;
        
        [Tab("Editor Only")]
        [SerializeField]
        private bool m_forceGameMode;
        
        [Tab("Editor Only")]
        [ShowIf("m_forceGameMode")]
        [SerializeField]
        private EGameMode m_gameMode;
        
        [Tab("Scenes")]
        [SerializeField, ForceFill, AssetsOnly]
        private SceneCollection m_mainMenuSceneCollection;

        [Tab("Scenes")] 
        [SerializeField, ForceFill, AssetsOnly]
        private ListContainer<SceneCollection> m_additiveGameplayScenes;
        
        [Tab("Scenes")]
        [SerializeField]
        private ListContainer<SceneCollectionWithId> m_gameplaySceneCollections = new List<SceneCollectionWithId>();
        
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
            m_gameContextVariable.Value = m_gameContext;
            
#if UNITY_EDITOR
            if (!m_gameContext.ForceGameMode)
                m_gameContext = new GameContext(EGameMode.Editor);
#else
            m_gameContext = new GameContext(EGameMode.MainMenu);           
#endif
            m_coreSceneController.StartScene(EGameMode.NewGame);

            switch (m_gameContext.GameMode)
            {
                case EGameMode.Editor:
#if UNITY_EDITOR
                    m_globalInitializer.InitializeActiveScene();
#endif
                    break;
                
                case EGameMode.MainMenu:
                    m_sceneLoader.LoadSceneCollection(m_mainMenuSceneCollection);
                    //TODO: wait for scene initialized?
                    
                    break;

                //TODO: 
                case EGameMode.NewGame:

                    for (var i = 0; i < m_additiveGameplayScenes.Count; i++)
                        m_sceneLoader.LoadSceneCollection(m_additiveGameplayScenes[i], true);
                    
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
