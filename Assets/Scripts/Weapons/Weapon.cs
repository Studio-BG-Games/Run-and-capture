using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public struct Weapon
    {
        public string name;
        public GameObject icon;
        public GameObject objectToThrow;
        public GameObject VFXGameObject;
        public int modifiedDamage;
        public int damage;
        public float speed;
        public int disnatce;
        public float reloadTime;
        public int shots;
        public AudioClip shotSound;
        public AudioClip hitSound;

        public void SetModifiedDamage(int bonus)
        {
            modifiedDamage = damage + bonus;
        }
    }
}