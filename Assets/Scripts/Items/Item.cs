using System;
using Data;
using HexFiled;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    public abstract class Item : IDisposable
    {
        private GameObject _instance;
        
        protected ItemInfo Data;
        protected Unit Unit;
        protected Action<Item> OnItemUsed;

        public bool IsInstantUse => Data.IsInstanceUse;
        public Sprite Icon => Data.Icon;
        public UnitColor Color => Unit.Color;
        protected Item(ItemInfo data)
        {
            Data = data;
        }

        public void PickUp(Unit unit)
        {
            Unit = unit;
        }

        public GameObject Spawn(HexCell cell)
        {
            _instance = Object.Instantiate(Data.Prefab, cell.transform.position + new Vector3(0, 1, 0),
                Quaternion.identity);
            return _instance;
        }
        

        public abstract void Invoke(Action<Item> item);
        public abstract void InstanceInvoke();
        public abstract void PlaceItem(HexCell cell);

        public void Dispose()
        {
            Object.Destroy(_instance);
        }
    }
}