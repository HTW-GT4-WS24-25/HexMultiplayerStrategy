using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class ToppingTypeDataProvider : MonoBehaviour
{
    [SerializeField] private List<ToppingTypeData> ToppingTypeData = new();
    
    public static ToppingTypeDataProvider Instance;
    
    private readonly Dictionary<ToppingType, ToppingTypeData> _toppingDataByType = new();
    
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        
        Instance = this;
        foreach (var typeData in ToppingTypeData)
        {
            _toppingDataByType.Add(typeData.Type, typeData);
        }
    }

    public ToppingTypeData GetData(ToppingType type)
    {
        return _toppingDataByType[type];
    }

    public Dictionary<ToppingType, ToppingTypeData> GetAllData()
    {
        return new Dictionary<ToppingType, ToppingTypeData>(_toppingDataByType);
    }
}