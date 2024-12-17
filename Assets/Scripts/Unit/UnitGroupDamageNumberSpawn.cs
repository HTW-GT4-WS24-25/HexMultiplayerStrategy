using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Unit
{
    public class UnitGroupDamageNumberSpawn : MonoBehaviour
    {
        [SerializeField] private DamageNumber damageNumberPrefab;
        
        [Button]
        public void SpawnDamageNumber(float damage)
        {
            damageNumberPrefab.Spawn(transform.position, damage);
        }

    }
}
