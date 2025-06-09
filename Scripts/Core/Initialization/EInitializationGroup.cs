using System;

namespace PixelEngine.Core.Initialization
{
    [Serializable]
    public enum EInitializationGroup
    {
        Core = 1,
        Systems = 2,
        UI = 3,
        Player = 4,
        Entities = 5,
        AI = 6,
        Environment = 7,
    }
}