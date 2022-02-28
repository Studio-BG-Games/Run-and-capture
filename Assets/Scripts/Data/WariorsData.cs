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
    }

    [Serializable]
    public struct WariorInfo
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private WariorType Type;
        [SerializeField, ShowIf("hasWeapon")] private Weapon _weapon;
        private bool hasWeapon => Type != WariorType.Defence;
    }

    public enum WariorType
    {
        Attack,
        Defence,
        Both
    }
}