using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Weapons;

namespace DefaultNamespace.Weapons
{
    [CreateAssetMenu(fileName = "Weapons", menuName = "Data/Weapons", order = 0)]
    public class WeaponsData : ScriptableObject
    {
        [SerializeField]
        private List<Weapon> _weapons;

        public List<Weapon> WeaponsList => _weapons;
    }
}