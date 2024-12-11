using GameEvents;
using HexSystem;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    public class UnitGroupMovement : NetworkBehaviour
    {
        [SerializeField] private UnitGroup unitGroup;
        
        public bool HasMovementLeft { get; set; }
        public Hexagon StartHexagon { get; set; }
        public Hexagon GoalHexagon { get; set; }
        public float MoveSpeed { get; set; }

        private Vector3 _startPosition;
        private bool _assignedToNextHexagon;
        private float _movementProgress;
        private float _distanceToGoal;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            unitGroup.WaypointQueue.OnWaypointsUpdated += OnWaypointsUpdated;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;

            unitGroup.WaypointQueue.OnWaypointsUpdated -= OnWaypointsUpdated;
        }

        public void Initialize(Hexagon currentHexagon)
        {
            GoalHexagon = currentHexagon;
            ResetForNextMovementStep();
        }

        private void ResetForNextMovementStep()
        {
            StartHexagon = GoalHexagon;
            _startPosition = transform.position;
            _movementProgress = 0;
            _assignedToNextHexagon = false;
        }

        public void Update()
        {
            if (!IsServer)
                return;

            if (HasMovementLeft && unitGroup.CanMove)
                Move();

            if (_movementProgress < 1) 
                return;
            
            SetupForMovementToNextGoal(true);
        }

        private void Move()
        {
            _movementProgress += Time.deltaTime * MoveSpeed / _distanceToGoal;
            transform.position = Vector3.Lerp(_startPosition, GoalHexagon.transform.position, _movementProgress);
            
            if (!_assignedToNextHexagon &&
                Vector3.Distance(GoalHexagon.transform.position, transform.position) < MapBuilder.TileWidth * 0.5f)
            {
                ServerEvents.Unit.OnUnitGroupReachedNewHex?.Invoke(unitGroup, GoalHexagon.Coordinates);
                _assignedToNextHexagon = true;
            }
        }

        private void OnWaypointsUpdated(Hexagon firstWaypoint, bool wasSplit)
        {
            if (!HasMovementLeft && !wasSplit)
                SetupForMovementToNextGoal(true);
            else if(!HasMovementLeft || StartHexagon == firstWaypoint || wasSplit)
                SetupForMovementToNextGoal(false);
        }

        private void SetupForMovementToNextGoal(bool startsFromHexCenter)
        {
            ResetForNextMovementStep();

            var newGoal = unitGroup.WaypointQueue.FetchWaypoint();
            HasMovementLeft = newGoal is not null;

            if (startsFromHexCenter)
                ServerEvents.Unit.OnUnitGroupReachedHexCenter.Invoke(unitGroup, GoalHexagon.Coordinates);

            if (HasMovementLeft)
                StartMovingToGoal(newGoal, startsFromHexCenter);
        }

        private void StartMovingToGoal(Hexagon goal, bool startsFromHexCenter)
        {
            if (startsFromHexCenter)
                ServerEvents.Unit.OnUnitGroupLeftHexCenter?.Invoke(unitGroup);
            
            GoalHexagon = goal;
            var goalPosition = goal.transform.position;
            transform.rotation = Quaternion.LookRotation(goalPosition - transform.position);
            _distanceToGoal = Vector3.Distance(transform.position, goalPosition);
        }
    }
}