using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEngine.Events;

namespace Unit
{
    public class WaypointQueue : NetworkBehaviour
    {
        public event UnityAction<Hexagon, bool> OnWaypointsUpdated;
        public event UnityAction OnPathChanged;
        
        private Queue<Hexagon> _queue = new();
        private Hexagon _lastFetched;

        public Hexagon FetchWaypoint()
        {
            _lastFetched = _queue.Count == 0 ? null : _queue.Dequeue();
            OnPathChanged?.Invoke();
            return _lastFetched;
        }
        
        public void UpdateWaypoints(List<Hexagon> waypoints, bool wasSplit = false)
        {
            if (waypoints.IsNullOrEmpty()) 
                return;
            
            _queue = new Queue<Hexagon>(waypoints);
            OnWaypointsUpdated?.Invoke(waypoints[0], wasSplit);
            OnPathChanged?.Invoke();
        }

        public List<Hexagon> GetCurrentAndNextWaypoints()
        {
            var waypoints = new List<Hexagon>(_queue);
            if (_lastFetched != null)
                waypoints.Insert(0,_lastFetched);

            return waypoints;
        }
    }
}