using System;
using System.Collections.Generic;
using System.Linq;
using PixelEngine.Core.SceneManagement;
using UnityEngine;

namespace PixelEngine.Core.GameManagement.Context
{
    [Serializable]
    public class VisitedScenesGameContextProperty : GameContextProperty
    {
        [SerializeField]
        private List<VisitedScene> m_visitedScenes = new List<VisitedScene>();
        
        public VisitedScenesGameContextProperty(ScriptableEnumGameContextPropertyId id) : base(id) { }
        
        #region Visited Scenes

        public void VisitScene(string sceneName)
        {
            if (m_visitedScenes.Any(s => s.SceneName == sceneName))
                return;
            
            m_visitedScenes.Add(new VisitedScene(sceneName));
        }
        
        public void VisitScene(string sceneName, long exitTimeStamp)
        {
            if (m_visitedScenes.Any(s => s.SceneName == sceneName))
            {
                for (var i = 0; i < m_visitedScenes.Count; i++)
                {
                    if (m_visitedScenes[i].SceneName != sceneName)
                        continue;
                    
                    var data = m_visitedScenes[i];
                    data.ExitTimeStamp = exitTimeStamp;
                    m_visitedScenes[i] = data;
                    break;
                }
                
                return;
            }
            
            m_visitedScenes.Add(new VisitedScene(sceneName, exitTimeStamp));
        }
        
        public bool HasVisitedScene(string sceneName)
        {
            return m_visitedScenes.Any(s => s.SceneName == sceneName);
        }

        #endregion
    }
}