using System;
using System.Collections.Generic;
using HexSystem;
using UI.NightShop;
using UnityEngine;

public class NightShopManager : MonoBehaviour
{
    [SerializeField] private NightShopUI nightShopUI;
    [SerializeField] private List<Card> cards;
    
    [SerializeField] private Card selectedCard;
    [SerializeField] private Hexagon selectedHexagon;
    
    private void Start()
    {
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToDay += CloseNightShop;
        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += OpenNightShop;
        
        GameEvents.NIGHT_SHOP.OnCardSelected += HandleSelectedCard;
        GameEvents.NIGHT_SHOP.OnCardDeselected += HandleDeselectedCard;

        GameEvents.INPUT.OnHexSelectedDuringNightShop += hexagon => { selectedHexagon = hexagon; };
        
        nightShopUI.Initialize(cards);
    }

    void OpenNightShop()
    {
        nightShopUI.transform.gameObject.SetActive(true);
    }

    void CloseNightShop()
    {
        nightShopUI.transform.gameObject.SetActive(false);
        GameEvents.NIGHT_SHOP.OnCardDeselected();
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
