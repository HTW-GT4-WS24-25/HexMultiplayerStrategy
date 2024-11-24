using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerMoneyText;
    
        private void OnEnable()
        {
            GameEvents.NIGHT_SHOP.OnMoneyAmountChanged += DisplayMoneyAmount;
        }

        void DisplayMoneyAmount(int amount)
        {
            playerMoneyText.text = "Gold: " + amount;
        }
    }
}
