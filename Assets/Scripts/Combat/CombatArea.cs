﻿using System;
using Unit;
using UnityEngine;

namespace Combat
{
    public class CombatArea : MonoBehaviour
    {
        private float _radius;
        private Combat _combat;

        public void Initialize(Combat combat, float radius)
        {
            _combat = combat;
            GetComponent<CapsuleCollider>().radius = radius;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("UnitGroup"))
                return;
            
            var collidedUnitGroup = other.GetComponentInParent<UnitGroup>();
            
            if (collidedUnitGroup.Movement.IsFighting)
                return;

            _combat.JoinCombat(collidedUnitGroup);
        }
    }
}