using System;
using Data;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    public abstract class Item : ScriptableObject
    {
        private GameObject _instance;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        public GameObject IconPrefab => iconPrefab;

        protected Unit Unit;
        protected Action OnItemUsed;

        public UnitColor Color => Unit.Color;

        public GameObject Spawn(HexCell cell)
        {
            var obj = SpawnHelper.Spawn(iconPrefab, cell.transform.position + new Vector3(0, 1, 0));
            obj.AddComponent<ItemView>().SetUp(this);
            obj.AddComponent<CapsuleCollider>().isTrigger = true;
            return obj;
        }

        public void PickUp(Unit unit)
        {
            Unit = unit;
            
        }
    }
}