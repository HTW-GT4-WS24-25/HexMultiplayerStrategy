using System;
using System.Collections;
using Input;
using Unity.Netcode;
using UnityEngine;

public class DayNightCycle : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;

    private NetworkVariable<float> _turnTime;
    private CycleState _cycleState = CycleState.Night;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            _turnTime.OnValueChanged += HandleTurnTimeChanged;
            
        if (IsServer)
        {
            StartCoroutine(TimerRoutine());
            inputReader.OnSwitchDayNightCycle += SwitchCycleState;
        }
    }

    public override void OnNetworkDespawn()
    {

        if (IsServer)
        {
            inputReader.OnSwitchDayNightCycle -= SwitchCycleState;
        }
    }
    
    #region Server

    private void SwitchCycleState()
    {
        if (_cycleState == CycleState.Day)
        {
            SwitchToNightTimeClientRpc();
        }
        else if(_cycleState == CycleState.Night)
        {
            SwitchToDayTimeClientRpc();
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            _turnTime.Value += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Client

    [ClientRpc]
    private void SwitchToDayTimeClientRpc()
    {
        Debug.Log("SwitchToDayTime");
        _cycleState = CycleState.Day;
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }

    [ClientRpc]
    private void SwitchToNightTimeClientRpc()
    {
        Debug.Log("SwitchToNightTime");
        _cycleState = CycleState.Night;
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }
    
    private void HandleTurnTimeChanged(float previousvalue, float newvalue)
    {
        throw new NotImplementedException();
    }

    #endregion

    public enum CycleState
    {
        Day,
        Night
    }
}
