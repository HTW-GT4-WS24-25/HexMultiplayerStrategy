using Core.HexSystem;
using Core.HexSystem.Generation;
using Core.HexSystem.Hex;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGeneratorTestScene
{
    public class TestMapBuilder : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Hexagon forestHexPrefab;
        [SerializeField] private Hexagon grassHexPrefab; 
        [SerializeField] private Hexagon mountainHexPrefab;
        
        [SerializeField] private MapGenerationConfig mapGenerationConfig;
        [SerializeField] private int nRings = 3;
        [SerializeField] private float spacing = 0;
        
        private const float TileWidth = 1;
    
        private MapDataGenerator _mapDataGenerator;
        private HexagonGrid _grid;
        private float _horizontalSpacing;
        private float _verticalSpacing;

        private Vector3 _qOffset;
        private Vector3 _rOffset;
    
        private void Awake()
        {
            _horizontalSpacing = TileWidth + spacing;

            var tileHeight = 2 * TileWidth / Mathf.Sqrt(3) + spacing;
            _verticalSpacing = 0.75f * tileHeight + spacing;

            _qOffset = new Vector3(_horizontalSpacing, 0, 0);
            _rOffset = new Vector3(0.5f * _horizontalSpacing, 0, _verticalSpacing);

            _grid = new HexagonGrid();
        }

        private void Start()
        {
            _mapDataGenerator = new MapDataGenerator(mapGenerationConfig);
            BuildNewMap();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) BuildNewMap();
        }

        private void BuildNewMap()
        {
            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
            _grid.Clear();

            var randomMapData = _mapDataGenerator.Generate(nRings);
            BuildMap(randomMapData);
        }

        private void BuildMap(int[] mapData)
        {
            var rotation180 = Quaternion.Euler(0, 180, 0);
            var dataIndex = 0;
    
            foreach (var coordinates in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
            {
                var hexPosition = _qOffset * coordinates.Q + _rOffset * coordinates.R;
                var hexPrefab = mapData[dataIndex++] switch
                {
                    (int)HexType.Mountain => mountainHexPrefab,
                    (int)HexType.Forest => forestHexPrefab,
                    _ => grassHexPrefab
                };                
                
                var randomHexRotation = Random.Range(0, 2) == 0 ? rotation180 : Quaternion.identity;
                var newHex = Instantiate(hexPrefab, hexPosition, randomHexRotation, transform);
               
                newHex.Initialize(coordinates);
                _grid.Add(newHex);
            }
        }
    }
}