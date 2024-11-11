using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(UnitGroup))]
    public class UnitGroupMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitGroupTravelLine groupTravelLine;
        
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 1f;
        
        public GridData GridData { get; set; }
        public Hexagon PreviousHexagon { get; private set; }
        public Hexagon NextHexagon { get; set; }
        
        private Queue<Hexagon> _hexWaypoints = new ();

        private UnitGroup _unitGroup;
        private float _distanceToNextHexagon;
        private float _travelProgress;
        private float _currentTravelStartTime;
        private Vector3 _currentStartPosition;
        private bool _isMoving;
        private bool _assignedToNextHexagon;
        private bool _isResting;

        private void Awake()
        {
            _unitGroup = GetComponent<UnitGroup>();
        }

        #region Server

        private void OnEnable()
        {
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleSwitchedDayNightCycle;
        }

        private void OnDisable()
        {
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState -= HandleSwitchedDayNightCycle;
        }

        private void Update()
        {
            if (!IsServer)
                return;
            
            if (_isResting)
                return; 
            
            FollowWaypoints();
        }

        public void CopyValuesFrom(UnitGroupMovement movementToCopy)
        {
            PreviousHexagon = movementToCopy.PreviousHexagon;
            List<Hexagon> pathToCopy = movementToCopy.GetAllWaypoints();
            if (pathToCopy.Count > 0)
                pathToCopy.Insert(0, movementToCopy.NextHexagon);
            SetAllWaypoints(pathToCopy);
        }
        
        public void SetAllWaypoints(List<Hexagon> newWaypoints)
        {
            var shouldStartMovingFromIdle = _hexWaypoints.Count == 0 && newWaypoints.Count > 0;
            var nextHexagonStationaryUnitGroupId = GridData.GetHexagonDataOnCoordinate(NextHexagon.Coordinates).StationaryUnitGroup;
            if(shouldStartMovingFromIdle && nextHexagonStationaryUnitGroupId == NetworkObjectId)
                GridData.RemoveStationaryUnitGroupFromHex(NextHexagon.Coordinates, _unitGroup);
                
            _hexWaypoints.Clear();
            _hexWaypoints = new Queue<Hexagon>(newWaypoints);
        
            if(PreviousHexagon != null && _hexWaypoints.Count > 0 && _hexWaypoints.Peek().Equals(PreviousHexagon))
                FetchNextWaypoint();
        
            UpdateTravelLine();
        }
        
        public List<Hexagon> GetAllWaypoints()
        {
            return new List<Hexagon>(_hexWaypoints);
        }

        private void FollowWaypoints()
        {
            if (_isMoving)
            {
                Move();

                if (!_assignedToNextHexagon && Vector3.Distance(NextHexagon.transform.position, transform.position) < MapBuilder.TileWidth * 0.5f)
                {
                    Debug.Log("Unit reached next hex");

                    GridData.MoveUnitGroupToHex(PreviousHexagon.Coordinates, NextHexagon.Coordinates, _unitGroup);
                    _assignedToNextHexagon = true;
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
        
        private void Move()
        {
            var traveledTime = Time.time - _currentTravelStartTime;
            var traveledDistance = traveledTime * moveSpeed;
            _travelProgress = traveledDistance / _distanceToNextHexagon;
            
            transform.position = Vector3.Lerp(_currentStartPosition, NextHexagon.transform.position, _travelProgress);
            
            SyncFirstTravelLinePositionClientRpc();
        }
        
        private void OnWaypointReached()
        {
            PreviousHexagon = null;
            _isMoving = false;

            if (_hexWaypoints.Count != 0) return;
            
            DisableTravelLineClientRpc();

            var nextHexagonData = GridData.GetHexagonDataOnCoordinate(NextHexagon.Coordinates);
            var stationaryUnitGroupId = nextHexagonData.StationaryUnitGroup;
            if (stationaryUnitGroupId == null)
            {
                Debug.Log("Unit should become stationary");
                GridData.UpdateStationaryUnitGroupOfHex(NextHexagon.Coordinates, _unitGroup);
            }
            else
            {
                var stationaryUnitGroup = UnitGroup.UnitGroupsInGame[stationaryUnitGroupId.Value];

                if (stationaryUnitGroup.PlayerColor == _unitGroup.PlayerColor)
                {
                    Debug.Log("Unit should be added to other stationary group");

                    stationaryUnitGroup.AddUnits(_unitGroup.UnitCount.Value);

                    GridData.DeleteUnitGroupFromHex(NextHexagon.Coordinates, _unitGroup);
                    _unitGroup.Delete();
                    GameEvents.UNIT.OnUnitGroupDeleted.Invoke(_unitGroup);
                }
            }
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

        private void UpdateTravelLine()
        { 
            var linePoints = new List<Vector3> { transform.position };
            if(_isMoving)
                linePoints.Add(NextHexagon.transform.position);
        
            linePoints.AddRange(_hexWaypoints.Select(hex => hex.transform.position));
            if (linePoints.Count > 1)
            {
                UpdateFullTravelLineClientRpc(linePoints.ToArray());
            }
        }
        
        private void HandleSwitchedDayNightCycle(DayNightCycle.CycleState newDayNightCycle)
        {
            _isResting = newDayNightCycle == DayNightCycle.CycleState.Night;
        }

        #endregion

        #region Client

        public void Initialize(PlayerColor playerColor)
        {
            groupTravelLine.Initialize(playerColor);
        }

        [ClientRpc]
        private void UpdateFullTravelLineClientRpc(Vector3[] travelLinePoints)
        {
            if (_unitGroup.PlayerId != NetworkManager.Singleton.LocalClientId)
                travelLinePoints = new [] { travelLinePoints[0], travelLinePoints[1] };
            
            groupTravelLine.SetAllPositions(travelLinePoints);
        }

        [ClientRpc]
        private void SyncFirstTravelLinePositionClientRpc()
        {
            groupTravelLine.SetFirstNodePosition(transform.position);
        }
        
        [ClientRpc]
        private void DisableTravelLineClientRpc()
        {
            groupTravelLine.gameObject.SetActive(false);
        }

        #endregion
    }
}