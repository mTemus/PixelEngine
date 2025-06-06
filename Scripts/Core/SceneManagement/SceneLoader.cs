using System.Threading.Tasks;
using EditorAttributes;
using PixelEngine.Core.SceneManagement.Events;
using PixelEngine.Utility.Math;
using UnityEngine;
using UnityEngine.UI;

namespace PixelEngine.Core.SceneManagement
{
    //TODO: addressables
    //TODO: UniTask/R3
    //TODO: modular loading screen
    //TODO: scene groups:
    //  - internal groups for gameplay levels?
    
    public class SceneLoader : MonoBehaviour
    {
        [Title(title: "Settings", titleSize: 22, titleSpace: 14, alignment: TextAnchor.MiddleCenter, drawLine: true)]
        
        [SerializeField] 
        [Range(100, 5000)]
        private int m_sceneOperationMillisecondsDelay = 100;
        
        [Title(title: "Events", titleSize: 22, titleSpace: 14, alignment: TextAnchor.MiddleCenter, drawLine: true)]
        [SerializeField, Required(true)]
        private ScriptableEventSceneData m_sceneLoadedEvent;
        
        [SerializeField, Required(true)]
        private ScriptableEventSceneData m_sceneUnloadedEvent;
        
        [SerializeField, Required(true)]
        private ScriptableEventSceneData m_scenePreUnloadedEvent;
        
        [SerializeField, Required(true)]
        private ScriptableEventSceneGroup m_sceneGroupLoadedEvent;
        
        [SerializeField, Required(true)]
        private ScriptableEventSceneGroup m_sceneGroupPreUnloadedEvent;
        
        [Title(title: "Other", titleSize: 22, titleSpace: 14, alignment: TextAnchor.MiddleCenter, drawLine: true)]
        [SerializeField] private Image m_loadingBar;
        [SerializeField] private float m_fillSpeed = 0.5f;
        [SerializeField] private Canvas m_loadingCanvas;
        [SerializeField] private Camera m_loadingCamera;
        [SerializeField] private SceneGroup[] m_sceneGroups;

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
        
        private async void Start()
        {
            enabled = false;
            await LoadSceneGroup(0);
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
    }
}