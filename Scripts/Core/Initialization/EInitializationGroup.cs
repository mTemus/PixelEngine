using System;

namespace PixelEngine.Core.Initialization
{
    [Serializable]
    public enum EInitializationGroup
    {
        Default = 0,
        Core = 1,
        Systems = 2,
        UI = 3,
        Player = 4,
        Entities = 5,
        AI = 6,
        Environment = 7,
    }
}