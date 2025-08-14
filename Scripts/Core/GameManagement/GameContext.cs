using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using UnityEngine;

namespace PixelEngine.Core.GameManagement
{
    [Serializable]
    public class GameContext
    {
#if UNITY_EDITOR
        [HorizontalLine("Editor Only Settings", color: FixedColor.IceWhite)]
        [SerializeField]
        private bool m_forceGameMode;
        
        public bool ForceGameMode => m_forceGameMode;
#endif
        
        [HorizontalLine("Core Properties", color: FixedColor.IceWhite)]
        [SerializeField]
        private EGameMode m_gameMode;
        
        //TODO: these should be in a save state!
        [HorizontalLine("Scenes Data", color: FixedColor.IceWhite)]
        [SerializeField]
        private List<VisitedScene> m_visitedScenes = new List<VisitedScene>();
        
        [SerializeField]
        private List<SceneOpenContext> m_sceneOpenContexts = new List<SceneOpenContext>();
        
        //TODO: save state as string name or header or other
        
        public EGameMode GameMode => m_gameMode;
        
        public GameContext(EGameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        #region Scene Open Context

        public void AddSceneOpenContext(SceneOpenContext sceneOpenContext)
        {
            if (m_sceneOpenContexts.Contains(sceneOpenContext))
            {
                Debug.LogError($"Scene open context for scene: {sceneOpenContext.SceneName} is already registered!");
                return;
            }
            
            m_sceneOpenContexts.Add(sceneOpenContext);
        }

        public SceneOpenContext GetSceneOpenContext(string sceneName)
        {
            var sceneOpenContext = m_sceneOpenContexts.FirstOrDefault(x => x.SceneName == sceneName);

            if (sceneOpenContext.OpenMode == ESceneOpenMode.None)
                return new SceneOpenContext(sceneName, ESceneOpenMode.AsGameState);
            
            m_sceneOpenContexts.Remove(sceneOpenContext);
            return sceneOpenContext;
        }

        #endregion

        #region Visited Scenes

        public void VisitScene(string sceneName)
        {
            if (m_visitedScenes.Any(s => s.SceneName == sceneName))
                return;
            
            m_visitedScenes.Add(new VisitedScene(sceneName));
        }
        
        public void VisitScene(string sceneName, Time exitTimeStamp)
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

        #endregion
    }
}