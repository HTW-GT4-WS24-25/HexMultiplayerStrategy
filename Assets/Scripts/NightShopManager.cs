using System;
using System.Collections.Generic;
using HexSystem;
using UI.NightShop;
using UnityEngine;
using UnityEngine.Serialization;

public class NightShopManager : MonoBehaviour
{
    [SerializeField] private NightShopUI nightShopUI;
    [SerializeField] private List<Card> cards;
    
    [SerializeField] private Card selectedCard;
    [SerializeField] private Hexagon selectedHexagon;
    
    private void Start()
    {
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightSwitched;
        
        GameEvents.NIGHT_SHOP.OnCardSelected += HandleSelectedCard;
        GameEvents.NIGHT_SHOP.OnCardDeselected += HandleDeselectedCard;

        GameEvents.INPUT.OnHexSelectedDuringNightShop += hexagon => { selectedHexagon = hexagon; };
        
        nightShopUI.Initialize(cards);
    }

    private void HandleDayNightSwitched(DayNightCycle.CycleState newDayNightCycle)
    {
        switch (newDayNightCycle)
        {
            case DayNightCycle.CycleState.Day:
                nightShopUI.transform.gameObject.SetActive(false);
                GameEvents.NIGHT_SHOP.OnCardDeselected();
                break;
            case DayNightCycle.CycleState.Night:
                nightShopUI.transform.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentException("Invalid DayNightCycle state");
        }
    }

    void HandleSelectedCard(Card card)
    {
        selectedCard = card;
    }

    void HandleDeselectedCard()
    {
        selectedCard = null;
    }
}
