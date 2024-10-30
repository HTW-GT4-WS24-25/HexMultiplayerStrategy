using System;
using System.Collections.Generic;
using UI.NightShop;
using Unity.VisualScripting;
using UnityEngine;

public class NightShopUI : MonoBehaviour
{
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private CardUI cardPrefab;
    
    public void Initialize(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            AddCardToShop(card);
        }
    }

    void AddCardToShop(Card card)
    {
        CardUI cardInstance = Instantiate(cardPrefab, cardContainer.transform);
        cardInstance.Initialize(card);
    }
}
