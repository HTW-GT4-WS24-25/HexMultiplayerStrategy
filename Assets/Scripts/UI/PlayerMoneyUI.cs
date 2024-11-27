using GameEvents;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerMoneyText;
    
        private void OnEnable()
        {
            ClientEvents.NightShop.OnMoneyAmountChanged += DisplayMoneyAmount;
        }

        void DisplayMoneyAmount(int amount)
        {
            playerMoneyText.text = "Gold: " + amount;
        }
    }
}
