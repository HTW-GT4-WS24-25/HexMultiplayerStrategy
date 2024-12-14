using HexSystem;
using UnityEngine;

public class UnitSpeedCalculator : MonoBehaviour
{
    private const float defaultSpeed = 0.5f;

    public static float Calculate(HexagonData hexagonData)
    {
        var hexTypeData = HexTypeDataProvider.Instance.GetData(hexagonData.HexType);
        
        return defaultSpeed * hexTypeData.MovementSpeedFactor;
    }
}
