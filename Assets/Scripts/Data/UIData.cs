using System.Collections.Generic;
using Chars;
using GameUI;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Data/UI Data", order = 0)]
    public class UIData : ScriptableObject
    {
        [SerializeField] private List<GameObject> _objectsToSpawn;
        [SerializeField] private PlayerControlView joystickView;
        [SerializeField] private PlayerInventoryView inventoryView;
        [SerializeField] private AdsMob adsMob;
        public List<GameObject> ObjectsToSpawn => _objectsToSpawn;
        public PlayerControlView PlayerControlView => joystickView;

        public PlayerInventoryView InventoryView => inventoryView;

        public AdsMob AdsMob => adsMob;
    }
}