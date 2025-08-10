using AdvancedSceneManager;
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
        private ScriptableEventSceneCollection m_sceneCollectionLoadedEvent;
        
        [Tab("Scene Collection Events")] 
        [SerializeField]
        private ScriptableEventSceneCollection m_sceneCollectionPreUnloadedEvent;

        public void EarlyInitialize()
        {
            SceneManager.runtime.collectionOpened += OnCollectionOpened;
            SceneManager.runtime.collectionClosed += OnCollectionClosed;
            SceneManager.runtime.sceneOpened += OnSceneOpened;
            SceneManager.runtime.sceneClosed += OnSceneClosed;
            SceneManager.runtime.scenePreloaded += OnScenePreloaded;
        }

        public void Uninitialize()
        {
            SceneManager.runtime.collectionOpened -= OnCollectionOpened;
            SceneManager.runtime.collectionClosed -= OnCollectionClosed;
            SceneManager.runtime.sceneOpened -= OnSceneOpened;
            SceneManager.runtime.sceneClosed -= OnSceneClosed;
            SceneManager.runtime.scenePreloaded -= OnScenePreloaded;
        }

        #region Collections

        private void OnCollectionOpened(SceneCollection collection)
        {
            m_sceneCollectionLoadedEvent.Raise(collection);
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

        private void OnSceneOpened(Scene scene)
        {
            m_sceneLoadedEvent.Raise(scene);
        }

        private void OnSceneClosed(Scene scene)
        {
            m_sceneUnloadedEvent.Raise(scene);
        }

        #endregion

        #region Public API

        public void LoadSceneCollection(SceneCollection collection)
        {
            var currentCollection = SceneManager.runtime.openCollection;

            if (currentCollection != null)
            {
                m_sceneCollectionPreUnloadedEvent.Raise(currentCollection);
                SceneManager.runtime.Close(currentCollection);    
            }
            
            SceneManager.runtime.Open(collection);
        }

        public void LoadScene(Scene scene)
        {
            SceneManager.runtime.Open(scene);
        }
        
        public void UnloadScene(Scene scene)
        {
            m_scenePreUnloadedEvent.Raise(scene);
            SceneManager.runtime.Close(scene);
        }

        #endregion
        
        // private void Update()
        // {
        //     if (!m_isLoading)
        //         return;
        //     
        //     var currentFillAmount = m_loadingBar.fillAmount;
        //     var progressDifference = Mathf.Abs(currentFillAmount - m_targetProgress);
        //     var dynamicFillSpeed = progressDifference * m_fillSpeed;
        //     
        //     m_loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, m_targetProgress, Time.deltaTime * dynamicFillSpeed);
        // }
        //
        // private async Task LoadSceneGroup(int index)
        // {
        //     m_loadingBar.fillAmount = 0f;
        //     m_targetProgress = 1f;
        //
        //     var progress = new LoadingProgress();
        //     progress.OnProgress += target => m_targetProgress = Mathf.Max(target, m_targetProgress);
        //     
        //     EnableLoadingCanvas();
        //     await SceneGroupManager.LoadScenes(m_sceneGroups[index], progress);
        //     EnableLoadingCanvas(false);
        // }
        //
        // private void EnableLoadingCanvas(bool enable = true)
        // {
        //     enabled = enable;
        //     m_isLoading = enable;
        //     m_loadingCanvas.gameObject.SetActive(enable);
        //     m_loadingCamera.gameObject.SetActive(enable);
        // }
        //
        // public async Task LoadScene(ESceneType sceneType, bool reloadIfLoaded = false)
        // {
        //     var sceneData = m_singleScenes.Find(x => x.SceneType == sceneType);
        //
        //     if (sceneData == null)
        //         throw new Exception($"Can't load scene: {sceneType}, scene not found!");
        //     
        //     await SingleScenesManager.LoadScene(sceneData);
        // }
    }
}