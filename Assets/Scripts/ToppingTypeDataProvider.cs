using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class ToppingTypeDataProvider : MonoBehaviour
{
    [SerializeField] private List<ToppingTypeData> HexTypeData = new();
    
    public static ToppingTypeDataProvider Instance;
    
    private readonly Dictionary<ToppingType, ToppingTypeData> _hexDataByType = new();
    
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        
        Instance = this;
        foreach (var typeData in HexTypeData)
        {
            _hexDataByType.Add(typeData.Type, typeData);
        }
    }

    public ToppingTypeData GetData(ToppingType type)
    {
        return _hexDataByType[type];
    }

    public Dictionary<ToppingType, ToppingTypeData> GetAllData()
    {
        return new Dictionary<ToppingType, ToppingTypeData>(_hexDataByType);
    }
}