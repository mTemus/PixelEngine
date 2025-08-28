using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using UnityEngine;

namespace PixelEngine.Core.GameManagement.Context
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
        
        [SerializeField]
        //TODO: display this in the editor? (git-amend video)
        private List<GameContextProperty> m_gameContextProperties = new List<GameContextProperty>();
        
        //TODO: these should be in a save state!
        [HorizontalLine("Scenes Data", color: FixedColor.IceWhite)]
        [SerializeField]
        private List<SceneOpenContext> m_sceneOpenContexts = new List<SceneOpenContext>();
        
        //TODO: save state as string name or header or other
        
        public EGameMode GameMode => m_gameMode;
        
        public GameContext(EGameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        #region Scene Open Context

        /// <summary>
        /// Should be called before a scene collection is added to load queue
        /// </summary>
        /// <param name="sceneOpenContext"></param>
        public void AddSceneOpenContext(SceneOpenContext sceneOpenContext, bool overrideExisting = false)
        {
            var existingContext = m_sceneOpenContexts.FirstOrDefault(x => x.SceneName == sceneOpenContext.SceneName);
            
            if (!string.IsNullOrEmpty(existingContext.SceneName))
            {
                if (overrideExisting)
                {
                    m_sceneOpenContexts.Remove(existingContext);
                }
                else
                {
                    Debug.LogError($"Scene open context for scene: {sceneOpenContext.SceneName} is already registered!");
                    return;
                }
            }
            
            m_sceneOpenContexts.Add(sceneOpenContext);
        }

        /// <summary>
        /// Should be called when a scene collection is loaded and will be initialized
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public SceneOpenContext GetSceneOpenContext(string sceneName)
        {
            var sceneOpenContext = m_sceneOpenContexts.FirstOrDefault(x => x.SceneName == sceneName);

            if (sceneOpenContext.OpenMode == ESceneOpenMode.None)
                return new SceneOpenContext(sceneName, ESceneOpenMode.AsGameState);
            
            m_sceneOpenContexts.Remove(sceneOpenContext);
            return sceneOpenContext;
        }

        #endregion

        #region Properties

        public void AddProperty(GameContextProperty property)
        {
            if (m_gameContextProperties.Contains(property))
                return;
            
            m_gameContextProperties.Add(property);
        }
        
        public bool HasProperty(ScriptableEnumGameContextPropertyId id)
        {
            return m_gameContextProperties.Any(x => x.ID == id);
        }
        
        public GameContextProperty GetProperty(ScriptableEnumGameContextPropertyId id)
        {
            var property = m_gameContextProperties.FirstOrDefault(x => x.ID == id);
            
            if (property == null)
            {
                Debug.LogError($"Property with id: {id} not found!");
                return null;
            }

            return property;
        }

        #endregion
        
    }
}