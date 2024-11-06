using UnityEngine;

[CreateAssetMenu(fileName = "New MapGenerationConfig", menuName = "Map Generation Config")]
public class MapGenerationConfig : ScriptableObject
{
    [Range(0f, 1f)]
    public float mountainChance;
}