using System.Collections.Generic;
using Helper;
using HexSystem;
using Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MatchStartup : NetworkBehaviour
{
    [SerializeField] MapCreator mapCreator;
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
        var rings = 3;
        
        var mapData = mapCreator.GenerateRandomMapData(rings);
        mapCreator.BuildMap(mapData, rings);
    }

    private void SetPlayerStartPositions()
    {
        var players = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
        foreach (var playerData in players)
        {
            var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
            _remainingStartCoordinates.Remove(playerStartCoordinate);

            var playerStartHex = mapCreator.Grid.Get(playerStartCoordinate);
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