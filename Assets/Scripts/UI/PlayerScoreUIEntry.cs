using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerScoreUIEntry : MonoBehaviour
    {
        [SerializeField] private Image playerColorIcon; 
        [SerializeField] private TextMeshProUGUI playerScoreText;

        public void Initialize(Color color, string nameAndScore)
        {
            playerColorIcon.color = color;
            playerScoreText.text = nameAndScore;
        }
    }
}
