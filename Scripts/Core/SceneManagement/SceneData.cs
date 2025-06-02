using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        [SerializeField] 
        private ESceneType m_sceneType;
    
        [SerializeField] 
        private SceneReference m_scene;

        public ESceneType SceneType => m_sceneType;

        public SceneReference Scene => m_scene;
        public string Name => m_scene.Name;
    }

    [Serializable]
    public class SceneGroup
    {
        [SerializeField] 
        private ScriptableEnumSceneName m_groupName;
        
        [SerializeField] 
        private List<SceneData> m_scenes;
    
        public ScriptableEnumSceneName GroupName => m_groupName;
        public List<SceneData> Scenes => m_scenes;

        public string FindSceneNameByType(ESceneType sceneType) => m_scenes.Find(x => x.SceneType == sceneType).Name;
    }
}
