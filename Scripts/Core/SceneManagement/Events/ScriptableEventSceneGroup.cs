using Obvious.Soap;
using PixelEngine.Core.SceneManagement.Loading;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement.Events
{
    [CreateAssetMenu(fileName = "x_ScriptableEvent" + nameof(SceneGroup), menuName = "PixelEngine/ScriptableEvents/Custom/"+ nameof(SceneGroup))]
    public class ScriptableEventSceneGroup : ScriptableEvent<SceneGroup>
    {
    
    }
}