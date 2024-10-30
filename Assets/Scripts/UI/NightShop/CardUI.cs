using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.NightShop
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        
        private Card card;
        
        public bool isSelected = false;

        private void OnEnable()
        {
            GameEvents.NIGHT_SHOP.OnCardSelected += HandleCardSelected;
            GameEvents.NIGHT_SHOP.OnCardDeselected += HandleCardDeselected;
        }

        public void Initialize(Card card)
        {
            this.card = card;
            nameText.text = card.name;
            costText.text = "Cost: " + card.cost;
        }

        public void OnClick()
        {
            if (isSelected)
            {
                GameEvents.NIGHT_SHOP.OnCardDeselected();
                return;
            }
            GameEvents.NIGHT_SHOP.OnCardSelected(card);
        }

        void HandleCardSelected(Card card)
        {
            if (Equals(this.card, card))
            {
                SelectCard();
                return;
            }
            DeselectCard();
        }

        void HandleCardDeselected()
        {
            DeselectCard();
        }
        
        void SelectCard()
        {
            background.color = new Color(1f, 0.75f, 0.75f, 1);
            isSelected = true;
        }

        void DeselectCard()
        {
            background.color = Color.white;
            isSelected = false;
        }
    }
}
