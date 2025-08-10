using AdvancedSceneManager.Models;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace PixelEngine.Core.SceneManagement.Events
{
    [AddComponentMenu("PixelEngine/SOAP/EventListeners/Scene Scriptable Event Listener")]
    public class EventListenerScene : EventListenerGeneric<Scene>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<Scene>[] EventResponses => _eventResponses;
        [System.Serializable]
        public class EventResponse : EventResponse<Scene>
        {
            [SerializeField] private ScriptableEventScene _scriptableEvent = null;
            public override ScriptableEvent<Scene> ScriptableEvent => _scriptableEvent;
            [SerializeField] private SceneUnityEvent _response = null;
            public override UnityEvent<Scene> Response => _response;
        }
        [System.Serializable]
        public class SceneUnityEvent : UnityEvent<Scene>
        {
            
        }
    }
}
