using System;
using Core.PlayerData;
using UnityEngine;

namespace Core.HexSystem.Hex
{
    public class Topping : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabsByLevel;
        [SerializeField] private bool isTraversable;

        private GameObject _currentToppingModel;

        private void Start()
        {
            SetLevel(1);
        }

        public void AdaptModelToPlayerColor(PlayerColor playerColor)
        {
            throw new NotImplementedException();
        }

        public void SetLevel(int level)
        {
            if(_currentToppingModel != null)
                Destroy(_currentToppingModel);
            
            _currentToppingModel = Instantiate(prefabsByLevel[level - 1], transform.position, transform.rotation, transform);
        }
    }
}