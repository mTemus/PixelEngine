using System;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public struct VisitedScene
    {
        public string SceneName { get; }
        public Time ExitTimeStamp { get; set; }

        public VisitedScene(string sceneName) : this()
        {
            SceneName = sceneName;
        }

        public VisitedScene(string sceneName, Time exitTimeStamp)
        {
            SceneName = sceneName;
            ExitTimeStamp = exitTimeStamp;
        }
    }
}