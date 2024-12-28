using Core.NightShop.Placeables;
using UnityEngine;

namespace Core.UI.InGame.NightShop
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public int cost;
        public Placeable placeable;
    }  
}

