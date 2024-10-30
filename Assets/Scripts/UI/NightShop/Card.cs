using UnityEngine;

namespace UI.NightShop
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Create Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public int cost;
    }  
}

