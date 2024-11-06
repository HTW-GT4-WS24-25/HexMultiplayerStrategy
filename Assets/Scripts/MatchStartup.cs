using System.Collections.Generic;
using Helper;
using HexSystem;
using Input;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MatchStartup : NetworkBehaviour
{
    [SerializeField] MapBuilder mapBuilder;
    [SerializeField] MapGenerationConfig mapGenerationConfig;
    [SerializeField] UnitPlacement unitPlacement;
    [SerializeField] CameraController cameraController;
    
    [Tooltip("X will be set to Q, Y will be set to R")]
    [SerializeField] private Vector2Int[] startCoordinates;

    private List<AxialCoordinates> _remainingStartCoordinates;
    private int _playersInMatch;
    private int _connectedPlayersAmount;
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        _connectedPlayersAmount = NetworkManager.Singleton.ConnectedClients.Count;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += HandleClientFinishedLoadingScene;
        
        _remainingStartCoordinates = new List<AxialCoordinates>();
        foreach (var startCoordinate in startCoordinates)
        {
            _remainingStartCoordinates.Add(new AxialCoordinates(startCoordinate.x, startCoordinate.y));
        }
    }

    private void HandleClientFinishedLoadingScene(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"Scene Loaded for Player{_playersInMatch}/{_connectedPlayersAmount} {clientId} at time: {Time.time}");

        if (++_playersInMatch == HostSingleton.Instance.GameManager.PlayerData.GetPlayerList().Count)
        {
            CreateMap();
            SetPlayerStartPositions();
        }
    }
    
    private void CreateMap()
    {
        const int nRings = 3;

        var mapGenerator = new MapDataGenerator(mapGenerationConfig);
        var mapData = mapGenerator.Generate(nRings);
        mapBuilder.BuildMapForAll(mapData, nRings);
    }

    private void SetPlayerStartPositions()
    {
        var players = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
        foreach (var playerData in players)
        {
            var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
            _remainingStartCoordinates.Remove(playerStartCoordinate);

            var playerStartHex = mapBuilder.Grid.Get(playerStartCoordinate);
            unitPlacement.TryAddUnitsToHex(playerStartHex, playerData);
            
            var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(playerData.ClientId);
            AlignCameraToStartClientRpc(playerStartHex.transform.position, clientRpcParams);
        }
    }

    [ClientRpc]
    private void AlignCameraToStartClientRpc(Vector3 startPosition, ClientRpcParams clientRpcParams)
    {
        cameraController.PositionCameraOnPlayerStartPosition(startPosition, Vector3.zero);
    }
}