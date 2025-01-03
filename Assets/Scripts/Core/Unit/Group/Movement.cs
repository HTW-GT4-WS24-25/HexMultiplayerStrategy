using System.Collections;
using Core.HexSystem.Generation;
using Core.HexSystem.Hex;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unit.Group
{
    public class Movement : NetworkBehaviour
    {
        [SerializeField] private WaypointQueue waypointQueue;
        [SerializeField] private UnityEvent<Hexagon> onReachedNewHex;
        [SerializeField] private UnityEvent<Hexagon> onReachedHexCenter;
        [SerializeField] private UnityEvent onLeftHexCenter;
        
        public UnityAction<float> OnMoveAnimationSpeedChanged;
        public bool HasMovementLeft { get; private set; }
        public Hexagon StartHexagon { get; private set; }
        public Hexagon GoalHexagon { get; private set; }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                _moveSpeed = value;
                if(HasMovementLeft && !_isPaused)
                    OnMoveAnimationSpeedChangedClientRpc(_moveSpeed);
            }
        }

        private float _moveSpeed;
        private Vector3 _startPosition;
        private bool _assignedToNextHexagon;
        private float _movementProgress;
        private float _distanceToGoal;
        private bool _isPaused;
        private bool _standsOnHexCenter;
        private Coroutine _moveRoutine;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            waypointQueue.OnWaypointsUpdated += OnWaypointsUpdated;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;

            waypointQueue.OnWaypointsUpdated -= OnWaypointsUpdated;
        }

        #region Server

        public void InitializeAsStationary(Hexagon currentHexagon)
        {
            Initialize(currentHexagon);
            _standsOnHexCenter = true;
        }

        public void Initialize(Hexagon currentHexagon)
        {
            GoalHexagon = currentHexagon;
            ResetForNextMovementStep();
        }

        public void SetPaused(bool pause)
        {
            _isPaused = pause;
            
            if (!_isPaused && HasMovementLeft)
            {
                _moveRoutine = StartCoroutine(MoveToGoal());
                transform.rotation = Quaternion.LookRotation(GoalHexagon.transform.position - transform.position);
                OnMoveAnimationSpeedChangedClientRpc(_moveSpeed);
            }
            else if (_isPaused && _moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                _moveRoutine = null;
                
                OnMoveAnimationSpeedChangedClientRpc(0);
            }
        }
        
        private void OnWaypointsUpdated()
        {
            Debug.Assert(!_isPaused, "Waypoints were updated while UnitGroupMovement was set to paused!");

            if (!HasMovementLeft)
            {
                StartMovingToNextWaypoint();
                HasMovementLeft = true;
                OnMoveAnimationSpeedChangedClientRpc(_moveSpeed);
                
                if(_standsOnHexCenter)
                {
                    onLeftHexCenter?.Invoke();
                    _standsOnHexCenter = false;
                }
            }
            else if (waypointQueue.PeekNextWaypoint() == StartHexagon)
            {
                SwitchGoalToNextWaypoint();
            }
        }
        
        private void SwitchGoalToNextWaypoint()
        {
            StopCoroutine(_moveRoutine);
            StartMovingToNextWaypoint();
        }
        
        private void StartMovingToNextWaypoint()
        {
            ResetForNextMovementStep();
            GoalHexagon = waypointQueue.FetchWaypoint();
            
            var goalPosition = GoalHexagon.transform.position;
            transform.rotation = Quaternion.LookRotation(goalPosition - transform.position);
            _distanceToGoal = Vector3.Distance(transform.position, goalPosition);
            
            _moveRoutine = StartCoroutine(MoveToGoal());
        }

        private IEnumerator MoveToGoal()
        {
            while (_movementProgress < 1)
            {
                ProcessMovementFrame();
                yield return null;
            }
            
            _moveRoutine = null;
            OnGoalReached();
        }

        private void ProcessMovementFrame()
        {
            _movementProgress += Time.deltaTime * _moveSpeed / _distanceToGoal;
            transform.position = Vector3.Lerp(_startPosition, GoalHexagon.transform.position, _movementProgress);
            
            if (!_assignedToNextHexagon &&
                Vector3.Distance(GoalHexagon.transform.position, transform.position) < MapBuilder.TileWidth * 0.5f)
            {
                onReachedNewHex!.Invoke(GoalHexagon);
                _assignedToNextHexagon = true;
            }
        }

        private void OnGoalReached()
        {
            HasMovementLeft = waypointQueue.PeekNextWaypoint() is not null; 
            onReachedHexCenter!.Invoke(GoalHexagon);
            
            if (HasMovementLeft)
            {
                StartMovingToNextWaypoint();
                onLeftHexCenter!.Invoke();
            }
            else
            {
                waypointQueue.Clear();
                _standsOnHexCenter = true;
                OnMoveAnimationSpeedChangedClientRpc(0);
            }
        }
        
        private void ResetForNextMovementStep()
        {
            StartHexagon = GoalHexagon;
            _startPosition = transform.position;
            _movementProgress = 0;
            _assignedToNextHexagon = false;
        }

        #endregion

        #region Client

        [ClientRpc]
        private void OnMoveAnimationSpeedChangedClientRpc(float value)
        {
            OnMoveAnimationSpeedChanged?.Invoke(value);
        }
        
        #endregion
    }
}