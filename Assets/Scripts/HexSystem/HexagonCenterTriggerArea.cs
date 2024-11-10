using Unit;
using UnityEngine;

namespace HexSystem
{
    public class HexagonCenterTriggerArea : MonoBehaviour
    {
        private Hexagon _hexagon;

        private void Awake()
        {
            _hexagon = GetComponentInParent<Hexagon>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("UnitGroup"))
            {
                Debug.Log("Unit entered hex center area");
                var unitGroup = other.GetComponentInParent<UnitGroup>();
                
                GameEvents.UNIT.OnUnitEnteredHexCenterArea?.Invoke(_hexagon, unitGroup);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("UnitGroup"))
            {
                Debug.Log("Unit left hex center area");
                var unitGroup = other.GetComponentInParent<UnitGroup>();
                
                GameEvents.UNIT.OnUnitLeftHexCenterArea?.Invoke(_hexagon, unitGroup);
            }
        }
    }
}