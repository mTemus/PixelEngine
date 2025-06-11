using Obvious.Soap;
using PixelEngine.Core.SceneManagement.Loading;
using UnityEngine;
using UnityEngine.Events;

namespace PixelEngine.Core.SceneManagement.Events
{
    [AddComponentMenu("Soap/EventListeners/EventListener"+nameof(SceneGroup))]
    public class EventListenerSceneGroup : EventListenerGeneric<SceneGroup>
    {
        [SerializeField] private EventResponse[] m_eventResponses = null;
        protected override EventResponse<SceneGroup>[] EventResponses => m_eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<SceneGroup>
        {
            [SerializeField] private ScriptableEventSceneGroup m_scriptableEvent = null;
            public override ScriptableEvent<SceneGroup> ScriptableEvent => m_scriptableEvent;

            [SerializeField] private SceneGroupUnityEvent m_response = null;
            public override UnityEvent<SceneGroup> Response => m_response;
        }

        [System.Serializable]
        public class SceneGroupUnityEvent : UnityEvent<SceneGroup>
        {
        
        }
    }
}