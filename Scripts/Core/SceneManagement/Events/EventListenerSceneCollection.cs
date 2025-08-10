using AdvancedSceneManager.Models;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace PixelEngine.Core.SceneManagement.Events
{
    [AddComponentMenu("PixelEngine/SOAP/EventListeners/Scene Collection Scriptable Event Listener")]
    public class EventListenerSceneCollection : EventListenerGeneric<SceneCollection>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<SceneCollection>[] EventResponses => _eventResponses;
        [System.Serializable]
        public class EventResponse : EventResponse<SceneCollection>
        {
            [SerializeField] private ScriptableEventSceneCollection _scriptableEvent = null;
            public override ScriptableEvent<SceneCollection> ScriptableEvent => _scriptableEvent;
            [SerializeField] private SceneCollectionUnityEvent _response = null;
            public override UnityEvent<SceneCollection> Response => _response;
        }
        [System.Serializable]
        public class SceneCollectionUnityEvent : UnityEvent<SceneCollection>
        {
            
        }
    }
}
