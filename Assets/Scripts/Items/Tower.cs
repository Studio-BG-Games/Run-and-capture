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
            throw new System.NotImplementedException();
        }

        public override void PlaceItem(HexCell cell)
        {
            Object.Instantiate(Data.SpawnablePrefab, cell.transform.position, Quaternion.identity);
            OnItemUsed?.Invoke(this);
        }
    }
}