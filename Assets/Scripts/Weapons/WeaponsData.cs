using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Weapons
{
    [CreateAssetMenu(fileName = "Weapons", menuName = "Data/Weapons", order = 0)]
    public class WeaponsData : ScriptableObject
    {
        [SerializeField]
        private List<Weapon> _weapons;

        public List<Weapon> WeaponsList => _weapons;
    }

    [Serializable]
    public struct Weapon
    {
        public GameObject objectToThrow;
        public int manaCost;
        public int damage;
        public float speed;
    }
}