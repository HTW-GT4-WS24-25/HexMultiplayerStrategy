using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class HexTypeDataProvider : MonoBehaviour
{
    [SerializeField] private List<HexTypeData> HexTypeData = new();
    
    public static HexTypeDataProvider Instance;
    
    private readonly Dictionary<HexType, HexTypeData> _hexDataByType = new();
    
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        
        Instance = this;
        foreach (var typeData in HexTypeData)
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