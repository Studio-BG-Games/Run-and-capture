﻿using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public struct Weapon
    {
        public GameObject objectToThrow;
        public GameObject VFXGameObject;
        public int manaCost;
        public int damage;
        public float speed;
        public int disnatce;
        public float reloadTime;
        public int shots;
    }
}