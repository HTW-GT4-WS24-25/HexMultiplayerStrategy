using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.InGame.NightShop
{
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
}
