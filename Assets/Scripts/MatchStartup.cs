using System.Collections.Generic;
using Helper;
using HexSystem;
using Input;
using Networking.Host;
using Score;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MatchStartup : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private MapBuilder mapBuilder;
    [SerializeField] private MapGenerationConfig mapGenerationConfig;
    [SerializeField] private UnitPlacement unitPlacement;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GridData hexGridData;
    [SerializeField] private HexControlAndCombatObserver hexControlAndCombatObserver;
    
    [Header("Settings")]
    [Tooltip("X will be set to Q, Y will be set to R")]
    [SerializeField] private Vector2Int[] startCoordinates;
    [SerializeField] private int numberOfMapRings;

    private List<AxialCoordinates> _remainingStartCoordinates;
    private int _playersInMatch;
    private int _numberOfConnectedPlayers;
    private ScoreCalculator _scoreCalculator;

    #region Server

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        hexControlAndCombatObserver.InitializeOnServer();
        
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
        }
    }
    
    private void CreateMap()
    {
        var mapGenerator = new MapDataGenerator(mapGenerationConfig);
        var mapData = mapGenerator.Generate(numberOfMapRings);
        mapBuilder.BuildMapForAll(mapData, numberOfMapRings);
        
        SetupHexGridDataClientRpc();
        _scoreCalculator = new ScoreCalculator(hexGridData);
    }

    private void SetPlayerStartPositions()
    {
        unitPlacement.Initialize(mapBuilder.Grid, hexGridData);
        
        var players = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
        foreach (var playerData in players)
        {
            var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
            _remainingStartCoordinates.Remove(playerStartCoordinate);
            var playerStartHex = mapBuilder.Grid.Get(playerStartCoordinate);
            
            GameEvents.NETWORK_SERVER.OnHexControllerChanged!.Invoke(playerStartHex, playerData);
            unitPlacement.TryAddUnitsToHex(playerStartCoordinate, playerData, 10);
            
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
    private void SetupHexGridDataClientRpc()
    {
        hexGridData.SetupNewData(numberOfMapRings);
    }

    #endregion
}