using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Unit.Group.Display
{
    public class DamageNumberSpawn : MonoBehaviour
    {
        [SerializeField] private DamageNumber damageNumberPrefab;
        
        [Button]
        public void SpawnDamageNumber(float damage)
        {
            damageNumberPrefab.Spawn(transform.position, damage);
        }

    }
}
