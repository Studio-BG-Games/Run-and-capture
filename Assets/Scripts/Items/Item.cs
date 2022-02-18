using System;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using DG.Tweening;
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

    public abstract class Item : ScriptableObject
    {
        private GameObject _instance;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemType type;

        public ItemType Type => type;
        
        public Sprite Icon => icon;


        public virtual void Invoke(ItemContainer container)
        {
            
        }
        

        public GameObject Spawn(HexCell cell, GameObject parent, GameObject iconPrefab)
        {
            _instance = GameObject.Instantiate(iconPrefab, cell.transform.position + new Vector3(0, 1, 0),
                Quaternion.identity, parent.transform);
            var view = _instance.AddComponent<ItemView>();
            view.SetUp(this);
            cell.Item = view;
            _instance.AddComponent<CapsuleCollider>().isTrigger = true;
            return _instance;
        }
        
        
    }
}