using Obvious.Soap;
using PixelEngine.Core.SceneManagement.Loading;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement.Events
{
    [CreateAssetMenu(fileName = "x_ScriptableEvent" + nameof(SceneData), menuName = "PixelEngine/ScriptableEvents/Custom/" + nameof(SceneData))]
    public class ScriptableEventSceneData : ScriptableEvent<SceneData>
    {
    
    }
}
