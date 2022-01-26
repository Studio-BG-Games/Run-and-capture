using System;
using DefaultNamespace;
using HexFiled;
using Units;
using UnityEngine;

namespace Items
{
    public enum ItemType
    {
        ATTACK,
        DEFENCE
    }

    [Serializable]
    public struct ItemIcon
    {
        [SerializeField] private ItemType type;
        [SerializeField] private GameObject prefab;

        public ItemType Type => type;

        public GameObject Prefab => prefab;
    }

    public abstract class Item : ScriptableObject, IDisposable
    {
        private GameObject _instance;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool isInvokeOnPickUp = false;
        [SerializeField] private ItemType type;

        public ItemType Type => type;

        public bool IsInvokeOnPickUp => isInvokeOnPickUp;

        public Sprite Icon => icon;

        protected Unit Unit;
        protected Action OnItemUsed;

        public UnitColor Color => Unit.Color;

        public GameObject Spawn(HexCell cell, GameObject parent, GameObject iconPrefab)
        {
            _instance = SpawnHelper.Spawn(iconPrefab, cell.transform.position + new Vector3(0, 1, 0), parent);
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