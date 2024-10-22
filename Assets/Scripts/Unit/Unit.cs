using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitTravelLine travelLine;
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 1f;
    
    public Waypoint NextWaypoint { get; private set; }
    
    private Queue<Waypoint> _waypoints = new ();
    private bool _isMoving;
    private Waypoint? _previousWaypoint;
    private float _travelProgress;
    private float _currentTravelStartTime;
    private Vector3 _currentStartPosition;
    private float _distanceToNextWaypoint;

    private void Start()
    {
        NextWaypoint = new Waypoint(new AxialCoordinate(0, 0), transform.position); // This is only for testing
    }

    private void Update()
    {
        if (_isMoving)
        {
            Move();
        }
        else if (_waypoints.Count > 0)
        {
            FetchNextWaypoint();
            _isMoving = true;
            UpdateTravelLine();
        }
    }
    
    public void AddWaypoint(Waypoint waypoint)
    {
        _waypoints.Enqueue(waypoint);
        UpdateTravelLine();
    }

    public void SetAllWaypoints(List<Waypoint> newWaypoints)
    {
        _waypoints.Clear();
        _waypoints = new Queue<Waypoint>(newWaypoints);
        
        if(_previousWaypoint != null && _waypoints.Count > 0 && _waypoints.Peek().Equals(_previousWaypoint.Value))
            FetchNextWaypoint();
        
        UpdateTravelLine();
    }

    private void UpdateTravelLine()
    { 
        var linePoints = new List<Vector3> { transform.position };
        if(_isMoving)
            linePoints.Add(NextWaypoint.Position);
        
        linePoints.AddRange(_waypoints.Select(waypoint => waypoint.Position));
        if (linePoints.Count > 1)
            travelLine.SetAllPositions(linePoints.ToArray());
    }

    private void FetchNextWaypoint()
    {
        _previousWaypoint = NextWaypoint;
        NextWaypoint = _waypoints.Dequeue();
        
        _currentTravelStartTime = Time.time;
        _currentStartPosition = transform.position;
        _distanceToNextWaypoint = Vector3.Distance(NextWaypoint.Position, transform.position);
        _travelProgress = 0f;
    }
    
    private void Move()
    {
        var traveledTime = Time.time - _currentTravelStartTime;
        var traveledDistance = traveledTime * moveSpeed;
        _travelProgress = traveledDistance / _distanceToNextWaypoint;
        _travelProgress = traveledDistance / _distanceToNextWaypoint;
        transform.position = Vector3.Lerp(_currentStartPosition, NextWaypoint.Position, _travelProgress);
        travelLine.SetFirstNodePosition(transform.position);
        
        if (_travelProgress >= 1f)
        {
            _previousWaypoint = null;
            _isMoving = false;
            if(_waypoints.Count == 0)
                travelLine.gameObject.SetActive(false);
        }
    }
    
    public struct Waypoint : IEquatable<Waypoint>
    {
        public AxialCoordinate Coordinates;
        public Vector3 Position;

        public Waypoint(AxialCoordinate coordinates, Vector3 position)
        {
            Coordinates = coordinates;
            Position = position;
        }

        public bool Equals(Waypoint other)
        {
            return Coordinates.Equals(other.Coordinates);
        }
    }
}
