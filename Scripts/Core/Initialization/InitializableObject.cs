using UnityEngine;

namespace PixelEngine.Core.Initialization
{
    public class InitializableObject : MonoBehaviour
    {
        [SerializeField]
        private InitializationGroup m_group;

        public InitializationGroup Group => m_group;
        
        
        
        
        
        
    }
}