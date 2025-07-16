using System;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public struct GameContext
    {
        public EGameMode GameMode;
        
        //TODO: scene group name/id
        //TODO: save state as string name or header or other
    }
}