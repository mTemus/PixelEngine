using System.Threading.Tasks;
using EditorAttributes;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.Initialization;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        [Title("References", alignment: TextAnchor.UpperCenter, titleSize: 20, titleSpace: 8, drawLine: true)]
        [SerializeField, Required]
        private SceneInitializationManager m_initialization;
        
        
        
        public async Task StartScene(EGameMode gameMode)
        {
            
            
            
            
            
            
            
        }

        public async Task StopUsingScene(EGameMode gameMode)
        {
            
        }
    }
}
