using System;
using Data;
using HexFiled;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    
    public class Building : Item
    {
        
        public Building(ItemInfo data) : base(data)
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
            var obj = Object.Instantiate(Data.SpawnablePrefab, cell.transform.position + Data.SpawnablePrefab.transform.position, Quaternion.identity);
            obj.GetComponent<TowerView>().SetUp(Unit.Color);
            cell.Building =  obj.GetComponent<TowerView>();
            OnItemUsed?.Invoke(this);
        }
    }
}