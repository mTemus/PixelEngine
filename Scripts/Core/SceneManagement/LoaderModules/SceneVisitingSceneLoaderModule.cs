using System;
using AdvancedSceneManager.Models;
using Core.Attributes;
using CustomInspector;
using PixelEngine.Core.GameManagement;
using PixelEngine.Core.GameManagement.Context;
using PixelEngine.Core.Initialization;
using PixelEngine.Core.SceneManagement.Events;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement.Loading.Modules
{
    public class SceneVisitingSceneLoaderModule : MonoBehaviour, IInitializable
    {
        [SerializeField, ForceFill, AssetsOnly, CreateEditableAsset]
        private GameContextVariable m_gameContext;
        
        [SerializeField, ForceFill, AssetsOnly, CreateEditableAsset]
        private ScriptableEnumGameContextPropertyId m_visitedScenesPropertyId;
        
        [SerializeField, ForceFill, AssetsOnly, CreateEditableAsset]
        private ScriptableEventSceneCollection m_sceneCollectionPreLoadEvent;
        
        [SerializeField, ForceFill, AssetsOnly, CreateEditableAsset]
        private ScriptableEventSceneCollection m_sceneCollectionPreUnloadEvent;

        [SerializeField] 
        private bool m_saveExitTime = true;

        #region Initialization

        public void EarlyInitialize()
        {
            m_sceneCollectionPreUnloadEvent.OnRaised += OnSceneCollectionPreUnload;
            m_sceneCollectionPreLoadEvent.OnRaised += OnSceneCollectionPreLoad;
        }

        public void LateInitialize()
        {
            if (!m_gameContext.Value.HasProperty(m_visitedScenesPropertyId))
                m_gameContext.Value.AddProperty(new VisitedScenesGameContextProperty(m_visitedScenesPropertyId));
        }

        public void Uninitialize()
        {
            m_sceneCollectionPreUnloadEvent.OnRaised -= OnSceneCollectionPreUnload;
            m_sceneCollectionPreLoadEvent.OnRaised -= OnSceneCollectionPreLoad;
        }

        #endregion

        private void OnSceneCollectionPreUnload(SceneCollection collection)
        {
            var metadata = collection.UserData<SceneCollectionMetadata>();
            
            if (metadata == null || metadata.VisitableScenes.Count == 0)
                return;
            
            var property = m_gameContext.Value.GetProperty(m_visitedScenesPropertyId) as VisitedScenesGameContextProperty;
            
            foreach (var scene in metadata.VisitableScenes)
            {
                if (m_saveExitTime)
                    property.VisitScene(scene.name, DateTime.Now.Ticks);    
                else
                    property.VisitScene(scene.name);
            }
        }

        private void OnSceneCollectionPreLoad(SceneCollection collection)
        {
            var metadata = collection.UserData<SceneCollectionMetadata>();
            
            if (metadata == null || metadata.VisitableScenes.Count == 0)
                return;
            
            var property = m_gameContext.Value.GetProperty(m_visitedScenesPropertyId) as VisitedScenesGameContextProperty;

            foreach (var scene in metadata.VisitableScenes)
            {
                if (!property.HasVisitedScene(scene.name))
                    continue;

                m_gameContext.Value.AddSceneOpenContext(new SceneOpenContext(scene.name, ESceneOpenMode.AsLoaded), true);
            }
        }
    }
}