using System;
using Data;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Items
{
    public abstract class Item : ScriptableObject, IDisposable
    {
        private GameObject _instance;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool isInvokeOnPickUp = false;

        public bool IsInvokeOnPickUp => isInvokeOnPickUp;

        public Sprite Icon => icon;
        public GameObject IconPrefab => iconPrefab;

        protected Unit Unit;
        protected Action OnItemUsed;
        
        public UnitColor Color => Unit.Color;

        public GameObject Spawn(HexCell cell, GameObject parrant)
        {
            _instance = SpawnHelper.Spawn(iconPrefab, cell.transform.position + new Vector3(0, 1, 0), parrant);
            _instance.AddComponent<ItemView>().SetUp(this);
            _instance.AddComponent<CapsuleCollider>().isTrigger = true;
            return _instance;
        }

        public virtual void PickUp(UnitColor color)
        {
            if (HexManager.UnitCurrentCell.TryGetValue(color, out var value))
                Unit = value.unit;
        }

        public void Dispose()
        {
            OnItemUsed = null;
        }
    }
}