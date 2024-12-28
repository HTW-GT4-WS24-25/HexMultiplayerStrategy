using System;
using Player;
using UnityEngine;

namespace HexSystem
{
    public class Topping : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabsByLevel;
        [SerializeField] private bool isTraversable;

        private GameObject _currentTopping;

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
            if(_currentTopping != null)
                Destroy(_currentTopping);
            
            _currentTopping = Instantiate(prefabsByLevel[level - 1], transform.position, Quaternion.identity, transform);
        }
    }
}