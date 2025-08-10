using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedSceneManager.Models;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement
{
    [CreateAssetMenu(fileName = "x_SceneCollectionMetadata", menuName = "PixelEngine/Core/SceneManagement/Scene Collection Metadata")]
    public class SceneCollectionMetadata : ScriptableObject
    {
        [SerializeField] 
        private List<ScenePriorityMetadata> m_initializableScenes;
     
        [SerializeField]
        private List<ScenePriorityMetadata> m_savableScenes;

        public List<Scene> InitializableScenes => GetScenesAscendingPriority(m_initializableScenes);
        public List<Scene> SavableScenes => GetScenesAscendingPriority(m_savableScenes);
        
        private List<Scene> GetScenesAscendingPriority(List<ScenePriorityMetadata> scenes) => scenes
            .OrderBy(s => s.Priority)
            .Select(s => s.Scene)
            .ToList();
    }
    
    [Serializable]
    public struct ScenePriorityMetadata
    {
        public Scene Scene;
        
        [Range(0, 100)]
        public int Priority;
    }
}
