using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameEvent
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [FormerlySerializedAs("listenEvent")] [SerializeField] private GameEvent<T> listenGameEvent;
        [SerializeField] private UnityEvent<T> onEventRaised;

        private void OnEnable()
        {
            listenGameEvent.RegisterListener(this);
        }
        
        private void OnDisable()
        {
            listenGameEvent.UnregisterListener(this);
        }
        
        public void Raise(T value)
        {
            onEventRaised?.Invoke(value);
        }
    }
    
    public class EventListener : EventListener<Empty> {}
}