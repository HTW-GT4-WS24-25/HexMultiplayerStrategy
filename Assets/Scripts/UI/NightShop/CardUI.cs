using System;
using GameEvents;
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
        public bool isDisabled = false;

        private void OnEnable()
        {
            ClientEvents.NightShop.OnCardSelected += HandleCardSelected;
            ClientEvents.NightShop.OnCardDeselected += HandleCardDeselected;
            ClientEvents.NightShop.OnMoneyAmountChanged += CheckIfPlayerHasEnoughMoney;
        }

        private void CheckIfPlayerHasEnoughMoney(int currentMoney)
        {
            isDisabled = currentMoney < card.cost;
            GetComponent<Button>().interactable = currentMoney >= card.cost;

            if (isSelected && isDisabled)
            {
                ClientEvents.NightShop.OnCardDeselected?.Invoke();
            }
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
                ClientEvents.NightShop.OnCardDeselected();
                return;
            }
            ClientEvents.NightShop.OnCardSelected(card);
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
