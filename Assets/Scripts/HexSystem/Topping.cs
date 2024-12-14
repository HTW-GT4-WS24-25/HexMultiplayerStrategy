using System;
using Player;
using UnityEngine;

namespace HexSystem
{
    public class Topping : MonoBehaviour
    {
        [SerializeField] private bool isTraversable;
        
        public bool IsTraversable => isTraversable;

        public void AdaptModelToPlayerColor(PlayerColor playerColor)
        {
            throw new NotImplementedException();
        }
    }
}