using Obvious.Soap;
using PixelEngine.Core.SceneManagement.Loading;
using UnityEngine;
using UnityEngine.Events;

namespace PixelEngine.Core.SceneManagement.Events
{
    public class EventListenerSceneData : EventListenerGeneric<SceneData>
    {
        [SerializeField] private EventResponse[] m_eventResponses = null;
        protected override EventResponse<SceneData>[] EventResponses => m_eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<SceneData>
        {
            [SerializeField] private ScriptableEventSceneData m_scriptableEvent = null;
            public override ScriptableEvent<SceneData> ScriptableEvent => m_scriptableEvent;

            [SerializeField] private SceneDataUnityEvent m_response = null;
            public override UnityEvent<SceneData> Response => m_response;
        }

        [System.Serializable]
        public class SceneDataUnityEvent : UnityEvent<SceneData>
        {
        
        }
    }
}