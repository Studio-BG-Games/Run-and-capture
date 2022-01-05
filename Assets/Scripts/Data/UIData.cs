﻿using System.Collections.Generic;
using Chars;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Data/UI Data", order = 0)]
    public class UIData : ScriptableObject
    {
        [SerializeField] private List<GameObject> _objectsToSpawn;
        [SerializeField] private PlayerControlView joystickView;
        [SerializeField] private PlayerInventoryView inventoryView;
        public List<GameObject> ObjectsToSpawn => _objectsToSpawn;
        public PlayerControlView PlayerControlView => joystickView;

        public PlayerInventoryView InventoryView => inventoryView;
    }
}