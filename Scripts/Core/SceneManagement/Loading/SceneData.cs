using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement.Loading
{
    [Serializable]
    public class SceneData
    {
        [SerializeField] 
        private ESceneType m_sceneType;
    
        [SerializeField] 
        private SceneReference m_scene;

        [SerializeField] 
        private bool m_isInitializable;
        
        [SerializeField] 
        private bool m_isSavable;
        
        public ESceneType SceneType => m_sceneType;
        public SceneReference Scene => m_scene;
        public string Name => m_scene.Name;
        public bool IsInitializable => m_isInitializable;
        public bool IsSavable => m_isSavable;
    }

    [Serializable]
    public class SceneGroup
    {
        [SerializeField] 
        private ScriptableEnumSceneGroupName m_groupName;
        
        [SerializeField] 
        private List<SceneData> m_scenes;
        
        public ScriptableEnumSceneGroupName GroupName => m_groupName;
        public List<SceneData> Scenes => m_scenes;
        
        public string FindSceneNameByType(ESceneType sceneType) => m_scenes.Find(x => x.SceneType == sceneType).Name;
    }
}
