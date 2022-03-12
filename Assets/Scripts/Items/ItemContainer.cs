using System;
using HexFiled;
using Items.ItemViews;
using Units;
using UnityEngine;

namespace Items
{
    public class ItemContainer
    {
        private ItemView _instance;

        public Action OnItemUsed;

        public Item Item { get; }

        public ItemView Instance => _instance;

        public UnitBase Unit { get; }

        public GameObject AimInstance { get; set; }

        public HexDirection HexDirection { get; set; }
        
        public Vector2 Direction { get; set; }

        public UnitBase Value { get; set; }

        public ItemContainer(Item item, ItemView instance, UnitBase unit)
        {
            Item = item;
            _instance = instance;
            Unit = unit;
        }

        public void DeAim()
        {
            AimInstance.SetActive(false);
        }
        
    }
}