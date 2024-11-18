using System.Collections;
using DG.Tweening;
using Input;
using Pinwheel.Jupiter;
using Unity.Netcode;
using UnityEngine;

public class DayNightCycle : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TurnTimeUI turnTimeUI;
    [SerializeField] private JDayNightCycle dayNightCycleSky;
    
    [Header("Settings")]
    [SerializeField] private float dayDuration = 20f;
    [SerializeField] private float nightDuration = 20f;
    [SerializeField] private float cycleSwitchAnimationDuration = 1.5f;
    [SerializeField] private float skyTimeForDay = 13f;

    private NetworkVariable<float> _turnTime = new();
    private CycleState _cycleState = CycleState.Night;
    private Tween _cycleSwitchTween;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            _turnTime.OnValueChanged += HandleTurnTimeChanged;
            
        if (IsServer)
        {
            StartCoroutine(TimerRoutine());
            inputReader.OnSwitchDayNightCycle += SwitchCycleState;
            
            SwitchToNightTimeClientRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
            _turnTime.OnValueChanged -= HandleTurnTimeChanged;
        
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

            if (_cycleState == CycleState.Day && _turnTime.Value >= dayDuration)
            {
                SwitchToNightTimeClientRpc();
                _turnTime.Value = 0;
            }

            if (_cycleState == CycleState.Night && _turnTime.Value >= nightDuration)
            {
                SwitchToDayTimeClientRpc();
                _turnTime.Value = 0;
            }
            
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
        
        _cycleSwitchTween?.Kill();
        var skyTimeForNight = dayNightCycleSky.Time;
        _cycleSwitchTween = DOVirtual.Float(0, 12f, cycleSwitchAnimationDuration, time => dayNightCycleSky.Time = (skyTimeForNight + time) % 24f).SetEase(Ease.OutCirc);
        
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }

    [ClientRpc]
    private void SwitchToNightTimeClientRpc()
    {
        Debug.Log("SwitchToNightTime");
        _cycleState = CycleState.Night;
        
        _cycleSwitchTween?.Kill();
        _cycleSwitchTween = DOVirtual.Float(0, 12f, cycleSwitchAnimationDuration, time => dayNightCycleSky.Time = (skyTimeForDay + time) % 24f).SetEase(Ease.OutCirc);
        
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState?.Invoke(_cycleState);
    }
    
    private void HandleTurnTimeChanged(float previousValue, float newValue)
    {
        turnTimeUI.UpdateTurnTime(newValue, _cycleState == CycleState.Day ? dayDuration : nightDuration);
    }

    #endregion

    public enum CycleState
    {
        Day,
        Night
    }
}