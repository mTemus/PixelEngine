using EditorAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

//TODO: initialize and add to services?
namespace PixelEngine.UI
{
    public class Blackscreen : MonoBehaviour
    {
        [SerializeField, Required] 
        private CanvasGroup m_canvasGroup;

        [SerializeField]
        private UnityEvent m_onShowEvent;
    
        [SerializeField]
        private UnityEvent m_onHideEvent;

        public UnityEvent OnShowEvent => m_onShowEvent;
        public UnityEvent OnHideEvent => m_onHideEvent;

        private void Start()
        {
            m_canvasGroup.blocksRaycasts = true;
        }
    
        public void Show(float duration)
        {
            PrepareCanvas(true);
        
            Tween.Alpha(m_canvasGroup, 1, duration)
                .OnComplete(() =>
                {
                    m_onShowEvent.Invoke();
                });
        }
    
        public void Hide(float duration)
        {
            PrepareCanvas(false);
        
            Tween.Alpha(m_canvasGroup, 0, duration)
                .OnComplete(() =>
                {
                    m_onHideEvent.Invoke();
                    m_canvasGroup.gameObject.SetActive(false);
                });
        }

        private void PrepareCanvas(bool show)
        {
            m_canvasGroup.alpha = show ? 1 : 0;
            m_canvasGroup.gameObject.SetActive(true);
        }
    }
}
