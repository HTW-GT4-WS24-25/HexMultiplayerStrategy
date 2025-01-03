using Core.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.InGame.NightShop
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private Image innerBackground;
        [SerializeField] private Image focus;
        [SerializeField] private Image focusGlow;
        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        
        private Card card;
        
        public bool isSelected;
        public bool isDisabled;

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
                ClientEvents.NightShop ?.OnCardDeselected();
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
            innerBackground.gameObject.SetActive(false);
            focus.gameObject.SetActive(true);
            focusGlow.gameObject.SetActive(true);
            
            isSelected = true;
        }

        void DeselectCard()
        {
            innerBackground.gameObject.SetActive(true);
            focus.gameObject.SetActive(false);
            focusGlow.gameObject.SetActive(false);
            
            isSelected = false;
        }
    }
}
