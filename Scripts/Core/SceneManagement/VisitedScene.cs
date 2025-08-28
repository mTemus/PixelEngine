using System;

namespace PixelEngine.Core.SceneManagement
{
    [Serializable]
    public struct VisitedScene
    {
        public string SceneName { get; }
        public long ExitTimeStamp { get; set; }

        public VisitedScene(string sceneName) : this()
        {
            SceneName = sceneName;
        }

        public VisitedScene(string sceneName, long exitTimeStamp)
        {
            SceneName = sceneName;
            ExitTimeStamp = exitTimeStamp;
        }
        
        public TimeSpan GetTimeAway()
        {
            var lastExit = new DateTime(ExitTimeStamp, DateTimeKind.Utc);
            return DateTime.UtcNow - lastExit;
        }
    }
}