using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HexConqueringEffect : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MeshRenderer floorRenderer;
    [SerializeField] private MeshRenderer wallRenderer;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;

    [Header("Settings")] 
    [SerializeField] private float circleDiameterOnFadeOut;
    [SerializeField] private float maxWallLevel;
    [SerializeField] private float minWallLevel;
    [SerializeField] private float fadeInAnimationDuration;
    [SerializeField] private float fadeOutAnimationDuration;

    private static readonly int HighlightColorId = Shader.PropertyToID("_Highlight_Color");
    private static readonly int CircleDiameterId = Shader.PropertyToID("_Circle_Diameter");
    private static readonly int WallLevelId = Shader.PropertyToID("_Highlight_Level");
    
    private Tween _effectAnimation;
    private Material _effectFloorMaterial;
    private Material _effectWallMaterial;

    [Button]
    public void PlayConqueringEffect(Color playerColor)
    {
        _effectFloorMaterial = new Material(floorMaterial);
        floorRenderer.material = _effectFloorMaterial;
        
        _effectWallMaterial = new Material(wallMaterial);
        _effectWallMaterial.SetFloat(WallLevelId, minWallLevel);
        wallRenderer.material = _effectWallMaterial;
        
        var startColor = playerColor;
        SetEffectColor(startColor);
        
        var endColor = BrightUpColor(playerColor, 3f);
        
        gameObject.SetActive(true);
        var effectAnimationSequence = DOTween.Sequence();
        effectAnimationSequence.Append(DOVirtual.Color(startColor, endColor, fadeInAnimationDuration, SetEffectColor));
        effectAnimationSequence.Join(DOVirtual.Float(_effectWallMaterial.GetFloat(WallLevelId), maxWallLevel, fadeInAnimationDuration, value => _effectWallMaterial.SetFloat(WallLevelId, value)));
        effectAnimationSequence.Append(DOVirtual.Float(maxWallLevel, minWallLevel, fadeOutAnimationDuration, value => _effectWallMaterial.SetFloat(WallLevelId, value)));
        effectAnimationSequence.Join(DOVirtual.Float(_effectFloorMaterial.GetFloat(CircleDiameterId), circleDiameterOnFadeOut, fadeOutAnimationDuration, value => _effectFloorMaterial.SetFloat(CircleDiameterId, value)));
        effectAnimationSequence.Join(DOVirtual.Color(endColor, Color.black, fadeOutAnimationDuration, SetEffectColor).SetEase(Ease.InCirc));
        effectAnimationSequence.OnComplete(() => gameObject.SetActive(false));
        _effectAnimation = effectAnimationSequence;
    }

    private void SetEffectColor(Color color)
    {
        _effectFloorMaterial.SetColor(HighlightColorId, color);
        _effectWallMaterial.SetColor(HighlightColorId, color);
    }

    private Color BrightUpColor(Color color, float intensity)
    {
        return new Color(
            color.r * Mathf.Pow(2, intensity),
            color.g * Mathf.Pow(2, intensity),
            color.b * Mathf.Pow(2, intensity),
            color.a);
    }
}
