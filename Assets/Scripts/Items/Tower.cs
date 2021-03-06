using System;
using Data;
using HexFiled;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    public class Tower : Item
    {
        
        public Tower(ItemInfo data) : base(data)
        {
            
        }
        
        public override void Invoke(Action<Item> action)
        {
            OnItemUsed += action;
        }

        public override void InstanceInvoke()
        {
            Unit.UseItem(this);
        }

        public override void PlaceItem(HexCell cell)
        {
            Unit.UseItem(this);
            var obj = Object.Instantiate(Data.SpawnablePrefab, cell.transform.position, Quaternion.identity);
            obj.AddComponent<TowerView>().SetUp(Unit.Color);
            OnItemUsed?.Invoke(this);
        }
    }
}