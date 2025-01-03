using System.Collections.Generic;
using System.Linq;
using Core.Buildings;
using Core.Combat;
using Core.GameEvents;
using Core.HexSystem;
using Core.HexSystem.Generation;
using Core.Input;
using Core.Score;
using Core.Unit;
using ExtensionMethods;
using Helper;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Core.Startup
{
    public class MatchStartup : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private UnitPlacement unitPlacement;
        [SerializeField] private BuildingPlacement buildingPlacement;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GridData hexGridData;
        [SerializeField] private ScoreUpdater scoreUpdater;
        [SerializeField] private DayNightCycle.DayNightCycle dayNightCycle;
        [SerializeField] private HexControlObserver hexControlObserver;
        [SerializeField] private CombatController combatController;

        [Header("Settings")]
        [Tooltip("X will be set to Q, Y will be set to R")]
        [SerializeField] private Vector2Int[] startCoordinates;
        [SerializeField] private int numberOfMapRings;
        [SerializeField] private MapGenerationConfig mapGenerationConfig;
        [SerializeField] private MatchConfiguration matchConfiguration;

        private List<AxialCoordinates> _remainingStartCoordinates;
        private int _playersInMatch;
        private int _numberOfConnectedPlayers;

        #region Server

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
        
            hexControlObserver.InitializeOnServer();
            combatController.InitializeOnServer();
        
            HostSingleton.Instance.GameManager.MatchConfig = matchConfiguration;
            ApplyMatchConfigurationClientRpc(matchConfiguration);
        
            _numberOfConnectedPlayers = NetworkManager.Singleton.ConnectedClients.Count;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += HandleClientFinishedLoadingScene;
        
            _remainingStartCoordinates = new List<AxialCoordinates>();
            foreach (var startCoordinate in startCoordinates)
            {
                _remainingStartCoordinates.Add(new AxialCoordinates(startCoordinate.x, startCoordinate.y));
            }
        }

        private void HandleClientFinishedLoadingScene(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"Scene Loaded for Player{_playersInMatch}/{_numberOfConnectedPlayers} {clientId} at time: {Time.time}");

            if (++_playersInMatch == _numberOfConnectedPlayers)
            {
                CreateMap();
                SetPlayerStartPositions();
                dayNightCycle.StartMatch();
            }
        }
    
        private void CreateMap()
        {
            var mapGenerator = new MapDataGenerator(mapGenerationConfig);
            var mapData = mapGenerator.Generate(
                numberOfMapRings, 
                startCoordinates.Select(coord => VectorExtensions.ToAxialCoordinates(coord)).ToList());
            mapBuilder.BuildMapForAll(mapData, numberOfMapRings);
        
            SetupHexGridDataClientRpc(mapData);
            var scoreCalculator = new ScoreCalculator(hexGridData);
            scoreUpdater.Initialize(scoreCalculator);
        }

        private void SetPlayerStartPositions()
        {
            unitPlacement.Initialize(mapBuilder.Grid, hexGridData);
            buildingPlacement.Initialize(hexGridData);

            var players = HostSingleton.Instance.GameManager.GetPlayers();
            foreach (var player in players)
            {
                var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
                _remainingStartCoordinates.Remove(playerStartCoordinate);
                var playerStartHex = mapBuilder.Grid.Get(playerStartCoordinate);
            
                ServerEvents.Player.OnInitialPlayerUnitsPlaced!.Invoke(playerStartCoordinate, player.ClientId);
                unitPlacement.TryAddUnitsToHex(playerStartCoordinate, player, player.Faction.StartUnitCount);
            
                var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(player.ClientId);
                AlignCameraToStartClientRpc(playerStartHex.transform.position, clientRpcParams);
            }
        }

        #endregion

        #region Client

        [ClientRpc]
        private void AlignCameraToStartClientRpc(Vector3 startPosition, ClientRpcParams clientRpcParams)
        {
            cameraController.PositionCameraOnPlayerStartPosition(startPosition, Vector3.zero);
        }

        [ClientRpc]
        private void SetupHexGridDataClientRpc(int[] mapData)
        {
            hexGridData.SetupNewData(mapData, numberOfMapRings);

            if (!IsHost) 
                return;
            
            foreach (var player in HostSingleton.Instance.GameManager.GetPlayers())
            {
                player.GridData = hexGridData;
            }
        }
    
        [ClientRpc]
        private void ApplyMatchConfigurationClientRpc(MatchConfiguration matchConfigurationFromHost)
        {
            dayNightCycle.DayDuration = matchConfigurationFromHost.dayDuration;
            dayNightCycle.NightDuration = matchConfigurationFromHost.nightDuration;
            dayNightCycle.NightsPerMatch = matchConfigurationFromHost.nightsPerMatch;
        }

        #endregion
    }
}