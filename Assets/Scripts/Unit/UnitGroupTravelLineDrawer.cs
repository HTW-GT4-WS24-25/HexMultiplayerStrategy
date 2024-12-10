using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    public class UnitGroupTravelLineDrawer : NetworkBehaviour
    {
        [SerializeField] private UnitGroupTravelLine travelLinePrefab;
        [SerializeField] private UnitGroup unitGroup;
        
        private UnitGroupTravelLine _travelLine;

        public override void OnNetworkSpawn()
        {
            unitGroup.OnUnitHighlightEnabled += OnEnableHighlight;
            unitGroup.OnUnitHighlightDisabled += OnDisableHighlight;
            
            if (!IsServer)
                return;
            
            unitGroup.WaypointQueue.OnPathChanged += UpdateTravelLine;
        }

        public override void OnNetworkDespawn()
        {
            unitGroup.OnUnitHighlightEnabled -= OnEnableHighlight;
            unitGroup.OnUnitHighlightDisabled -= OnDisableHighlight;
            
            if (!IsServer)
                return;
            
            unitGroup.WaypointQueue.OnPathChanged -= UpdateTravelLine;
        }

        #region Server

        private void UpdateTravelLine()
        {
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

        #endregion

        #region Client
        
        public void InitializeTravelLine(PlayerColor playerColor)
        {
            _travelLine = Instantiate(travelLinePrefab, transform.position, Quaternion.identity);
            _travelLine.Initialize(playerColor);
            _travelLine.gameObject.SetActive(false);
        }
        
        public void Update()
        {
            if (_travelLine.gameObject.activeSelf)
                SyncFirstTravelLinePosition();
        }

        [ClientRpc]
        private void UpdateFullTravelLineClientRpc(Vector3[] travelLinePoints)
        {
            if (unitGroup.PlayerId != NetworkManager.Singleton.LocalClientId)
                travelLinePoints = new [] { travelLinePoints[0], travelLinePoints[1] };
            
            _travelLine.gameObject.SetActive(true);
            _travelLine.SetAllPositions(travelLinePoints);
        }
        
        [ClientRpc]
        private void DisableTravelLineClientRpc()
        {
            _travelLine.gameObject.SetActive(false);
        }
        
        private void SyncFirstTravelLinePosition()
        {
            _travelLine?.SetFirstNodePosition(transform.position);
        }

        private void OnEnableHighlight()
        {
            _travelLine?.EnableHighlight();
        }

        private void OnDisableHighlight()
        {
            _travelLine?.DisableHighlight();
        }

        #endregion
    }
}