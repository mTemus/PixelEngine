using System.Linq;
using AdvancedSceneManager;
using AdvancedSceneManager.Callbacks.Events;
using AdvancedSceneManager.Models;
using CustomInspector;
using PixelEngine.Core.Initialization;
using PixelEngine.Core.SceneManagement.Events;
using UnityEngine;

namespace PixelEngine.Core.SceneManagement.Loading
{
    //TODO: UniTask/R3
    public class SceneLoaderASMWrapper : MonoBehaviour, IInitializable
    {
        [Tab("Settings")]
        [SerializeField] 
        [Range(100, 5000)]
        private int m_sceneOperationMillisecondsDelay = 100;

        [Tab("Scene Events")] 
        [SerializeField]
        private ScriptableEventScene m_scenePreloadedEvent;
        
        [Tab("Scene Events")] 
        [SerializeField]
        private ScriptableEventScene m_sceneLoadedEvent;

        [Tab("Scene Events")] 
        [SerializeField]
        private ScriptableEventScene m_sceneUnloadedEvent;
        
        [Tab("Scene Events")] 
        [SerializeField]
        private ScriptableEventScene m_scenePreUnloadedEvent;

        [Tab("Scene Collection Events")] 
        [SerializeField]
        private ScriptableEventSceneCollection m_sceneCollectionPreLoadEvent;
        
        [Tab("Scene Collection Events")] 
        [SerializeField]
        private ScriptableEventSceneCollection m_sceneCollectionLoadedEvent;
        
        [Tab("Scene Collection Events")] 
        [SerializeField]
        private ScriptableEventSceneCollection m_sceneCollectionPreUnloadedEvent;

        public void EarlyInitialize()
        {
            SceneManager.runtime.RegisterCallback<CollectionOpenEvent>(OnCollectionOpened);
            SceneManager.runtime.collectionClosed += OnCollectionClosed;
            SceneManager.runtime.scenePreloaded += OnScenePreloaded;
        }

        public void Uninitialize()
        {
            SceneManager.runtime.UnregisterCallback<CollectionOpenEvent>(OnCollectionOpened);
            SceneManager.runtime.collectionClosed -= OnCollectionClosed;
            SceneManager.runtime.scenePreloaded -= OnScenePreloaded;
        }

        #region Collections

        private void OnCollectionOpened(CollectionOpenEvent @event)
        {
            m_sceneCollectionLoadedEvent.Raise(@event.collection);
        }

        private void OnCollectionClosed(SceneCollection collection)
        {
            
        }

        #endregion

        #region Single Scenes

        private void OnScenePreloaded(Scene scene)
        {
            m_scenePreloadedEvent.Raise(scene);
        }

        //Called when opening a scene from a collection
        private void OnSceneOpened(Scene scene)
        {
            m_sceneLoadedEvent.Raise(scene);
        }

        //Called when opening a scene from a collection
        private void OnSceneClosed(Scene scene)
        {
            m_sceneUnloadedEvent.Raise(scene);
        }

        #endregion

        #region Public API

        public void LoadSceneCollection(SceneCollection collection, bool additive = false)
        {
            if (additive)
            {
                m_sceneCollectionPreLoadEvent.Raise(collection);
                SceneManager.runtime.OpenAdditive(collection);
            }
            else
            {
                var currentCollection = SceneManager.runtime.openCollection;

                if (currentCollection != null)
                {
                    m_sceneCollectionPreUnloadedEvent.Raise(currentCollection);
                    SceneManager.runtime.Close(currentCollection);    
                }
            
                m_sceneCollectionPreLoadEvent.Raise(collection);
                SceneManager.runtime.Open(collection);    
            }
        }
        
        public void UnloadAdditiveSceneCollection(SceneCollection collection)
        {
            var additiveCollection = SceneManager.runtime.openAdditiveCollections.FirstOrDefault(openedCollection => openedCollection == collection);
            
            if (additiveCollection == null)
            {
                Debug.LogError($"Additive collection {collection.name} is not opened!");
                return;
            }
            
            m_sceneCollectionPreUnloadedEvent.Raise(additiveCollection);
            SceneManager.runtime.Close(additiveCollection);
        }
        
        public void UnloadAllAdditiveSceneCollections()
        {
            var additiveCollections = SceneManager.runtime.openAdditiveCollections.ToArray();
            
            foreach (var additiveCollection in additiveCollections)
            {
                m_sceneCollectionPreUnloadedEvent.Raise(additiveCollection);
                SceneManager.runtime.Close(additiveCollection);
            }
        }

        #endregion
    }
}