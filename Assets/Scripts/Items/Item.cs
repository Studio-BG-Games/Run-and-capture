﻿using System;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using HexFiled;
using Items.ItemViews;
using Units;
using UnityEngine;
using Object = UnityEngine.Object;

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
        [SerializeField] private ItemType type;

        public ItemType Type => type;
        
        public Sprite Icon => icon;

        protected Unit Unit;
        protected Action<Unit> OnItemUsed;

        public UnitColor Color => Unit.Color;

        public GameObject Spawn(HexCell cell, GameObject parent, GameObject iconPrefab)
        {
            _instance = GameObject.Instantiate(iconPrefab, cell.transform.position + new Vector3(0, 1, 0),
                Quaternion.identity, parent.transform);
            _instance.AddComponent<ItemView>().SetUp(this);
            cell.Item = this;
            _instance.AddComponent<CapsuleCollider>().isTrigger = true;
            return _instance;
        }

        public virtual void PickUp(Unit unit)
        {
            
            unit.PickUpItem(this);
            Despawn();
        }

        public void Despawn()
        {
            Destroy(_instance.gameObject);
        }

        public void Dispose()
        {
            OnItemUsed = null;
        }
    }
}