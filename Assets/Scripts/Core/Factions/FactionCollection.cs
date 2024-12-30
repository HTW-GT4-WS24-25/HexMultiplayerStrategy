using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Factions
{
    [CreateAssetMenu(fileName = "FactionCollection", menuName = "Factions/FactionCollection", order = 0)]
    public class FactionCollection : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<FactionType, Faction> _factionsByType;

        public Faction GetFaction(FactionType factionType)
        {
            return _factionsByType[factionType];
        }
    }
}