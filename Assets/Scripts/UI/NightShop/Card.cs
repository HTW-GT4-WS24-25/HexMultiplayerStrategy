using NightShop.Placeables;
using UnityEngine;

namespace UI.NightShop
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public int cost;
        public Placeable placeable;
    }  
}

