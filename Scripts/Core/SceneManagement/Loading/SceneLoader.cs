using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomInspector;
using PixelEngine.Core.SceneManagement.Events;
using PixelEngine.Utility.Math;
using UnityEngine;
using UnityEngine.UI;

namespace PixelEngine.Core.SceneManagement.Loading
{
    //TODO: addressables
    //TODO: UniTask/R3
    //TODO: modular loading screen
    //TODO: scene groups:
    //  - internal groups for gameplay levels?
    
    public class SceneLoader : MonoBehaviour
    {
        [Tab("Settings")]
        [SerializeField] 
        [Range(100, 5000)]
        private int m_sceneOperationMillisecondsDelay = 100;
        
        [Tab("Events")] 
        [SerializeField]
        private ScriptableEventSceneData m_sceneLoadedEvent;
        
        [Tab("Events")] 
        [SerializeField]
        private ScriptableEventSceneData m_sceneUnloadedEvent;
        
        [Tab("Events")] 
        [SerializeField]
        private ScriptableEventSceneData m_scenePreUnloadedEvent;
        
        [Tab("Events")] 
        [SerializeField]
        private ScriptableEventSceneGroup m_sceneGroupLoadedEvent;
        
        [Tab("Events")] 
        [SerializeField]
        private ScriptableEventSceneGroup m_sceneGroupPreUnloadedEvent;
        

        [SerializeField] 
        private List<SceneGroup> m_sceneGroups;
        
        [SerializeField] 
        private List<SceneData> m_singleScenes;
        
        [Tab("Other")]
        [SerializeField] private Image m_loadingBar;
        [Tab("Other")]
        [SerializeField] private float m_fillSpeed = 0.5f;
        [Tab("Other")]
        [SerializeField] private Canvas m_loadingCanvas;
        [Tab("Other")]
        [SerializeField] private Camera m_loadingCamera;
        

        private float m_targetProgress;
        private bool m_isLoading;

        public SceneGroupManager SceneGroupManager;
        public SingleSceneManager SingleScenesManager;

        private void Awake()
        {
            SceneGroupManager = new SceneGroupManager(m_sceneOperationMillisecondsDelay);
            SingleScenesManager = new SingleSceneManager(m_sceneOperationMillisecondsDelay);
        
            //TODO: bind scriptable events to normal events in managers
            
#if UNITY_EDITOR
            SceneGroupManager.OnSceneGroupLoaded += group => Debug.Log($"Scene group loaded: {group.GroupName.name}");
            SceneGroupManager.OnBeforeSceneGroupUnloaded += group => Debug.Log($"Scene group unloaded: {group.GroupName.name}");

            SingleScenesManager.OnSceneLoaded += scene => Debug.Log($"Scene loaded: {scene.Name}");
            SingleScenesManager.OnScenePreUnloaded += scene => Debug.Log($"Scene preunloaded: {scene.Name}");
            SingleScenesManager.OnSceneUnloaded += scene => Debug.Log($"Scene unloaded: {scene.Name}");
#endif

        }

        private void Update()
        {
            if (!m_isLoading)
                return;
            
            var currentFillAmount = m_loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - m_targetProgress);
            var dynamicFillSpeed = progressDifference * m_fillSpeed;
            
            m_loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, m_targetProgress, Time.deltaTime * dynamicFillSpeed);
        }
        
        private async Task LoadSceneGroup(int index)
        {
            m_loadingBar.fillAmount = 0f;
            m_targetProgress = 1f;

            var progress = new LoadingProgress();
            progress.OnProgress += target => m_targetProgress = Mathf.Max(target, m_targetProgress);
            
            EnableLoadingCanvas();
            await SceneGroupManager.LoadScenes(m_sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        private void EnableLoadingCanvas(bool enable = true)
        {
            enabled = enable;
            m_isLoading = enable;
            m_loadingCanvas.gameObject.SetActive(enable);
            m_loadingCamera.gameObject.SetActive(enable);
        }

        public async Task LoadScene(ESceneType sceneType, bool reloadIfLoaded = false)
        {
            var sceneData = m_singleScenes.Find(x => x.SceneType == sceneType);

            if (sceneData == null)
                throw new Exception($"Can't load scene: {sceneType}, scene not found!");
            
            await SingleScenesManager.LoadScene(sceneData);
        }
    }
}