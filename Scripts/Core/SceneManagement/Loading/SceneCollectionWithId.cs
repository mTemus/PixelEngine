using System;
using AdvancedSceneManager.Models;

namespace PixelEngine.Core.SceneManagement.Loading
{
    [Serializable]
    public struct SceneCollectionWithId
    {
        public ScriptableEnumSceneCollectionName ID;
        public SceneCollection SceneCollection;
    }
}