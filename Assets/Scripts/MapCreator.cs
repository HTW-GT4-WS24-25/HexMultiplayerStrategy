using UnityEngine;
using Random = UnityEngine.Random;

public class MapCreator : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Tile grassTilePrefab;
    [SerializeField] private Tile mountainTilePrefab;
    
    [Header("Settings")]
    [SerializeField] private float tileWidth;
    [SerializeField] private float spacing;
    [Range(0f, 1f)]
    [SerializeField] private float mountainChance;

    private float _horizontalSpacing;
    private float _verticalSpacing;
    
    private void Awake()
    {
        _horizontalSpacing = tileWidth + spacing;

        var tileHeight = 2 * tileWidth / Mathf.Sqrt(3) + spacing;
        _verticalSpacing = 0.75f * tileHeight + spacing;
    }

    private void Start()
    {
        CreateMap();
    }

    private void CreateMap()
    {
        CreateRow(transform.position + new Vector3(-_horizontalSpacing, 0, 2*_verticalSpacing), 3);
        CreateRow(transform.position + new Vector3(-1.5f*_horizontalSpacing, 0, _verticalSpacing), 4);
        CreateRow(transform.position + new Vector3(-2*_horizontalSpacing, 0, 0), 5);
        CreateRow(transform.position + new Vector3(-1.5f*_horizontalSpacing, 0, -_verticalSpacing), 4);
        CreateRow(transform.position + new Vector3(-_horizontalSpacing, 0, -2*_verticalSpacing), 3);
    }

    private void CreateRow(Vector3 leftPosition, int amount)
    {
        var tilePosition = leftPosition;
        var rotation180 = Quaternion.Euler(0, 180, 0);
        
        for (var i = 0; i < amount; i++)
        {
            var randomTilePrefab = Random.Range(0f, 1f) <= mountainChance ? mountainTilePrefab : grassTilePrefab;
            var randomTileRotation = Random.Range(0, 2) == 0 ? rotation180 : Quaternion.identity;
            
            Instantiate(randomTilePrefab, tilePosition, randomTileRotation, transform);
            tilePosition.x += _horizontalSpacing;
        }
    }
}
