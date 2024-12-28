using Core.GameEvents;
using TMPro;
using UnityEngine;

namespace Core.UI.InGame.NightShop
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerMoneyText;
    
        private void OnEnable()
        {
            ClientEvents.NightShop.OnMoneyAmountChanged += DisplayMoneyAmount;
        }

        private void OnDisable()
        {
            ClientEvents.NightShop.OnMoneyAmountChanged -= DisplayMoneyAmount;
        }

        void DisplayMoneyAmount(int amount)
        {
            playerMoneyText.text = "Gold: " + amount;
        }
    }
}
