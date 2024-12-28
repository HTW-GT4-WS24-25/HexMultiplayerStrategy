using System.Collections.Generic;
using Core.HexSystem.Hexagon;
using UnityEngine;

namespace Core.HexSystem
{
    public class HexTypeDataProvider : MonoBehaviour
    {
        public static HexTypeDataProvider Instance;
    
        [SerializeField] private List<HexTypeData> hexTypeData = new();
    
        private readonly Dictionary<HexType, HexTypeData> _hexDataByType = new();
    
        private void Awake()
        {
            if (Instance != null) 
                Destroy(this);
        
            Instance = this;
            DontDestroyOnLoad(gameObject);
        
            foreach (var typeData in hexTypeData)
            {
                _hexDataByType.Add(typeData.Type, typeData);
            }
        }

        public HexTypeData GetData(HexType type)
        {
            return _hexDataByType[type];
        }

        public Dictionary<HexType, HexTypeData> GetAllData()
        {
            return new Dictionary<HexType, HexTypeData>(_hexDataByType);
        }
    }
}