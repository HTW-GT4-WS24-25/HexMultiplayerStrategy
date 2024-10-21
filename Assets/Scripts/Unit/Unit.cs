using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitTravelLine travelLine;
    
    [Header("Settings")]
    [SerializeField] private float moveTime = 1f;
    
    private Queue<Vector3> _waypoints = new ();
    private bool _isMoving;
    private Vector3 _destination;
    private float _travelProgress;
    private float _currentTravelStartTime;
    private Vector3 _currentStartPosition;

    private void Update()
    {
        if (_isMoving)
        {
            Move();
        }
        else if (_waypoints.Count > 0)
        {
            SetNextWaypointAsDestination();
            _isMoving = true;
            UpdateTravelLine();
        }
    }
    
    public void AddWaypoint(Vector3 point)
    {
        _waypoints.Enqueue(point);
        UpdateTravelLine();
    }

    private void UpdateTravelLine()
    { 
        var linePoints = new List<Vector3> { transform.position };
        if(_isMoving)
            linePoints.Add(_destination);
        
        linePoints.AddRange(_waypoints);
        if (linePoints.Count > 1)
            travelLine.SetAllPositions(linePoints.ToArray());
    }

    private void SetNextWaypointAsDestination()
    {
        _destination = _waypoints.Dequeue();
        
        _currentTravelStartTime = Time.time;
        _currentStartPosition = transform.position;
        _travelProgress = 0f;
    }
    
    private void Move()
    {
        var traveledTime = Time.time - _currentTravelStartTime;
        _travelProgress = traveledTime / moveTime;
        transform.position = Vector3.Lerp(_currentStartPosition, _destination, _travelProgress);
        travelLine.SetFirstNodePosition(transform.position);

        if (_travelProgress >= 1f)
        {
            _isMoving = false;
            if(_waypoints.Count == 0)
                travelLine.gameObject.SetActive(false);
        }
    }
}
