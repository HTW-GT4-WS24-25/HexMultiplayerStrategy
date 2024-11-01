using System.Collections.Generic;
using HexSystem;
using Input;
using UnityEngine;
using Random = UnityEngine.Random;

public class SessionStartup : MonoBehaviour
{
    [SerializeField] MapCreator mapCreator;
    [SerializeField] UnitPlacement unitPlacement;
    [SerializeField] CameraController cameraController;
    [Tooltip("X will be set to Q, Y will be set to R")]
    [SerializeField] private Vector2Int[] startCoordinates;

    private List<AxialCoordinate> _remainingStartCoordinates;
    
    private void Start() // TODO: Replace with OnNetworkSpawn and Server only
    {
        _remainingStartCoordinates = new List<AxialCoordinate>();
        foreach (var startCoordinate in startCoordinates)
        {
            _remainingStartCoordinates.Add(new AxialCoordinate(startCoordinate.x, startCoordinate.y));
        }
        
        Debug.Log("The Start Coordinates are:");
        foreach (var remainingStartCoordinate in _remainingStartCoordinates)
        {
            Debug.Log($"{remainingStartCoordinate.Q}, {remainingStartCoordinate.R}");
        }
        CreateMap();
        SetPlayerStartPosition();
    }
    
    // Create map
    private void CreateMap()
    {
        mapCreator.CreateRingMap(3);
    }
    
    // Put player on start position
    private void SetPlayerStartPosition()
    {
        var playerStartCoordinate = _remainingStartCoordinates[Random.Range(0, _remainingStartCoordinates.Count)];
        _remainingStartCoordinates.Remove(playerStartCoordinate);

        var playerStartHex = mapCreator.Grid.Get(playerStartCoordinate);
        unitPlacement.TryAddUnitsToHex(playerStartHex);
        cameraController.PositionCameraOnPlayerStartPosition(playerStartHex.transform.position, Vector3.zero);
    }
    
    
    // Align camera controller to start position
}