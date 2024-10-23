using System.Collections.Generic;
using UnityEngine;

namespace GameEvent
{
    public abstract class GameEvent<T> : ScriptableObject
    {
        protected List<EventListener<T>> _listeners = new ();

        public void RegisterListener(EventListener<T> listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(EventListener<T> listener)
        {
            _listeners.Remove(listener);
        }

        public void Invoke(T value)
        {
            foreach (var eventListener in _listeners)
            {
                eventListener.Raise(value);
            }
        }
    }
    
    public readonly struct Empty {}
    
    [CreateAssetMenu(fileName = "New Event", menuName = "Events/Event", order = 0)]
    public class GameEvent : GameEvent<Empty> { }
}


