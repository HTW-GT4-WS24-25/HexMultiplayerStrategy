using UnityEngine;

namespace Core.Combat
{
    [CreateAssetMenu(fileName = "DamageCurves", menuName = "Combat/DamageCurves", order = 0)]
    public class DamageCurves : ScriptableObject
    {
        [field: SerializeField] public AnimationCurve Default { get; private set; }
    }
}