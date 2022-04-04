using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Data
{
    [CreateAssetMenu(fileName = "WariorsData", menuName = "Data/Wariors data")]
    public class WariorsData : ScriptableObject
    {
        [SerializeField] private List<WariorInfo> _wariors;
        public WariorInfo Wariors { get { return _wariors[0]; } }
    }

    [Serializable]
    public partial struct WariorInfo
    {
        public GameObject wariorPrefab;
        [SerializeField] private WariorType Type;
        [SerializeField, ShowIf("hasWeapon")] private Weapon _weapon;
        public int maxHP;
        public int manaRegen;
        public int maxMana; 
        private bool hasWeapon => Type != WariorType.Defence;
    }

    public enum WariorType
    {
        Attack,
        Defence,
        Both
    }
}