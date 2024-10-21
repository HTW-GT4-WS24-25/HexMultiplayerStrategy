using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Event
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField] private Event<T> listenEvent;
        [SerializeField] private UnityEvent<T> onEventRaised;

        private void OnEnable()
        {
            listenEvent.RegisterListener(this);
        }
        
        private void OnDisable()
        {
            listenEvent.UnregisterListener(this);
        }
        
        public void Raise(T value)
        {
            onEventRaised?.Invoke(value);
        }
    }
    
    public class EventListener : EventListener<Empty> {}
}