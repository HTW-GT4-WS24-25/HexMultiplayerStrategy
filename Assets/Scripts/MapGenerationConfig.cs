using UnityEngine;

[CreateAssetMenu(fileName = "New MapGenerationConfig", menuName = "Map Generation Config")]
public class MapGenerationConfig : ScriptableObject
{
    [Header("Mountain Distribution")]
    
    [Range(0f, 1f)]
    public float mountainProbability;

    [Range(0f, 10f)]
    public float mountainProbabilityEnforcement;

    public int mountainAreaCheckingSize;

    [Header("Forest Distribution")]
    
    [Range(0f, 1f)]
    public float forestProbability;

    [Range(0f, 10f)]
    public float forestProbabilityEnforcement;

    public int forestAreaCheckingSize;

    [Header("Forest Generation")] 
    
    [Range(0f, 1f)]
    public float diminishingProbabilityPerDistance;
}