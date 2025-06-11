using EditorAttributes;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    //TODO: forcing load game with a game state name to fill
    public class GameManager : MonoBehaviour
    {
        [Title("Variables", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField, Required] 
        private EGameModeVariable m_gameModeVariable;

        [Title("Editor Only", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField]
        private bool m_forceGameMode;
        
        [ShowField("m_forceGameMode")]
        [SerializeField]
        private EGameMode m_gameMode;
        
        public async void PrepareGame()
        {
#if UNITY_EDITOR
            m_gameModeVariable.Value = m_forceGameMode ? m_gameMode : EGameMode.Editor;
#else
            m_gameModeVariable.Value = EGameMode.MainMenu;            
#endif
            //TODO:
            // Core initialization
            
            
            
            
            
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
