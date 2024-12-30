using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Unit.Group.Display
{
    public class PopupNumberSpawn : MonoBehaviour
    {
        [SerializeField] private DamageNumber damageNumberPrefab;
        [SerializeField] private DamageNumber healNumberPrefab;
        
        [Button]
        public void SpawnDamageNumber(float damage)
        {
            damageNumberPrefab.Spawn(transform.position, damage);
        }

        [Button]
        public void SpawnHealNumber(float healAmount)
        {
            healNumberPrefab.Spawn(transform.position, healAmount);
        }
        
    }
}
