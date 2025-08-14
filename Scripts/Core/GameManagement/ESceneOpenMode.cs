using System;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public enum ESceneOpenMode
    {
        None = 0,
        AsNew = 1,
        AsLoaded = 2,
        AsGameState = 3
    }
}