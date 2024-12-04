using System.Collections.Generic;
using GameEvents;
using HexSystem;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    public class UnitGroupMovement : NetworkBehaviour
    {
        public bool hasMovementLeft;

        public Hexagon startHexagon;
        public Hexagon goalHexagon;

        [SerializeField] private UnitGroup unitGroup;
        [SerializeField] private float moveSpeed = 3;

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
            goalHexagon = currentHexagon;
            ResetForNextMovementStep();
        }

        private void ResetForNextMovementStep()
        {
            startHexagon = goalHexagon;
            _startPosition = transform.position;
            _movementProgress = 0;
            _assignedToNextHexagon = false;
        }

        public void Update()
        {
            if (!IsServer)
                return;

            if (hasMovementLeft && unitGroup.CanMove)
                Move();

            if (_movementProgress >= 1)
                SetupForMovementToNextGoal(true);
        }


        private void Move()
        {
            _movementProgress += Time.deltaTime * moveSpeed / _distanceToGoal;
            transform.position = Vector3.Lerp(_startPosition, goalHexagon.transform.position, _movementProgress);

            if (!_assignedToNextHexagon &&
                Vector3.Distance(goalHexagon.transform.position, transform.position) < MapBuilder.TileWidth * 0.5f)
            {
                ServerEvents.Unit.OnUnitGroupReachedNewHex?.Invoke(unitGroup, goalHexagon.Coordinates);
                _assignedToNextHexagon = true;
            }
        }

        private void OnWaypointsUpdated(List<Hexagon> waypoints)
        {
            if (!hasMovementLeft || waypoints[0] == startHexagon)
            {
                //Pretend to have reached goal and immediately startMovingToNext
                SetupForMovementToNextGoal(false);
            }
        }

        private void SetupForMovementToNextGoal(bool reachedHexCenter)
        {
            ResetForNextMovementStep();

            var newGoal = unitGroup.WaypointQueue.FetchWaypoint();
            hasMovementLeft = newGoal is not null;

            if (reachedHexCenter)
                ServerEvents.Unit.OnUnitGroupReachedHexCenter.Invoke(unitGroup, goalHexagon.Coordinates);

            if (hasMovementLeft)
                StartMovingToGoal(newGoal);
        }

        private void StartMovingToGoal(Hexagon goal)
        {
            ServerEvents.Unit.OnUnitGroupLeftHexCenter?.Invoke(unitGroup);
            goalHexagon = goal;
            var goalPosition = goal.transform.position;
            transform.rotation = Quaternion.LookRotation(goalPosition - transform.position);
            _distanceToGoal = Vector3.Distance(transform.position, goalPosition);
        }
    }
}