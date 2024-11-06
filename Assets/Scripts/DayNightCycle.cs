using System;
using Input;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private CycleState _cycleState = CycleState.Night;

    private void OnEnable()
    {
        inputReader.OnSwitchDayNightCycle += SwitchCycleState;
    }
    
    private void OnDisable()
    {
        inputReader.OnSwitchDayNightCycle -= SwitchCycleState;
    }

    private void SwitchCycleState()
    {
        if (_cycleState == CycleState.Day)
        {
            SwitchToNightTime();
        }
        else if(_cycleState == CycleState.Night)
        {
            SwitchToDayTime();
        }
    }
    
    private void SwitchToDayTime()
    {
        Debug.Log("SwitchToDayTime");
        _cycleState = CycleState.Day;
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }

    private void SwitchToNightTime()
    {
        Debug.Log("SwitchToNightTime");
        _cycleState = CycleState.Night;
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }

    public enum CycleState
    {
        Day,
        Night
    }
}
