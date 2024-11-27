using System.Collections;
using DG.Tweening;
using GameEvents;
using Input;
using Pinwheel.Jupiter;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TurnTimeUI turnTimeUI;
    [SerializeField] private JDayNightCycle dayNightCycleSky;
    
    [Header("Settings")]
    [SerializeField] private float cycleSwitchAnimationDuration = 1.5f;
    [SerializeField] private float skyTimeForDay = 13f;

    public CycleState cycleState = CycleState.Night;
    public float DayDuration { get; set; }
    public float NightDuration { get; set; }
    public int NightsPerMatch { get; set; }
    
    private readonly NetworkVariable<float> _turnTime = new();
    private Tween _cycleSwitchTween;
    private int _nightsThisGame;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            _turnTime.OnValueChanged += HandleTurnTimeChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
            _turnTime.OnValueChanged -= HandleTurnTimeChanged;
    }
    
    #region Server

    public void StartMatch()
    {
        StartCoroutine(DayNightTimerRoutine());
        
        SwitchToNightTimeClientRpc();
    }

    private IEnumerator DayNightTimerRoutine()
    {
        while (true)
        {
            _turnTime.Value += Time.deltaTime;

            if (cycleState == CycleState.Day && _turnTime.Value >= DayDuration)
            {
                SwitchToNightTimeClientRpc();
                _turnTime.Value = 0;

                if (++_nightsThisGame == NightsPerMatch)
                {
                    ServerEvents.DayNightCycle.OnGameEnded?.Invoke();
                    break;
                }
                
                ServerEvents.DayNightCycle.OnTurnEnded?.Invoke();
            }

            if (cycleState == CycleState.Night && _turnTime.Value >= NightDuration)
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
        cycleState = CycleState.Day;
        
        _cycleSwitchTween?.Kill();
        var skyTimeForNight = dayNightCycleSky.Time;
        _cycleSwitchTween = DOVirtual.Float(0, 12f, cycleSwitchAnimationDuration, time => dayNightCycleSky.Time = (skyTimeForNight + time) % 24f).SetEase(Ease.OutCirc);
        
        ClientEvents.DayNightCycle.OnSwitchedCycleState?.Invoke(cycleState);
    }

    [ClientRpc]
    private void SwitchToNightTimeClientRpc()
    {
        Debug.Log("SwitchToNightTime");
        cycleState = CycleState.Night;
        
        _cycleSwitchTween?.Kill();
        _cycleSwitchTween = DOVirtual.Float(0, 12f, cycleSwitchAnimationDuration, time => dayNightCycleSky.Time = (skyTimeForDay + time) % 24f).SetEase(Ease.OutCirc);
        
        ClientEvents.DayNightCycle.OnSwitchedCycleState?.Invoke(cycleState);
    }
    
    private void HandleTurnTimeChanged(float previousValue, float newValue)
    {
        turnTimeUI.UpdateTurnTime(newValue, cycleState == CycleState.Day ? DayDuration : NightDuration);
    }

    #endregion

    public enum CycleState
    {
        Day,
        Night
    }
}