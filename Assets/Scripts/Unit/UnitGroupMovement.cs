using System;
using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit
{
    [RequireComponent(typeof(UnitGroup))]
    public class UnitGroupMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitGroupTravelLine groupTravelLine;
        
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 1f;
    
        public UnitGroup UnitGroup { get; private set; }
        
        public Hexagon PreviousHexagon { get; private set; }
        public Hexagon NextHexagon { get; set; }
        
        private Queue<Hexagon> _hexWaypoints = new ();
       
        private float _distanceToNextHexagon;
        private float _travelProgress;
        private float _currentTravelStartTime;
        private Vector3 _currentStartPosition;
        private bool _isMoving;
        private bool _assignedToNextHexagon;
        private bool _isResting;

        private void Awake()
        {
            UnitGroup = GetComponent<UnitGroup>();
        }

        private void OnEnable()
        {
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleSwitchedDayNightCycle;
        }

        private void OnDisable()
        {
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState -= HandleSwitchedDayNightCycle;
        }

        public void Initialize(PlayerColor playerColor)
        {
            groupTravelLine.Initialize(playerColor);
        }

        private void Update()
        {
            if (!IsServer)
                return;
            
            if (_isResting)
                return; 
            
            if (_isMoving)
            {
                Move();

                if (!_assignedToNextHexagon && Vector3.Distance(NextHexagon.transform.position, transform.position) < MapBuilder.TileWidth * 0.5f)
                {
                    Debug.Log("Unit reached next hex");

                    PreviousHexagon.unitGroups.Remove(UnitGroup);
                    
                    UnitGroup.PlaceOnHex(NextHexagon);
                    _assignedToNextHexagon = true;
                    
                    NextHexagon.UpdateDominance(UnitGroup.PlayerColor);
                }
            
                if (_travelProgress >= 1f)
                {
                    OnWaypointReached();
                }
            }
            else if (_hexWaypoints.Count > 0)
            {
                FetchNextWaypoint();
                _isMoving = true;
                UpdateTravelLine();
            }
        }

        public void SetAllWaypoints(List<Hexagon> newWaypoints)
        {
            if(_hexWaypoints.Count == 0 && newWaypoints.Count > 0 && UnitGroup.Hexagon.StationaryUnitGroup == UnitGroup)
                UnitGroup.Hexagon.RemoveStationaryUnitGroup();
                
            _hexWaypoints.Clear();
            _hexWaypoints = new Queue<Hexagon>(newWaypoints);
        
            if(PreviousHexagon != null && _hexWaypoints.Count > 0 && _hexWaypoints.Peek().Equals(PreviousHexagon))
                FetchNextWaypoint();
        
            UpdateTravelLine();
        }
        
        private void HandleSwitchedDayNightCycle(DayNightCycle.CycleState newDayNightCycle)
        {
            _isResting = newDayNightCycle == DayNightCycle.CycleState.Night;
        }

        private void UpdateTravelLine()
        { 
            var linePoints = new List<Vector3> { transform.position };
            if(_isMoving)
                linePoints.Add(NextHexagon.transform.position);
        
            linePoints.AddRange(_hexWaypoints.Select(hex => hex.transform.position));
            if (linePoints.Count > 1)
                groupTravelLine.SetAllPositions(linePoints.ToArray());
        }

        private void FetchNextWaypoint()
        {
            PreviousHexagon = NextHexagon;
            NextHexagon = _hexWaypoints.Dequeue();
        
            _currentTravelStartTime = Time.time;
            _currentStartPosition = transform.position;
            _distanceToNextHexagon = Vector3.Distance(NextHexagon.transform.position, transform.position);
            _travelProgress = 0f;

            _assignedToNextHexagon = false;
        }
    
        private void Move()
        {
            var traveledTime = Time.time - _currentTravelStartTime;
            var traveledDistance = traveledTime * moveSpeed;
            _travelProgress = traveledDistance / _distanceToNextHexagon;
            
            transform.position = Vector3.Lerp(_currentStartPosition, NextHexagon.transform.position, _travelProgress);
            
            groupTravelLine.SetFirstNodePosition(transform.position);
        }

        private void OnWaypointReached()
        {
            if (_hexWaypoints.Count == 0)
            {
                groupTravelLine.gameObject.SetActive(false);

                if (NextHexagon.StationaryUnitGroup == null)
                {
                    Debug.Log("Unit should become stationary");
                    NextHexagon.ChangeUnitGroupOnHexToStationary(UnitGroup);
                }
                else
                {
                    Debug.Log("Unit should be added to other stationary group");
                    
                    var stationaryGroup = NextHexagon.StationaryUnitGroup;
                    stationaryGroup.AddUnits(UnitGroup.UnitCount.Value);
                    UnitGroup.Delete();
                    GameEvents.UNIT.OnUnitGroupDeleted.Invoke(UnitGroup);
                }
            }
            
            PreviousHexagon = null;
            _isMoving = false;
        }
    }
}