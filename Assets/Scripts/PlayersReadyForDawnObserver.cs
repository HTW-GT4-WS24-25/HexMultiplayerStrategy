using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Networking.Host;
using Unity.Netcode;

public class PlayersReadyForDawnObserver : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        ClientEvents.NightShop.OnLocalPlayerChangedReadyForDawnState += LocalPlayerReadyStateChanged;
        
        if(!IsServer)
            return;
        ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleCycleStateSwitched;
        
        HandleCycleStateSwitched(DayNightCycle.CycleState.Night);
    }

    public override void OnNetworkDespawn()
    {
        ClientEvents.NightShop.OnLocalPlayerChangedReadyForDawnState -= LocalPlayerReadyStateChanged;
        
        if(!IsServer)
            return;
        ClientEvents.DayNightCycle.OnSwitchedCycleState -= HandleCycleStateSwitched;
    }

    private readonly Dictionary<ulong, bool> playerReadyStates = new();
    private int playersReadyForDawn = 0;

    #region Server

    [Rpc(SendTo.Server)]
    private void HandlePlayerReadyStateChangedRpc(ulong playerId, bool ready)
    {
        playerReadyStates[playerId] = ready;
        var updatedReadyPlayerCount = playerReadyStates.Count(player => player.Value);
        if (updatedReadyPlayerCount == playersReadyForDawn)
            return;
        
        playersReadyForDawn = updatedReadyPlayerCount;
        UpdateReadyForDawnPlayersClientRpc(playersReadyForDawn, playerReadyStates.Count);
        
        if(playersReadyForDawn == playerReadyStates.Count)
            ServerEvents.NightShop.OnAllPlayersReadyForDawn?.Invoke();
    }

    private void HandleCycleStateSwitched(DayNightCycle.CycleState newCycleState)
    {
        if (newCycleState != DayNightCycle.CycleState.Night)
            return;
        
        playerReadyStates.Clear();
        playersReadyForDawn = 0;
        var players = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
        players.ForEach(player => playerReadyStates.Add(player.ClientId, false));
        UpdateReadyForDawnPlayersClientRpc(playersReadyForDawn, players.Count);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void UpdateReadyForDawnPlayersClientRpc(int readyPlayers, int maxPlayers)
    {
        ClientEvents.NightShop.OnReadyPlayersChanged?.Invoke(readyPlayers, maxPlayers);
    }
    
    private void LocalPlayerReadyStateChanged(bool isReadyForDawn)
    {
        HandlePlayerReadyStateChangedRpc(NetworkManager.LocalClientId, isReadyForDawn);
    }

    #endregion
}
