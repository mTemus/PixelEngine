using System.Threading.Tasks;
using PixelEngine.Utility.Math;
using UnityEngine;
using UnityEngine.UI;

namespace PixelEngine.Core.SceneManagement
{
    //TODO: addressables
    //TODO: UniTask/R3
    //TODO: modular loading screen
    //TODO: scene group with a tag/enum name
    //TODO: loading single scenes (?)
    //TODO: scene groups:
    //  - which is initializable checkbox
    //  - which is savable checkbox
    //  - internal groups for gameplay levels?
    //TODO: events:
    //  - scene group loaded -> for initialization
    
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image m_loadingBar;
        [SerializeField] private float m_fillSpeed = 0.5f;
        [SerializeField] private Canvas m_loadingCanvas;
        [SerializeField] private Camera m_loadingCamera;
        [SerializeField] private SceneGroup[] m_sceneGroups;

        private float m_targetProgress;
        private bool m_isLoading;

        public readonly SceneGroupManager Manager = new SceneGroupManager();

        private void Awake()
        {
            Manager.OnSceneLoaded += sceneName => Debug.Log($"Scene loaded: {sceneName}");
            Manager.OnSceneUnloaded += sceneName => Debug.Log($"Scene unloaded: {sceneName}");
            Manager.OnSceneGroupLoaded += () => Debug.Log($"Scene group loaded.");
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
            await Manager.LoadScenes(m_sceneGroups[index], progress);
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