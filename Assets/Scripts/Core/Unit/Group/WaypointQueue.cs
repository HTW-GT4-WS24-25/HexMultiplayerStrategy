using System.Collections.Generic;
using System.Linq;
using Core.HexSystem.Hex;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEngine.Events;

namespace Core.Unit.Group
{
    public class WaypointQueue : NetworkBehaviour
    {
        public event UnityAction OnWaypointsUpdated;
        public event UnityAction OnPathChanged;
        
        private Queue<Hexagon> _queue = new();
        private Hexagon _lastFetched;

        public Hexagon FetchWaypoint()
        {
            _lastFetched = _queue.Count == 0 ? null : _queue.Dequeue();
            OnPathChanged?.Invoke();
            return _lastFetched;
        }

        public Hexagon PeekNextWaypoint()
        {
            return _queue.Count == 0 ? null : _queue.Peek();
        }
        
        public void UpdateWaypoints(List<Hexagon> waypoints)
        {
            if (waypoints.IsNullOrEmpty()) 
                return;
            
            _queue = new Queue<Hexagon>(waypoints);
            OnWaypointsUpdated?.Invoke();
            OnPathChanged?.Invoke();
        }

        public List<Hexagon> GetWaypoints()
        {
            return _queue.ToList();
        }

        public List<Hexagon> GetCurrentAndNextWaypoints()
        {
            var waypoints = new List<Hexagon>(_queue);
            if (_lastFetched != null)
                waypoints.Insert(0,_lastFetched);

            return waypoints;
        }

        public void Clear()
        {
            _lastFetched = null;
            _queue.Clear();
            OnPathChanged?.Invoke();
        }
    }
}