using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit
{
    public class UnitGroupTravelLineDrawer : NetworkBehaviour
    {
        [SerializeField] private UnitGroupTravelLine travelLinePrefab;
        [SerializeField] private UnitGroup unitGroup;
        
        private UnitGroupTravelLine _travelLine;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            unitGroup.Movement.OnPathChanged += UpdateTravelLine;
            unitGroup.OnUnitHighlightEnabled += OnEnableHighlight;
            unitGroup.OnUnitHighlightDisabled += OnDisableHighlight;
            
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;
            
            unitGroup.Movement.OnPathChanged -= UpdateTravelLine;
            unitGroup.OnUnitHighlightEnabled -= OnEnableHighlight;
            unitGroup.OnUnitHighlightDisabled -= OnDisableHighlight;
        }

        private void UpdateTravelLine()
        {
            if (_travelLine is null)
                DrawLine(unitGroup.PlayerColor);
            
            var linePoints = new List<Vector3> { transform.position };
        
            linePoints.AddRange(
                unitGroup.WaypointQueue.GetCurrentAndNextWaypoints()
                    .Select(hex => hex.transform.position)
                );
            
            if (linePoints.Count > 1)
                UpdateFullTravelLineClientRpc(linePoints.ToArray());
            else
                DisableTravelLineClientRpc();
        }

        private void DrawLine(PlayerColor playerColor)
        {
            _travelLine = Instantiate(travelLinePrefab, transform);
            _travelLine.Initialize(playerColor);
        }

        [ClientRpc]
        private void UpdateFullTravelLineClientRpc(Vector3[] travelLinePoints)
        {
            if (unitGroup.PlayerId != NetworkManager.Singleton.LocalClientId)
                travelLinePoints = new [] { travelLinePoints[0], travelLinePoints[1] };
            
            _travelLine.SetAllPositions(travelLinePoints);
        }

        [ClientRpc]
        public void SyncFirstTravelLinePositionClientRpc()
        {
            _travelLine?.SetFirstNodePosition(transform.position);
        }
        
        [ClientRpc]
        private void DisableTravelLineClientRpc()
        {
            Destroy(_travelLine.gameObject);
        }

        private void OnEnableHighlight()
        {
            _travelLine.EnableHighlight();
        }

        private void OnDisableHighlight()
        {
            _travelLine.DisableHighlight();
        }
    }
}