using System.Collections.Generic;
using System.Linq;
using Buildings;
using Combat;
using ExtensionMethods;
using GameEvents;
using Helper;
using HexSystem;
using Input;
using Networking.Host;
using Score;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MatchStartup : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private MapBuilder mapBuilder;
    [SerializeField] private UnitPlacement unitPlacement;
    [SerializeField] private BuildingPlacement buildingPlacement;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GridData hexGridData;
    [SerializeField] private ScoreUpdater scoreUpdater;
    [SerializeField] private DayNightCycle dayNightCycle;
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
            startCoordinates.Select(coord => coord.ToAxialCoordinates()).ToList());
        mapBuilder.BuildMapForAll(mapData, numberOfMapRings);
        
        SetupHexGridDataClientRpc(mapData);
        var scoreCalculator = new ScoreCalculator(hexGridData);
        scoreUpdater.Initialize(scoreCalculator);
    }

    private void SetPlayerStartPositions()
    {
        unitPlacement.Initialize(mapBuilder.Grid, hexGridData);
        buildingPlacement.Initialize(mapBuilder.Grid, hexGridData);
        
        var players = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
        foreach (var playerData in players)
        {
            var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
            _remainingStartCoordinates.Remove(playerStartCoordinate);
            var playerStartHex = mapBuilder.Grid.Get(playerStartCoordinate);
            
            ServerEvents.Player.OnInitialPlayerUnitsPlaced!.Invoke(playerStartCoordinate, playerData.ClientId);
            unitPlacement.TryAddUnitsToHex(playerStartCoordinate, playerData, 30);
            
            var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(playerData.ClientId);
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