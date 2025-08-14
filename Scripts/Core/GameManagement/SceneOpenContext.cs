using System;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public struct SceneOpenContext : IEquatable<SceneOpenContext>
    {
        public string SceneName { get; }
        public ESceneOpenMode OpenMode { get; }

        public SceneOpenContext(string sceneName) : this()
        {
            SceneName = sceneName;
            OpenMode = ESceneOpenMode.AsNew;
        }

        public SceneOpenContext(string sceneName, ESceneOpenMode openMode)
        {
            SceneName = sceneName;
            OpenMode = openMode;
        }

        public bool Equals(SceneOpenContext other)
        {
            return SceneName == other.SceneName;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneOpenContext other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SceneName, (int)OpenMode);
        }
    }
}