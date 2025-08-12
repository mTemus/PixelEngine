using System;
using CustomInspector;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public class GameContext
    {
#if UNITY_EDITOR
        [HorizontalLine("Editor Only Settings", color: FixedColor.IceWhite)]
        [SerializeField]
        private bool m_forceGameMode;
        
        public bool ForceGameMode => m_forceGameMode;
#endif
        
        [HorizontalLine("Core Properties", color: FixedColor.IceWhite)]
        [SerializeField]
        private EGameMode m_gameMode;
        
        public EGameMode GameMode => m_gameMode;

        public GameContext(EGameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        //TODO: scene group name/id
        //TODO: save state as string name or header or other
    }
}